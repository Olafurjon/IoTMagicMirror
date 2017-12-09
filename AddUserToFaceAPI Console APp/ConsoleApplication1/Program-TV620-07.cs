using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using System.Linq;
using Microsoft.ProjectOxford.Face.Contract;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Microsoft.ProjectOxford.Common.Contract;
using Microsoft.ProjectOxford.Face;


namespace CSHttpClientSample
{
    static class Program
    {
        // **********************************************
        // *** Update or verify the following values. ***
        // **********************************************

        // Replace the subscriptionKey string value with your valid subscription key.
        const string subscriptionKey = "0cfd75d5d8054f768ddcd03fbc3bd376";
        




        static void Main()
        {

            GroupTest();
            Console.ReadLine();
            /*
            FaceTest();
            Console.ReadLine();

                // Get the path and filename to process from the user.
                Console.WriteLine("Detect faces:");
                Console.Write("Enter the path to an image with faces that you wish to analzye: ");
                string imageFilePath = Console.ReadLine();

                // Execute the REST API call.
                MakeAnalysisRequest(imageFilePath);

                Console.WriteLine("\nPlease wait a moment for the results to appear. Then, press Enter to exit...\n");
                Console.ReadLine();


      
            Console.WriteLine("Enter an ID for the group you wish to create:");
            Console.WriteLine("(Use numbers, lower case letters, '-' and '_'. The maximum length of the personGroupId is 64.)");

            string personGroupId = Console.ReadLine();
            MakeCreateGroupRequest(personGroupId);

            Console.WriteLine("\n\n\nWait for the result below, then hit ENTER to exit...\n\n\n");
            Console.ReadLine();
                            MakeRequest();
            Console.WriteLine("Hit ENTER to exit...");
            Console.ReadLine();
             *               */





        }

        static async void GroupTest()
        {
           FaceServiceClient faceServiceClient = new FaceServiceClient(subscriptionKey.ToString(), "https://northeurope.api.cognitive.microsoft.com/face/v1.0/");
            // Create an empty person group
            string personGroupId = "mytestersd";
            try 
            {

                
                await faceServiceClient.CreatePersonGroupAsync(personGroupId, "My Friends");
                Console.WriteLine("Group created");
            }
            catch
            {

                Console.WriteLine("Group Exists");
            }




            CreatePersonResult friend1 = await faceServiceClient.CreatePersonAsync(
                // Id of the person group that the person belonged to
                personGroupId,
                // Name of the person
                "Nafn1"
            );

            CreatePersonResult friend2 = await faceServiceClient.CreatePersonAsync(
            // Id of the person group that the person belonged to
            personGroupId,
            // Name of the person
            "Nafn2"
        );
            var i = 0;
            const string friend1ImageDir = @"C:\Users\3010943379\Pictures\Face1";
            var test = Directory.GetFiles(friend1ImageDir, "*.jpg");
            foreach (string imagePath in Directory.GetFiles(friend1ImageDir, "*.jpg"))
            {
                i++;
                using (Stream s = File.OpenRead(imagePath))
                {
                    // Detect faces in the image and add to Anna
                    await faceServiceClient.AddPersonFaceAsync(
                        personGroupId, friend1.PersonId, s);
                    Console.WriteLine("Face1 picture: " + i);
                }
            }

            const string friend2ImageDir = @"C:\Users\3010943379\Pictures\Face2";
            i = 0;
            foreach (string imagePath in Directory.GetFiles(friend2ImageDir, "*.jpg"))
            {
                i++;
                using (Stream s = File.OpenRead(imagePath))
                {
                    // Detect faces in the image and add to Anna
                    await faceServiceClient.AddPersonFaceAsync(
                        personGroupId, friend2.PersonId, s);
                    Console.WriteLine(" picture: " + i);
                }
            }

            await faceServiceClient.TrainPersonGroupAsync(personGroupId);

            TrainingStatus trainingStatus = null;
            while (true)
            {
                trainingStatus = await faceServiceClient.GetPersonGroupTrainingStatusAsync(personGroupId);



                await Task.Delay(1000);

                if (trainingStatus.Status.ToString() != "running")
                {
                    break;
                }
            }

            if (trainingStatus.Status.ToString() != "running")
            {


                string testImageFile = @"C:\Users\3010943379\Pictures\tester\test.jpg";

                using (Stream s = File.OpenRead(testImageFile))
                {
                    var faces = await faceServiceClient.DetectAsync(s);
                    var faceIds = faces.Select(face => face.FaceId).ToArray();

                    var results = await faceServiceClient.IdentifyAsync(personGroupId, faceIds);
                    foreach (var identifyResult in results)
                    {
                        Console.WriteLine("Result of face: {0}", identifyResult.FaceId);
                        if (identifyResult.Candidates.Length == 0)
                        {
                            Console.WriteLine("No one identified");
                        }
                        else
                        {
                            // Get top 1 among all candidates returned
                            var candidateId = identifyResult.Candidates[0].PersonId;
                            var person = await faceServiceClient.GetPersonAsync(personGroupId, candidateId);
                            Console.WriteLine("Identified as {0}, Confidence: {1}", person.Name, identifyResult.Candidates[0].Confidence);
                        }
                    }
                }
            }

        }


