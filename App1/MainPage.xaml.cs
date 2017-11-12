﻿using System;
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
using Windows.UI.Xaml.Navigation;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage.Streams;
using System.Threading.Tasks;
using System.Threading;
using Windows.Storage;
using Microsoft.ProjectOxford.Common.Contract;
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
        const string subscriptionKey = "0cfd75d5d8054f768ddcd03fbc3bd376";
        string activeId;
        private MediaCapture mediaCapture;
        private StorageFile photostorage;
        private FaceServiceClient faceServiceClient = new FaceServiceClient(subscriptionKey.ToString(), "https://northeurope.api.cognitive.microsoft.com/face/v1.0/");
        private readonly string PHOTO_FILE_NAME = "photo.jpg";
        public string personGroupId = "facegroup";
        public KeyValuePair<string, float> maxemotion = new KeyValuePair<string, float>();

     



        public MainPage()
        {
            
            this.InitializeComponent();

            timetest();
           viewCamera();
           Task P = Task.Run(()=> { takePhoto(); }) ;
          Task t = Task.Run(()=> { GroupTest(); });
            WhoIsTimer();

           CheckFaceTimer();

        }
        
        public void timetest()
        {
            DispatcherTimer disptime;
            disptime = new DispatcherTimer();

            disptime.Interval = new TimeSpan(0, 0, 0, 1);
            disptime.Tick += disptime_Tick;

            disptime.Start();

        }



        private void disptime_Tick(object sender, object e)
        {
            tbl_timenow.Text = TimeNow();
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
            if(activeId != null)
            {
                tbl_status.Text = activeId.ToString();
            }

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
           
            /* if(activeId != null && activeId != CheckFace().ToString())
             {

             }
             if (activeId != null )
             {
                 tbl_status.Text = activeId.ToString();
             }*/
            Task t = Task.Run(() => { takePhoto();
                CheckFace(); });
        }




        private string TimeNow()
        {
            string time;

            TimeSpan t = DateTime.UtcNow.TimeOfDay;
            time = string.Format("{0:D2}:{1:D2}:{2:D2}", t.Hours, t.Minutes, t.Seconds);
            return time;
        }
        private async void GroupTest()
        {
            var photodir = await KnownFolders.PicturesLibrary.GetFileAsync(PHOTO_FILE_NAME);
            string photo =  photodir.Path;

            string picdir = photo.Substring(0, photo.Length - 9);

            

            try
            {
                await faceServiceClient.CreatePersonGroupAsync(personGroupId, "FaceGroup");
                 //tbl_status.Text = "Group created";
            }
            catch
            {

                //tbl_status.Text = "Group exists";
            }

            try
            {
                var requiredFaceAttributes = new FaceAttributeType[] {
                FaceAttributeType.Age,
                FaceAttributeType.Gender,
                FaceAttributeType.Smile,
                FaceAttributeType.FacialHair,
                FaceAttributeType.HeadPose,
                FaceAttributeType.Glasses,
                FaceAttributeType.Emotion
            };



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


                

                using (Stream s = File.OpenRead(testImageFile))
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
                        //var emotionlist = emotion.ToRankedList();
                        // emo = emotionlist.Max().Value;
                       // emotionstring = emotion.Happiness.ToString();
                       
                         
                    }
          
                    //emo.ToString();
                    
                    var faceIds = faces.Select(face => face.FaceId).ToArray();
                    var results = await faceServiceClient.IdentifyAsync(personGroupId, faceIds);


                    foreach (var identifyResult in results)
                    {
                        //  tbl_status.Text = ("Result of face: " + identifyResult.FaceId);
                        if (identifyResult.Candidates.Length == 0)
                        {
                            //tbl_status.Text = ("No one identified, i will add you now, your new name is Bill");
                            CreatePersonResult friend1 = await faceServiceClient.CreatePersonAsync(
                            // Id of the person group that the person belonged to
                            personGroupId,
                            // Name of the person
                            "Face " + results.Count()
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
                            var candidateId = identifyResult.Candidates[0].PersonId;
                            var person = await faceServiceClient.GetPersonAsync(personGroupId, candidateId);


                            activeId = person.Name;

                        }
                        else
                        {
                            
                            // Get top 1 among all candidates returned
                            var candidateId = identifyResult.Candidates[0].PersonId;
                            var person = await faceServiceClient.GetPersonAsync(personGroupId, candidateId);
                            //tbl_status.Text = ("Identified as " + person.Name);
                            //activeId = person.Name.ToString();
                            //await faceServiceClient.UpdatePersonAsync(personGroupId, person.PersonId, "Ólafur Jón");
                            activeId = person.Name.ToString();


                        }
                    }
                }
            }
            catch(Exception e)
            {
                activeId = "Main: " + e.InnerException.Message;

            }

        }


        private async void viewCamera()
        {
            mediaCapture = new MediaCapture();
            await mediaCapture.InitializeAsync();

           CameraPreview.Source = mediaCapture;
           await mediaCapture.StartPreviewAsync();

        }
        private async void takePhoto()
        {
            Face[] face;

            Random r = new Random();
            photostorage = await KnownFolders.PicturesLibrary.CreateFileAsync(PHOTO_FILE_NAME, CreationCollisionOption.ReplaceExisting);

            ImageEncodingProperties imageProperties = ImageEncodingProperties.CreateJpeg();
            imageProperties.Height = 1200;
            imageProperties.Width = 1800;
            await mediaCapture.CapturePhotoToStorageFileAsync(imageProperties, photostorage);

            using (Stream s = File.OpenRead(await GetPhoto()))
            {
               
                var faces = await faceServiceClient.DetectAsync(s);
                face = faces;

            }
            if (face.Length <= 0)
            {
                takePhoto();
            }
        }



            
           
        

        private async Task<string> GetPhoto()
        {

            var photodir = await KnownFolders.PicturesLibrary.GetFileAsync(PHOTO_FILE_NAME);
            
               
            return photodir.Path;
        }


        private async void CheckFace()
        {
         
            try
            {
                
                var photodir = await KnownFolders.PicturesLibrary.GetFileAsync(PHOTO_FILE_NAME);
                string photo = photodir.Path;

                var requiredFaceAttributes = new FaceAttributeType[] {
                FaceAttributeType.Age,
                FaceAttributeType.Gender,
                FaceAttributeType.Smile,
                FaceAttributeType.FacialHair,
                FaceAttributeType.HeadPose,
                FaceAttributeType.Glasses,
                FaceAttributeType.Emotion
            };


                using (Stream s = File.OpenRead(photo))
                {
                    var faces = await faceServiceClient.DetectAsync(s, returnFaceLandmarks: false,
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
                            //tbl_status.Text = ("Identified as " + person.Name);
                            //activeId = person.Name.ToString();

                            activeId = person.Name.ToString();

                        }
                    }
                }
            }
            catch (Exception e)
            {
                activeId = "CheckFace= " + e.Message + e.StackTrace;
            }
        }



        private void tbl_timenow_SelectionChanged(object sender, RoutedEventArgs e)
        {

        }

        private void textBlock_SelectionChanged(object sender, RoutedEventArgs e)
        {

        }
    }
}
