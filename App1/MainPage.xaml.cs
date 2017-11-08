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
using Windows.UI.Xaml.Navigation;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage.Streams;
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
        private MediaCapture mediaCapture;
        private StorageFile photostorage;
        private readonly string PHOTO_FILE_NAME = "photo.jpg";


        public MainPage()
        {
            
            this.InitializeComponent();

            timetest();
            viewCamera();
            takePhoto();

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

    
        private string TimeNow()
        {
            string time;

            TimeSpan t = DateTime.UtcNow.TimeOfDay;
            time = string.Format("{0:D2}:{1:D2}:{2:D2}", t.Hours, t.Minutes, t.Seconds);
            return time;
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
            photostorage = await KnownFolders.PicturesLibrary.CreateFileAsync( PHOTO_FILE_NAME , CreationCollisionOption.ReplaceExisting);
           ImageEncodingProperties imageProperties = ImageEncodingProperties.CreateJpeg();
            await mediaCapture.CapturePhotoToStorageFileAsync(imageProperties, photostorage);

        }

        private string GetPhoto()
        {
            string photostring;
            photostring = KnownFolders.PicturesLibrary.GetFileAsync(PHOTO_FILE_NAME).ToString();
            return photostring;
        }



        private void tbl_timenow_SelectionChanged(object sender, RoutedEventArgs e)
        {

        }

        private void textBlock_SelectionChanged(object sender, RoutedEventArgs e)
        {

        }
    }
}