        static async void FaceTest()
        {
            FaceServiceClient faceServiceClient = new FaceServiceClient(subscriptionKey.ToString(), "https://northeurope.api.cognitive.microsoft.com/face/v1.0/");



            string imageUrl ="https://scontent-lht6-1.xx.fbcdn.net/v/t31.0-8/23215886_10154914800207233_6176823990811634886_o.jpg?oh=76552a0d5e6afe0bebf4ec8b09f69a4f&oe=5AA49AE7";
            var requiredFaceAttributes = new FaceAttributeType[] {
                FaceAttributeType.Age,
                FaceAttributeType.Gender,
                FaceAttributeType.Smile,
                FaceAttributeType.FacialHair,
                FaceAttributeType.HeadPose,
                FaceAttributeType.Glasses,
                FaceAttributeType.Emotion
            };
            var faces = await faceServiceClient.DetectAsync(imageUrl,
                returnFaceLandmarks: true,
                returnFaceAttributes: requiredFaceAttributes);

            foreach (var face in faces)
            {
                var id = face.FaceId;
                var attributes = face.FaceAttributes;
                var age = attributes.Age;
                var gender = attributes.Gender;
                var smile = attributes.Smile;
                var facialHair = attributes.FacialHair;
                var headPose = attributes.HeadPose;
                var glasses = attributes.Glasses;
                
            }

        }

        static async void MakeCreateGroupRequest(string personGroupId)
        {
            var client = new HttpClient();

            // Request headers - replace this example key with your valid key.
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey.ToString());

            // Request URI string.
            // NOTE: You must use the same region in your REST call as you used to obtain your subscription keys.
            //   For example, if you obtained your subscription keys from westus, replace "westcentralus" in the 
            //   URI below with "westus".
            string uri = "https://northeurope.api.cognitive.microsoft.com/face/v1.0/persongroups/" + personGroupId;

            // Here "name" is for display and doesn't have to be unique. Also, "userData" is optional.
            string json = "{\"name\":\"My Group\", \"userData\":\"Some data related to my group.\"}";
            HttpContent content = new StringContent(json);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = await client.PutAsync(uri, content);

            // If the group was created successfully, you'll see "OK".
            // Otherwise, if a group with the same personGroupId has been created before, you'll see "Conflict".
            Console.WriteLine("Response status: " + response.StatusCode);
        }
        static async void MakeRequest()
        {
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey.ToString());

            var uri = "https://northeurope.api.cognitive.microsoft.com/face/v1.0/persongroups/{personGroupId}?ok";

            var response = await client.GetAsync(uri);
            Console.WriteLine(response.StatusCode);
        }



        /// <summary>
        /// Gets the analysis of the specified image file by using the Computer Vision REST API.
        /// </summary>
        /// <param name="imageFilePath">The image file.</param>
        static async void MakeAnalysisRequest(string imageFilePath)
        {
            const string uriBase = "https://northeurope.api.cognitive.microsoft.com/face/v1.0/detect";
            HttpClient client = new HttpClient();

            // Request headers.
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);

            // Request parameters. A third optional parameter is "details".
            string requestParameters = "returnFaceId=true&returnFaceLandmarks=false&returnFaceAttributes=age,gender,smile,makeup";

            // Assemble the URI for the REST API Call.
            string uri = uriBase + "?" + requestParameters;

            HttpResponseMessage response;
            
            // Request body. Posts a locally stored JPEG image.
            byte[] byteData = GetImageAsByteArray(imageFilePath);

            using (ByteArrayContent content = new ByteArrayContent(byteData))
            {
                // This example uses content type "application/octet-stream".
                // The other content types you can use are "application/json" and "multipart/form-data".
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                // Execute the REST API call.
                response = await client.PostAsync(uri, content);
                
                
                // Get the JSON response.
                string contentString = await response.Content.ReadAsStringAsync();
                string[] dirtytext = contentString.Split(']');
                Console.WriteLine(dirtytext[0]);
                
                // Display the JSON response.
                Console.WriteLine("\nResponse:\n");
                Console.WriteLine(JsonPrettyPrint(contentString));
            }
        }



        /// <summary>
        /// Returns the contents of the specified file as a byte array.
        /// </summary>
        /// <param name="imageFilePath">The image file to read.</param>
        /// <returns>The byte array of the image data.</returns>
        static byte[] GetImageAsByteArray(string imageFilePath)
        {
            FileStream fileStream = new FileStream(imageFilePath, FileMode.Open, FileAccess.Read);
            BinaryReader binaryReader = new BinaryReader(fileStream);
            return binaryReader.ReadBytes((int)fileStream.Length);
        }


        /// <summary>
        /// Formats the given JSON string by adding line breaks and indents.
        /// </summary>
        /// <param name="json">The raw JSON string to format.</param>
        /// <returns>The formatted JSON string.</returns>
        static string JsonPrettyPrint(string json)
        {
            if (string.IsNullOrEmpty(json))
                return string.Empty;

            json = json.Replace(Environment.NewLine, "").Replace("\t", "");

            StringBuilder sb = new StringBuilder();
            bool quote = false;
            bool ignore = false;
            int offset = 0;
            int indentLength = 3;

            foreach (char ch in json)
            {
                switch (ch)
                {
                    case '"':
                        if (!ignore) quote = !quote;
                        break;
                    case '\'':
                        if (quote) ignore = !ignore;
                        break;
                }

                if (quote)
                    sb.Append(ch);
                else
                {
                    switch (ch)
                    {
                        case '{':
                        case '[':
                            sb.Append(ch);
                            sb.Append(Environment.NewLine);
                            sb.Append(new string(' ', ++offset * indentLength));
                            break;
                        case '}':
                        case ']':
                            sb.Append(Environment.NewLine);
                            sb.Append(new string(' ', --offset * indentLength));
                            sb.Append(ch);
                            break;
                        case ',':
                            sb.Append(ch);
                            sb.Append(Environment.NewLine);
                            sb.Append(new string(' ', offset * indentLength));
                            break;
                        case ':':
                            sb.Append(ch);
                            sb.Append(' ');
                            break;
                        default:
                            if (ch != ' ') sb.Append(ch);
                            break;
                    }
                }
            }

            return sb.ToString().Trim();
        }
    }
}