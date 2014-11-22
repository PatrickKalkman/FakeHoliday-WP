using System.IO.IsolatedStorage;
using System.Windows;
using System.Windows.Media;

using Caliburn.Micro;
using FakeHoliday.Common;
using FakeHoliday.Resources;

using Nokia.Graphics.Imaging;
using System;
using System.IO;
using System.Windows.Media.Imaging;
using Windows.Storage;

using Telerik.Windows.Controls;

namespace FakeHoliday.ViewModels
{
    public class BlendPageViewModel : FakeHolidayViewModel
    {
        private readonly ImageIO imageIO;
        private readonly FlipTileCreator flipTileCreator;

        public BlendPageViewModel(BackgroundImageBrush backgroundImageBrush, INavigationService navigationService, ILog logger, ImageIO imageIO, FlipTileCreator flipTileCreator)
            : base(backgroundImageBrush, navigationService, logger)
        {
            this.imageIO = imageIO;
            this.flipTileCreator = flipTileCreator;
            PinButtonText = "pin tile";
            PinIcon = new Uri("Assets/PinIcon.png", UriKind.Relative);
            Process();
        }

        public string PageTitle
        {
            get { return AppResources.BlendPageTitle; }
        }
        
        public void Pin()
        {
            flipTileCreator.CreateTile();         
        }

        public async void Process()
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            var backgroundStorageFile = await localFolder.GetFileAsync("BackgroundTemp.jpg");
            var foregroundStorageFile = await localFolder.GetFileAsync("ForegroundTemp.jpg");
            var maskStorageFile = await localFolder.GetFileAsync("ForegroundMask.jpg");

            using (var backgroundSource = new StorageFileImageSource(backgroundStorageFile))
            using (var foregroundImageSource = new StorageFileImageSource(foregroundStorageFile))
            using (var foregroundMaskSource = new StorageFileImageSource(maskStorageFile))
            using (var filterEffect = new FilterEffect(backgroundSource))
            using (var blendFilter = new BlendFilter(foregroundImageSource))
            {
                ImageProviderInfo result = await backgroundSource.GetInfoAsync();
                var bitmap = new WriteableBitmap((int)result.ImageSize.Width, (int)result.ImageSize.Height);

                using (var renderer = new WriteableBitmapRenderer(filterEffect, bitmap))
                {
                    blendFilter.MaskSource = foregroundMaskSource;
                    blendFilter.BlendFunction = BlendFunction.Normal;
                    //blendFilter.TargetArea = new Rect();
                    //blendFilter.TargetOutputOption = OutputOption.PreserveAspectRatio;
                    //blendFilter.TargetAreaRotation = 


                    filterEffect.Filters = new IFilter[] { blendFilter };

                    WriteableBitmap resultBuffer = await renderer.RenderAsync();
                    imageIO.SaveFile(resultBuffer, "result.jpg");
                    OriginalImageSource = Load("result.jpg");
                }
            }
        }

        public void ImageSaving(ImageSavingEventArgs e)
        {
            string fileName = string.Format("Fakeholiday_{0}", DateTime.Now.ToString("yyMMddhhmmss"));
            e.FileName = fileName;
            RadMessageBox.ShowAsync(string.Format("The image {0} is saved in your media library.", fileName), "Saved");
        }

        public BitmapImage Load(string fileName)
        {
            byte[] data;

            using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (IsolatedStorageFileStream isfs = isf.OpenFile(fileName, FileMode.Open, FileAccess.Read))
                {
                    // Allocate an array large enough for the entire file 
                    data = new byte[isfs.Length];
                    // Read the entire file and then close it 
                    isfs.Read(data, 0, data.Length);
                    isfs.Close();
                }
            }

            var ms = new MemoryStream(data);
            var bi = new BitmapImage();
            bi.SetSource(ms);
            return bi;
        }

        private string pinButtonText;

        public string PinButtonText
        {
            get { return pinButtonText; }
            set
            {
                pinButtonText = value;
                NotifyOfPropertyChange(() => PinButtonText);
            }
        }

        private Uri pinIcon;

        public Uri PinIcon
        {
            get { return pinIcon; }
            set
            {
                pinIcon = value;
                NotifyOfPropertyChange(() => PinIcon);
            }
        }

        ImageSource originalImageSource;

        public ImageSource OriginalImageSource
        {
            get { return originalImageSource; }
            set
            {
                originalImageSource = value;
                NotifyOfPropertyChange(() => OriginalImageSource);
            }
        }

    }
}
