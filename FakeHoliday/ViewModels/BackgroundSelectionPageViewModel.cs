using System;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.IsolatedStorage;
using System.Windows;
using System.Windows.Media.Imaging;

using Caliburn.Micro;

using FakeHoliday.Common;
using FakeHoliday.Resources;

using Microsoft.Phone.Tasks;

using Telerik.Windows.Controls;

namespace FakeHoliday.ViewModels
{
    public class BackgroundSelectionPageViewModel : FakeHolidayViewModel
    {
        private readonly PhotoChooserTask photoChooserTask = new PhotoChooserTask();
        private readonly ImageIO imageIO;

        public BackgroundSelectionPageViewModel(BackgroundImageBrush backgroundImageBrush, INavigationService navigationService, ILog logger, ImageIO imageIO)
            : base(backgroundImageBrush, navigationService, logger)
        {
            this.imageIO = imageIO;
            photoChooserTask.ShowCamera = true;
            photoChooserTask.Completed += photoChooserTask_Completed;
            backgrounds = new ObservableCollection<Background>();
            FillBackgrounds();
        }

        Stream original;

        BitmapImage originalBitmap;

        void photoChooserTask_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult == Microsoft.Phone.Tasks.TaskResult.OK)
            {
                original = e.ChosenPhoto;

                originalBitmap = new BitmapImage
                {
                    DecodePixelWidth = (int)(480.0 * Application.Current.Host.Content.ScaleFactor / 100.0)
                };

                original.Position = 0;
                originalBitmap.SetSource(original);

                imageIO.SaveFile(originalBitmap, "BackgroundTemp.jpg");
                cameraRollSelected = true;
            }
        }

        bool cameraRollSelected;

        protected override void OnActivate()
        {
            base.OnActivate();
            if (cameraRollSelected)
            {
                Navigate();
                cameraRollSelected = false;
            }
        }

        public void Navigate()
        {
            var uri = navigationService.UriFor<BlendPageViewModel>().BuildUri();
            navigationService.Navigate(uri);            
        }

        public string PageTitle
        {
            get { return AppResources.BackgroundSelectionPageTitle; }
        }

        public void SelectBackground()
        {
            photoChooserTask.Show();
        }

        public void Tapped(ListBoxItemTapEventArgs eventArgs)
        {
            if (eventArgs.Item.Content != null)
            {
                var selectedBackground = eventArgs.Item.Content as Background;
                if (selectedBackground != null)
                {
                    SaveFileToIsolatedStorage(new Uri(selectedBackground.ImageUrl.Substring(1), UriKind.Relative), "BackgroundTemp.jpg");
                    Navigate();
                }
            }
        }

        private ObservableCollection<Background> backgrounds;

        public ObservableCollection<Background> Backgrounds
        {
            get { return backgrounds; }
            set
            {
                backgrounds = value;
                NotifyOfPropertyChange(() => Backgrounds);
            }
        }

        private void FillBackgrounds()
        {
            backgrounds.Add(new Background { Name = "Italy Florence", ImageUrl = "/Assets/Backgrounds/Italy_Florence.jpg" });
            backgrounds.Add(new Background { Name = "Barcelona", ImageUrl = "/Assets/Backgrounds/Barcelona.jpg" });
            backgrounds.Add(new Background { Name = "Canada", ImageUrl = "/Assets/Backgrounds/Canada_.jpg" });
            backgrounds.Add(new Background { Name = "Canada Lake", ImageUrl = "/Assets/Backgrounds/Canada2.jpg" });
            backgrounds.Add(new Background { Name = "Canada Tofino", ImageUrl = "/Assets/Backgrounds/Canada_Tofino.jpg" });
            backgrounds.Add(new Background { Name = "Holland", ImageUrl = "/Assets/Backgrounds/Holland_Tulips.jpg" });
            backgrounds.Add(new Background { Name = "Italy Tuscany", ImageUrl = "/Assets/Backgrounds/Italy_Tuscany.jpg" });
            backgrounds.Add(new Background { Name = "Italy Tuscany", ImageUrl = "/Assets/Backgrounds/Italy_Tuscany2.jpg" });
            backgrounds.Add(new Background { Name = "London", ImageUrl = "/Assets/Backgrounds/London.jpg" });
            backgrounds.Add(new Background { Name = "Portugal", ImageUrl = "/Assets/Backgrounds/Portugal.jpg" });
            backgrounds.Add(new Background { Name = "USA", ImageUrl = "/Assets/Backgrounds/USA.jpg" });
            NotifyOfPropertyChange(() => Backgrounds);
        }

        private static void SaveFileToIsolatedStorage(Uri fileUri, string _fileName)
        {
            var AppIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication();
            var FileName = AppIsolatedStorage.CreateFile(_fileName);
            var FileData = Application.GetResourceStream(fileUri);
            var bytes = new byte[4096];
            int Count;
            while ((Count = FileData.Stream.Read(bytes, 0, 4096)) > 0)
            {
                FileName.Write(bytes, 0, Count);
            }
            FileName.Close();
        }

    }
}