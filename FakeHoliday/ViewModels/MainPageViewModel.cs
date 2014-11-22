using System;
using System.Windows.Media;
using Caliburn.Micro;
using FakeHoliday.Resources;
using Microsoft.Phone.Tasks;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using Nokia.Graphics.Imaging;
using Nokia.InteropServices.WindowsRuntime;

using FakeHoliday.Views;
using FakeHoliday.Common;

/// http://developer.nokia.com/resources/library/Lumia/nokia-imaging-sdk/core-concepts/effects.html

namespace FakeHoliday.ViewModels
{
    public class MainPageViewModel : FakeHolidayViewModel
    {
        private readonly PhotoChooserTask photoChooserTask = new PhotoChooserTask();

        private readonly SolidColorBrush foregroundBrush = new SolidColorBrush(Colors.Red);
        private readonly SolidColorBrush backgroundBrush = new SolidColorBrush(Colors.Green);

        private readonly ImageIO imageIO;

        private readonly IMessageStorage messageStorage;

        public MainPageViewModel(BackgroundImageBrush backgroundImageBrush, INavigationService navigationService, ILog logger, IMessageStorage messageStorage, ImageIO imageIO)
            : base(backgroundImageBrush, navigationService, logger)
        {
            ChooseIcon = new Uri("Assets/Pictures.png", UriKind.Relative);
            SegmentIcon = new Uri("Assets/photo.fix.png", UriKind.Relative);
            SegmentFinishedIcon = new Uri("Assets/photo.fix.undo.png", UriKind.Relative);
            ClearLinesIcon = new Uri("Assets/delete.png", UriKind.Relative);
            UndoIcon = new Uri("Assets/Undo.png", UriKind.Relative);

            StartButtonText = "start";
            AboutButtonText = "about";
            PrivacyButtonText = "privacy";
            SegmentButtonText = "select";
            ClearLinesButtonText = "clear";
            SegmentFinishedButtonText = "next";
            UndoButtonText = "undo";
            HelpButtonText = "help";

            photoChooserTask.ShowCamera = true;
            photoChooserTask.Completed += photoChooserTask_Completed;

            manipulationAreaMargin.Left = -75;
            ForegroundButtonPressed = true;
            this.messageStorage = messageStorage;
            this.imageIO = imageIO;
            IsSelectionButtonsVisible = false;
            IsFirstInstructionVisible = true;
        }

        MainPage connectedView;

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            connectedView = view as MainPage;
        }

        Stream original;

