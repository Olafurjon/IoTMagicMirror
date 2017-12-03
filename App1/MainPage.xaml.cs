using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.Media.Capture; //Þetta þarf til þess að nota vefmyndavélina
using Windows.Media.MediaProperties;
using Windows.Storage.Streams;
using System.Threading.Tasks; //Til að vinna með UI þráðinn og tímasetja þetta spes
using System.Threading;
using Windows.Storage; //auðveldar Windows öppum að fara bara í known folders þannig að þú þurfir ekki að vera með eh gisk path
using Microsoft.ProjectOxford.Common.Contract; //Þetta er fyrir APIið til að virka
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;




// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace App1
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        const string subscriptionKey = "0cfd75d5d8054f768ddcd03fbc3bd376"; //Subkeyin sem ég fékk frá microsoft til að nota API-inn
        string activeId;
        string Infostring = null;
        string mood;
        bool punactive = false;
        double personage;
        string hasglasses;
        string persongender;
        object[] activeperson;
        string greetingtime;
        string personname;
        double personsmile;
        string pun;
        public List<string> notalonepun = new List<string>();
        public List<string> possiblepuns = new List<string>();
        string personId;
        int rounds = 0;
        List<object[]> VisitedPersons = new List<object[]>();
        bool grouptestFinished = false;
        bool nofaces = false;
        private List<object[]> list = new List<object[]>();
        private List<string> personlist = new List<string>();
        FaceAttributeType[] requiredFaceAttributes = new FaceAttributeType[] {
                FaceAttributeType.Age,
                FaceAttributeType.Gender,
                FaceAttributeType.Smile,
                FaceAttributeType.FacialHair,
                FaceAttributeType.HeadPose,
                FaceAttributeType.Glasses,
                FaceAttributeType.Emotion
        };

       


        private MediaCapture mediaCapture; //vinnur með myndavélinni
        private StorageFile photostorage; //Hjálpar til með að vista gögnin á einfaldari máta
        private FaceServiceClient faceServiceClient = new FaceServiceClient(subscriptionKey.ToString(), "https://northeurope.api.cognitive.microsoft.com/face/v1.0/");
        private readonly string PHOTO_FILE_NAME = "photo.jpg";
        public string personGroupId = "facegroup"; //Groupann sem vistast í dbinn sem ég á hjá microsoft, eðlilegast væri náttúrulega að það væri kannski 1 grúppa per heimil
                                                   //public KeyValuePair<string, float> maxemotion = new KeyValuePair<string, float>();


            


        public MainPage()
        {

            this.InitializeComponent();
            FillNotAloneList(); //Var í miðju progressi hér kemur basically mögulegar línur sem kemur á spegilinn þegar að það eru fleiri en 1 andlit 
          //var name = InputTextDialogAsync("Input Name");
            //name.Start();
            timetest();
            viewCamera();
            Task P = Task.Run(() => { takePhoto(); }); //Takephoto verður á keyra á sínum eigin þræði eða allavega ekki á sama þræði
            P.Wait(); //er með þetta til öryggis gæti komíð í veg fyrir að hitt lesi myndina í miðjum klíðum við að vista hana
            Task.Delay(1000); //annað backup til þess að vera viss um að ég fái ekki imagesize to small.. 
            
            Task t = Task.Run(() => { //groupest verður að vera keyrt á sínum eigin þræði líka
                GroupTest();
                //CheckFace();
            });
            t.Wait();

            puntimer(); //tikkar og uppfærir miðjutextan á speglinum á 30 sek fresti
            WhoIsTimer(); //uppfærir status timerinn (mun vera implementað öðruvísi fyrir GUIið
            infoTimer(); 
            CheckFaceTimer(); //á mín fresti (vegna 20 calls á mín takmörkunina) er andlitið uppfært og þannig getur það skipt um andlit og fengið nýjar upplýsingar og gert viðeigandi hluti við þær upplýsingar.

        }

        //Uppfærir tímann á ms fresti þegar appið verður tilbúið verður ekki sýnt ms það var bara sýnt fyrir debug purposes
        public void timetest()
        {
            DispatcherTimer disptime;
            disptime = new DispatcherTimer();

            disptime.Interval = new TimeSpan(0, 0, 0, 1);
            disptime.Tick += disptime_Tick;

            disptime.Start();

        }

        //þetta er það sem exec eftir að timetest hefur klikkað á mín fresti 
        private void disptime_Tick(object sender, object e)
        {
            tbl_timenow.Text = TimeNow()[0];
            tbl_dateNow.Text = TimeNow()[1];
        }

        public void puntimer()
        {
            DispatcherTimer disptime;
            disptime = new DispatcherTimer();

            disptime.Interval = new TimeSpan(0, 0, 15);
            disptime.Tick += disp_pun;

            disptime.Start();

        }

        private void disp_pun(object sender, object e)
        {

            if (!punactive )
            {
                if (possiblepuns.Count() != 0)
                {
                    Random ran = new Random();
                    int index = ran.Next(0, possiblepuns.Count());
                    pun = possiblepuns[index];
                    possiblepuns.RemoveAt(index);
                    tbl_pun.Text = pun.ToString();
                    punactive = true;
                }
            }
            if(punactive)
            {
                tbl_pun.Text = "";
                punactive = false;
            }

        }

        public void WhoIsTimer()
        {
            DispatcherTimer disptime;
            disptime = new DispatcherTimer();

            disptime.Interval = new TimeSpan(0, 0, 2);
            disptime.Tick += disptime_WhoIs;

            disptime.Start();

        }

        private void disptime_WhoIs(object sender, object e)
        {
            if (activeId != null)
            {
                tbl_status.Text = activeId.ToString();
            }

        }

        public void infoTimer()
        {
            DispatcherTimer disptime;
            disptime = new DispatcherTimer();

            disptime.Interval = new TimeSpan(0, 0, 2);
            disptime.Tick += Disp_UpdateInfo;

            disptime.Start();

        }

        private void Disp_UpdateInfo(object sender, object e)
        {
            if (Infostring != null)
            {
                tbl_Info.Text = WorkWithInfo(Infostring);
            }

        }

        private string WorkWithInfo(string info)
        {
            return info.ToString();
        }

        public void CheckFaceTimer()
        {
            DispatcherTimer disptime;
            disptime = new DispatcherTimer();

            disptime.Interval = new TimeSpan(0, 1, 0);


            disptime.Tick += disptime_CheckFace;

            disptime.Start();

        }

        private void disptime_CheckFace(object sender, object e)
        {


            Task t = Task.Run(() => { takePhoto(); });
            t.Wait();
            if (t.IsCompleted)
            {
                Task p = Task.Run(() =>
                {
                    //GroupTest();
                    CheckFace();
                });

            }

        }


        //var að vinna með að vera með locally storaðan lista sem myndi upppfærast itl að geta gert persónutengdari hluti og t.d. haldið utan um hve oft hann kemur og fer og ef að emotion skiptist eða sett á sig gleraugu etc..
        public void AddPersonToVisited(string name, double age, string gender, string emotion, string glasses, string id , double smile,bool greeted,int goneandcome)
        {
            object[] person = new object[] {name,age,gender,emotion,glasses,id,smile,greeted,goneandcome };
            VisitedPersons.Add(person);
            activeperson = person;
        }

        //skilar tvískiptum streng eða þá klukkunni og dagsetningunni
        private string[] TimeNow()
        {
            string[] time = new string[2];

            TimeSpan t = DateTime.UtcNow.TimeOfDay;
            DateTime dt = DateTime.UtcNow.Date;

            if(t.Hours < 10 && t.Hours > 06)
            {
                greetingtime = "morning";
            }
            if(t.Hours > 10 && t.Hours < 13)
            {
                greetingtime = "afternoon";
            }
            if(t.Hours > 13 && t.Hours < 18)
            {
                greetingtime = "day";
            }
            if (t.Hours > 18 && t.Hours < 23)
            {
                greetingtime = "evening";
            }
            if (t.Hours > 23 && t.Hours < 06)
            {
                greetingtime = "night";
            }



            time[0] = string.Format("{0:D2}:{1:D2}:{2:D2} {3}/{4}/{5}", t.Hours, t.Minutes, t.Seconds, dt.Day,dt.Month,dt.Year);
            time[1] = string.Format("{0:D2}/{1}/{2}", dt.Day, dt.Month, dt.Year);
            return time;
        }

        //fyrsta kveðjan breytist eftir því hve liðið er á daginn
        private string greeting()
        {
            switch(greetingtime)
            {
                case "morning":
                    return "Good Morning ";
                case "afternoon":
                    return "Good Afternoon ";
                case "day":
                    return "Good Day ";
                case "evening":
                    return "Good Evening ";
                case "night":
                    return "Should you not be sleeping? ";
                default:
                    return "Hello Good-Looking! i mean ";
            }
        }

        //grouptest er í raun mainfunctionið, ég á eftir að implementa hvernig þú stofnar nýjann í miðjum klíðum var að vinna í því þegar Piinn framdi sjálfsmorð... 
        private async void GroupTest()
        {
            var photodir = await KnownFolders.PicturesLibrary.GetFileAsync(PHOTO_FILE_NAME);
            string photo =  photodir.Path;
            
            string picdir = photo.Substring(0, photo.Length - 9);

            /*
                        try
                        {
                            await faceServiceClient.CreatePersonGroupAsync(personGroupId, "FaceGroup");


                             //tbl_status.Text = "Group created";
                        }
                        catch
                        {

                            //tbl_status.Text = "Group exists";
                        }*/

            try
            {


                if (nofaces)
                {
                    return;
                }
                else
                {
                    //smá kóði sem ég notaði til að deleta manneskjum sem hún þekkti í raun ekki en voru óvart stöfnuð án andlita
                    //     await faceServiceClient.DeletePersonAsync(personGroupId, Guid.Parse("d93621c4-496d-4ec0-b56c-3042104abfc5"));
                    // var persons = await faceServiceClient.ListPersonsAsync(personGroupId);
                    /*   foreach(var person in persons)
                       {
                           if(person.PersistedFaceIds.Count() == 0)
                           {
                               personlist.Add(person.PersonId.ToString());
                           }
                       }
                       var lists = personlist;
                       for (int i = 0; i < personlist.Count; i++)
                       {

                           await faceServiceClient.DeletePersonAsync(personGroupId,Guid.Parse(personlist[i]));
                       }*/
                    await faceServiceClient.TrainPersonGroupAsync(personGroupId);

                TrainingStatus trainingStatus = null;
                while (true)
                {
                    trainingStatus = await faceServiceClient.GetPersonGroupTrainingStatusAsync(personGroupId);

                    if (trainingStatus.Status.ToString() != "running")
                    {
                        break;
                    }

                    await Task.Delay(1000);
                }


                string testImageFile = photo;



                    bool firstface = true;
                    using (Stream s = File.OpenRead(await GetPhoto()))
                    {
                        var faces = await faceServiceClient.DetectAsync(s, returnFaceLandmarks: true,
                            returnFaceAttributes: requiredFaceAttributes);

                        foreach (var faceinfo in faces)
                        {

                            var id = faceinfo.FaceId;
                            var attributes = faceinfo.FaceAttributes;
                            var age = attributes.Age;
                            var gender = attributes.Gender;
                            var smile = attributes.Smile;
                            var facialHair = attributes.FacialHair;
                            var headPose = attributes.HeadPose;
                            var glasses = attributes.Glasses;
                            var emotion = attributes.Emotion;
                            var emotionlist = emotion.ToRankedList().First();
                            if (firstface)
                            {

                                mood = emotionlist.Key;
                                hasglasses = glasses.ToString();
                                personId = id.ToString();
                                persongender = gender;
                                personage = age;
                                personsmile = smile;
                                firstface = false;

                            }


                        }

                        //emo.ToString();

                        var faceIds = faces.Select(face => face.FaceId).ToArray();
                        var results = await faceServiceClient.IdentifyAsync(personGroupId, faceIds);



                        foreach (var identifyResult in results)
                        {
                            //  tbl_status.Text = ("Result of face: " + identifyResult.FaceId);
                            if (identifyResult.Candidates.Length == 0)
                            {
                                var name = InputTextDialogAsync("Input Name");
                                //string name = await InputTextDialogAsync("Input Name");
                                //tbl_status.Text = ("No one identified, i will add you now, your new name is Bill");
                                CreatePersonResult friend1 = await faceServiceClient.CreatePersonAsync(
                                // Id of the person group that the person belonged to
                                personGroupId,
                                // Name of the person
                                "Elvis"
                            );

                                for (int z = 0; z < 6; z++)
                                {
                                    Random r = new Random();
                                    photostorage = await KnownFolders.PicturesLibrary.CreateFileAsync((z + PHOTO_FILE_NAME), CreationCollisionOption.ReplaceExisting);
                                    ImageEncodingProperties imageProperties = ImageEncodingProperties.CreateJpeg();
                                    await mediaCapture.CapturePhotoToStorageFileAsync(imageProperties, photostorage);
                                    var friend1ImageDir = await KnownFolders.PicturesLibrary.GetFileAsync(z + PHOTO_FILE_NAME);
                                    string imagePath = friend1ImageDir.Path;

                                    using (Stream k = File.OpenRead(imagePath))
                                    {
                                        await faceServiceClient.AddPersonFaceAsync(
                                            personGroupId, friend1.PersonId, k);
                                    }

                                }


                                await faceServiceClient.TrainPersonGroupAsync(personGroupId);

                                trainingStatus = null;
                                while (true)
                                {
                                    trainingStatus = await faceServiceClient.GetPersonGroupTrainingStatusAsync(personGroupId);

                                    if (trainingStatus.Status.ToString() != "running")
                                    {
                                        break;
                                    }

                                    await Task.Delay(1000);
                                }
                                CheckFace();

                            }
                            else
                            {

                                // Get top 1 among all candidates returned
                                var candidateId = identifyResult.Candidates[0].PersonId;
                                var person = await faceServiceClient.GetPersonAsync(personGroupId, candidateId);
                                //tbl_status.Text = ("Identified as " + person.Name);
                                //activeId = person.Name.ToString();
                                //await faceServiceClient.UpdatePersonAsync(personGroupId, person.PersonId, "Ólafur Jón");
                                activeId = person.Name.ToString() + " " + rounds;


                            }
                        }
                    }
                   
                    rounds++;
                }
                grouptestFinished = true;
            }
            catch (Exception e)
            {
                activeId = "Main: " + activeId + " " + rounds;
                grouptestFinished = true;

            }

        }

        //notað í debugging purposes eða þá kveikir á að streama myndavélina
        private async void viewCamera()
        {
            mediaCapture = new MediaCapture();
            await mediaCapture.InitializeAsync();

          // CameraPreview.Source = mediaCapture;
           //await mediaCapture.StartPreviewAsync();

        }

        //fall sem tekur mynd, það var að rugla í þessu að hafa þetta í while loopu en eftir að ég breytti þessu aðeins minnkaði töluvert villurnar um ImageSize to small
        private async void takePhoto()
        {
            int tries = 0;
            nofaces = false;
                Face[] face;
            //hérna ætlum við að setja myndina í og replacea núverandi til þess að vélin sé ekki hægt og rólega að fyllast af myndum
                photostorage = await KnownFolders.PicturesLibrary.CreateFileAsync(PHOTO_FILE_NAME, CreationCollisionOption.ReplaceExisting);
                //ég forceaði myndina í því að fara í c.a 200kb frekar en 50-70 kb til að reyna útiloka villuna um imagesize sem stoppaði mig þarna
                ImageEncodingProperties imageProperties = ImageEncodingProperties.CreateJpeg();
                imageProperties.Height = 1080;
                imageProperties.Width = 1920;
            //hérna er snappað mynd og sett á staðinn
                await mediaCapture.CapturePhotoToStorageFileAsync(imageProperties, photostorage);
             
                using (Stream s = File.OpenRead(await GetPhoto()))
                {
                    var faces = await faceServiceClient.DetectAsync(s);
                    face = faces;
                }
                //flest hérna útskýrir sig sjálft
                if (face.Length <= 0)
                {
                possiblepuns.Add("Lonely, i am so Lonely i have nobody...");
                    activeId = "Hello? is someone there?";
                    Infostring = "Nobody is home";
                     nofaces = true;
                }
                else if( face.Length > 1)
                {
                Random nr = new Random();


                possiblepuns.Add(notalonepun[nr.Next(0, notalonepun.Count())]);
                }
                else
                {
                nofaces = false;
                    // activeId = "Hello? is someone there?";
                }
                tries++;
          
            
        }

        //sækjum myndina, og þetta þarna delay var precaution þar sem ég var orðin MJÖG desperate að losna við villuna, það er séns að ég geti tekið það út núna en riska það ekki ;)
        private async Task<string> GetPhoto()
        {
            await Task.Delay(10000);

            var photodir = await KnownFolders.PicturesLibrary.GetFileAsync(PHOTO_FILE_NAME);
            
               
            return photodir.Path;
        }

        //þetta er síðan fallið sem er fyrir continous facerec og er keyrt á mín fresti, hér á eftir að implementa að búa  til nýtt nafn
        private async void CheckFace()
        {
            try
            {
                if (grouptestFinished)
                {
                    var photodir = await KnownFolders.PicturesLibrary.GetFileAsync(PHOTO_FILE_NAME);
                    string photo = photodir.Path;

                    string testImageFile = photo;

                    if (nofaces)
                    {

                    }
                    else
                    {
                        bool firstface = true;
                        using (Stream s = File.OpenRead(await GetPhoto()))
                        {
                            var faces = await faceServiceClient.DetectAsync(s, returnFaceLandmarks: true,
                                returnFaceAttributes: requiredFaceAttributes);


                                foreach (var faceinfo in faces)
                                {

                                    var id = faceinfo.FaceId;
                                    var attributes = faceinfo.FaceAttributes;
                                    var age = attributes.Age;
                                    var gender = attributes.Gender;
                                    var smile = attributes.Smile;
                                    var facialHair = attributes.FacialHair;
                                    var headPose = attributes.HeadPose;
                                    var glasses = attributes.Glasses;
                                    var emotion = attributes.Emotion;
                                    var emotionlist = emotion.ToRankedList().First();
                                if (firstface)
                                {
                                   
                                    mood = emotionlist.Key;
                                    hasglasses = glasses.ToString();
                                    personId = id.ToString();
                                    persongender = gender;
                                    personage = age;
                                    personsmile = smile;
                                    firstface = false;
                                    
                                }
                                    
                                // emo = emotionlist.Max().Value;
                                // emotionstring = emotion.Happiness.ToString();
                                //Infostring = "ID: " + id.ToString() + "," + "Age: " + age.ToString() + "," + "Gender: " + gender.ToString() + "," + "Glasses: " + glasses.ToString();
                                Infostring = mood;
                            }
                                

                                var faceIds = faces.Select(face => face.FaceId).ToArray();
                                var results = await faceServiceClient.IdentifyAsync(personGroupId, faceIds);


                                foreach (var identifyResult in results)
                                {
                                    //  tbl_status.Text = ("Result of face: " + identifyResult.FaceId);
                                    if (identifyResult.Candidates.Length == 0)
                                    {
                                        //NewUser
                                        activeId = "createnewUser";

                                    }
                                    else
                                    {
                                        var candidateId = identifyResult.Candidates[0].PersonId;
                                        var person = await faceServiceClient.GetPersonAsync(personGroupId, candidateId);
                                    personname = person.Name;

                                    //tbl_status.Text = ("Identified as " + person.Name);
                                    //activeId = person.Name.ToString();
                                    if (VisitedPersons.Count() != 0)
                                    {
                                        foreach (var check in VisitedPersons)
                                        {

                 
                                            if  (check[0] == activeperson[0])
                                               {
                                                    int comeandgo = Convert.ToInt32(check[8]);
                                                    comeandgo++;
                                                    check[3] = mood;
                                                    check[4] = hasglasses;
                                                    check[6] = personsmile;
                                                    check[8] = comeandgo;
                                                    check[7] = true;
                                                    activeperson = check;
                                                    if (check[2].ToString().ToLower() == "female")
                                                    {
                                                        activeId = "You are looking good gurl ";
                                                     }
                                                    if (check[2].ToString().ToLower() == "male")
                                                    {
                                                        activeId = "You are looking good man! ";
                                                    }


                                               }
                                               else
                                               {

                                                   AddPersonToVisited(personname, personage, persongender, mood, hasglasses, personId, personsmile, false, 0);
                                                   activeId = greeting() + person.Name.ToString();
                                               }  
                                        }
                                    }
                                    else
                                    {
                                        AddPersonToVisited(personname, personage, persongender, mood, hasglasses, personId, personsmile, false, 0);
                                        activeId = greeting() + activeperson[0].ToString();

                                    }

                                    }
                                }
                            }
                        }
                    }
                else { }
            }
               
            catch (Exception e)
            {
                activeId = "CheckFace= " + activeId;
                //CheckFace();
            }
            
            
        }
        //Piinn dó áður en ég gat prufukeyrt þetta af viti
        private async Task<string> InputTextDialogAsync(string name)
        {
            TextBox inputTextBox = new TextBox();
            inputTextBox.AcceptsReturn = false;
            inputTextBox.Height = 32;
            ContentDialog dialog = new ContentDialog();
            dialog.Content = inputTextBox;
            dialog.Title = name;
            dialog.IsSecondaryButtonEnabled = true;
            dialog.PrimaryButtonText = "Ok";
            dialog.SecondaryButtonText = "Cancel";
            if (await dialog.ShowAsync() == ContentDialogResult.Primary)
                return inputTextBox.Text;
            else
                return "Noname";
        }
        private void FillNotAloneList()
        {
            notalonepun.Add("Ah how nice you are not alone i was beginning to worry ");
            notalonepun.Add("I see you have your dog with you ");
            notalonepun.Add("Oh... you brought this with you...");
        }


        private void tbl_timenow_SelectionChanged(object sender, RoutedEventArgs e)
        {

        }

        private void textBlock_SelectionChanged(object sender, RoutedEventArgs e)
        {

        }
    }
}