        void photoChooserTask_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult == Microsoft.Phone.Tasks.TaskResult.OK)
            {
                original = e.ChosenPhoto;

                var originalBitmap = new BitmapImage
                {
                    DecodePixelWidth = (int)(480.0 * Application.Current.Host.Content.ScaleFactor / 100.0)
                };

                original.Position = 0;
                originalBitmap.SetSource(original);
                OriginalImageSource = originalBitmap;

                imageIO.SaveFile(originalBitmap, "ForegroundTemp.jpg");
                IsSegmentEnabled = true;
                IsSelectionButtonsVisible = true;
                IsFirstInstructionVisible = false;
                IsUndoEnabled = true;
            }
        }

        public void LayoutUpdated()
        {
            AnnotationCanvasWidth = actualWidth;
            AnnotationCanvasHeight = actualHeight;
        }

        double actualWidth;
        double actualHeight;

        public void OriginalImageSizeChanged(SizeChangedEventArgs e)
        {
            actualWidth = e.NewSize.Width;
            actualHeight = e.NewSize.Height;
        }

        Polyline polyline;

        SolidColorBrush brush;

        public void StartManipulation(ManipulationStartedEventArgs e)
        {
            polyline = new Polyline
            {
                Stroke = brush,
                StrokeThickness = 10
            };

            var manipulationAreaDeltaX = ManipulationAreaMargin.Left;
            var manipulationAreaDeltaY = ManipulationAreaMargin.Top;

            var point = NearestPointInElement(
                e.ManipulationOrigin.X + manipulationAreaDeltaX,
                e.ManipulationOrigin.Y + manipulationAreaDeltaY, AnnotationCanvasWidth, AnnotationCanvasHeight
                );

            polyline.Points.Add(point);

            connectedView.CurrentAnnotationCanvas.Children.Add(polyline);
        }

        public void Undo()
        {
            if (connectedView.SegmenterAnnotationsCanvas.Children.Count > 0)
            {
                connectedView.SegmenterAnnotationsCanvas.Children.RemoveAt(connectedView.SegmenterAnnotationsCanvas.Children.Count - 1);
            }
        }

        public void EndManipulation(ManipulationStartedEventArgs e)
        {
            connectedView.CurrentAnnotationCanvas.Children.RemoveAt(connectedView.CurrentAnnotationCanvas.Children.Count - 1);

            connectedView.SegmenterAnnotationsCanvas.Children.Add(polyline);

            polyline = null;
        }

        public void ManipulationDelta(ManipulationDeltaEventArgs e)
        {
            var manipulationAreaDeltaX = ManipulationAreaMargin.Left;
            var manipulationAreaDeltaY = ManipulationAreaMargin.Top;

            var x = e.ManipulationOrigin.X - e.DeltaManipulation.Translation.X + manipulationAreaDeltaX;
            var y = e.ManipulationOrigin.Y - e.DeltaManipulation.Translation.Y + manipulationAreaDeltaY;

            var point = NearestPointInElement(x, y, AnnotationCanvasWidth, AnnotationCanvasHeight);

            polyline.Points.Add(point);
        }

        private WriteableBitmap maskBitmap;

        public async void Segment()
        {
            if (connectedView.SegmenterAnnotationsCanvas.Children.Count > 0)
            {

                maskBitmap = new WriteableBitmap((int)connectedView.SegmenterAnnotationsCanvas.ActualWidth,
                    (int)connectedView.SegmenterAnnotationsCanvas.ActualHeight);
                var annotationsBitmap = new WriteableBitmap((int)connectedView.SegmenterAnnotationsCanvas.ActualWidth,
                    (int)connectedView.SegmenterAnnotationsCanvas.ActualHeight);

                annotationsBitmap.Render(connectedView.SegmenterAnnotationsCanvas, new ScaleTransform
                {
                    ScaleX = 1,
                    ScaleY = 1
                });

                annotationsBitmap.Invalidate();

                original.Position = 0;

                using (var source = new StreamImageSource(original))
                using (var segmenter = new InteractiveForegroundSegmenter(source))
                using (var renderer = new WriteableBitmapRenderer(segmenter, maskBitmap))
                using (var annotationsSource = new BitmapImageSource(annotationsBitmap.AsBitmap()))
                {
                    var foregroundColor = foregroundBrush.Color;
                    var backgroundColor = backgroundBrush.Color;

                    segmenter.ForegroundColor = Windows.UI.Color.FromArgb(foregroundColor.A, foregroundColor.R, foregroundColor.G,
                        foregroundColor.B);
                    segmenter.BackgroundColor = Windows.UI.Color.FromArgb(backgroundColor.A, backgroundColor.R, backgroundColor.G,
                        backgroundColor.B);
                    segmenter.Quality = 0.5;
                    segmenter.AnnotationsSource = annotationsSource;

                    IsBusy = true;

                    await renderer.RenderAsync();

                    imageIO.SaveFile(maskBitmap, "ForegroundMask.jpg");

                    connectedView.MaskImage.Source = maskBitmap;

                    IsBusy = false;
                    IsSegmentFinishedEnabled = true;
                }
            }
        }

        public void SegmentFinished()
        {
            var message = new SelectedForegroundMessage();
            message.ForegroundSelectionMask = maskBitmap;
            message.PictureWithForegound = original;

            messageStorage.Store("msg", message);
            var uri = navigationService.UriFor<BackgroundSelectionPageViewModel>().BuildUri();
            navigationService.Navigate(uri);
        }

        public void ClearLines()
        {
            connectedView.SegmenterAnnotationsCanvas.Children.Clear();
            connectedView.MaskImage.Source = null;
            OriginalImageSource = null;
            IsSelectionButtonsVisible = false;
            IsFirstInstructionVisible = true;
            IsUndoEnabled = false;
            IsSegmentFinishedEnabled = false;
            IsSegmentEnabled = false;
        }

        Thickness manipulationAreaMargin;

        public Thickness ManipulationAreaMargin
        {
            get { return manipulationAreaMargin; }
            set
            {
                manipulationAreaMargin = value;
                NotifyOfPropertyChange(() => ManipulationAreaMargin);
            }
        }

        double annotationCanvasHeight;

        public double AnnotationCanvasHeight
        {
            get { return annotationCanvasHeight; }
            set
            {
                annotationCanvasHeight = value;
                NotifyOfPropertyChange(() => AnnotationCanvasHeight);
            }
        }

        double annotationCanvasWidth;

        public double AnnotationCanvasWidth
        {
            get { return annotationCanvasWidth; }
            set
            {
                annotationCanvasWidth = value;
                NotifyOfPropertyChange(() => AnnotationCanvasWidth);
            }
        }

        private Point NearestPointInElement(double x, double y, double width, double height)
        {
            var clampedX = Math.Min(Math.Max(0, x), width);
            var clampedY = Math.Min(Math.Max(0, y), height);

            return new Point(clampedX, clampedY);
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

        public string PageTitle
        {
            get { return AppResources.MainPageTitle; }
        }

        public void Privacy()
        {
            var uri = navigationService.UriFor<PrivacyPageViewModel>().BuildUri();
            navigationService.Navigate(uri);
        }

        public void About()
        {
            navigationService.Navigate(new Uri("/YourLastAboutDialog;component/AboutPage.xaml", UriKind.Relative));
        }

        public void Choose()
        {
            photoChooserTask.Show();
        }

        public void Help()
        {
            var uri = navigationService.UriFor<HelpPageViewModel>().BuildUri();
            navigationService.Navigate(uri);
        }

        private bool isSegmentEnabled;

        public bool IsSegmentEnabled
        {
            get { return isSegmentEnabled; }
            set
            {
                isSegmentEnabled = value;
                NotifyOfPropertyChange(() => IsSegmentEnabled);
            }
        }

        private bool isSegmentFinishedEnabled;

        public bool IsSegmentFinishedEnabled
        {
            get { return isSegmentFinishedEnabled; }
            set
            {
                isSegmentFinishedEnabled = value;
                NotifyOfPropertyChange(() => IsSegmentFinishedEnabled);
            }
        }

        private bool isUndoEnabled;

        public bool IsUndoEnabled
        {
            get { return isUndoEnabled; }
            set
            {
                isUndoEnabled = value;
                NotifyOfPropertyChange(() => IsUndoEnabled);
            }
        }

        private bool isSelectionButtonsVisible;

        public bool IsSelectionButtonsVisible
        {
            get { return isSelectionButtonsVisible; }
            set
            {
                isSelectionButtonsVisible = value;
                NotifyOfPropertyChange(() => IsSelectionButtonsVisible);
            }
        }

        private bool isFirstInstructionVisible;

        public bool IsFirstInstructionVisible
        {
            get { return isFirstInstructionVisible; }
            set
            {
                isFirstInstructionVisible = value;
                NotifyOfPropertyChange(() => IsFirstInstructionVisible);
            }
        }

        private string segmentButtonText;

        public string SegmentButtonText
        {
            get { return segmentButtonText; }
            set
            {
                segmentButtonText = value;
                NotifyOfPropertyChange(() => SegmentButtonText);
            }
        }

        private string segmentFinishedButtonText;

        public string SegmentFinishedButtonText
        {
            get { return segmentFinishedButtonText; }
            set
            {
                segmentFinishedButtonText = value;
                NotifyOfPropertyChange(() => SegmentFinishedButtonText);
            }
        }


        private string aboutButtonText;

        public string AboutButtonText
        {
            get { return aboutButtonText; }
            set
            {
                aboutButtonText = value;
                NotifyOfPropertyChange(() => AboutButtonText);
            }
        }

        private string instruction;

        public string Instruction
        {
            get { return instruction; }
            set
            {
                instruction = value;
                NotifyOfPropertyChange(() => Instruction);
            }
        }

        private string privacyButtonText;

        public string PrivacyButtonText
        {
            get { return privacyButtonText; }
            set
            {
                privacyButtonText = value;
                NotifyOfPropertyChange(() => PrivacyButtonText);
            }
        }

        private string startButtonText;

        public string StartButtonText
        {
            get { return startButtonText; }
            set
            {
                startButtonText = value;
                NotifyOfPropertyChange(() => StartButtonText);
            }
        }

        private string undoButtonText;

        public string UndoButtonText
        {
            get { return undoButtonText; }
            set
            {
                undoButtonText = value;
                NotifyOfPropertyChange(() => UndoButtonText);
            }
        }

        private string clearLinesButtonText;

        public string ClearLinesButtonText
        {
            get { return clearLinesButtonText; }
            set
            {
                clearLinesButtonText = value;
                NotifyOfPropertyChange(() => ClearLinesButtonText);
            }
        }

        private string helpButtonText;

        public string HelpButtonText
        {
            get { return helpButtonText; }
            set
            {
                helpButtonText = value;
                NotifyOfPropertyChange(() => HelpButtonText);
            }
        }
        

        private Uri chooseIcon;

        public Uri ChooseIcon
        {
            get { return chooseIcon; }
            set
            {
                chooseIcon = value;
                NotifyOfPropertyChange(() => ChooseIcon);
            }
        }

        private Uri segmentIcon;

        public Uri SegmentIcon
        {
            get { return segmentIcon; }
            set
            {
                segmentIcon = value;
                NotifyOfPropertyChange(() => SegmentIcon);
            }
        }

        private Uri segmentFinishedIcon;

        public Uri SegmentFinishedIcon
        {
            get { return segmentFinishedIcon; }
            set
            {
                segmentFinishedIcon = value;
                NotifyOfPropertyChange(() => SegmentFinishedIcon);
            }
        }

        private Uri clearLinesIcon;

        public Uri ClearLinesIcon
        {
            get { return clearLinesIcon; }
            set
            {
                clearLinesIcon = value;
                NotifyOfPropertyChange(() => ClearLinesIcon);
            }
        }

        private Uri undoIcon;

        public Uri UndoIcon
        {
            get { return undoIcon; }
            set
            {
                undoIcon = value;
                NotifyOfPropertyChange(() => UndoIcon);
            }
        }
 
        private bool foregroundButtonPressed = true;
        private bool settingBusy;

        public bool ForegroundButtonPressed
        {
            get { return foregroundButtonPressed; }
            set
            {
                foregroundButtonPressed = value;
                if (!settingBusy)
                {
                    settingBusy = true;
                    BackgroundButtonPressed = !value;
                }
                if (value)
                {
                    brush = foregroundBrush;
                    Instruction = "select the foreground";
                }
                settingBusy = false;
                NotifyOfPropertyChange(() => ForegroundButtonPressed);
            }
        }

        private bool backgroundButtonPressed;

        public bool BackgroundButtonPressed
        {
            get { return backgroundButtonPressed; }
            set
            {
                if (!settingBusy)
                {
                    settingBusy = true;
                    ForegroundButtonPressed = !value;
                }
                settingBusy = false;
                backgroundButtonPressed = value;
                if (value)
                {
                    brush = backgroundBrush;
                    Instruction = "select the background";
                }
                NotifyOfPropertyChange(() => BackgroundButtonPressed);
            }
        }
    }
}