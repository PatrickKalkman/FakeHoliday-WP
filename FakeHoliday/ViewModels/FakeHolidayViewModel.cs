using Caliburn.Micro;
using FakeHoliday.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace FakeHoliday.ViewModels
{
    public class FakeHolidayViewModel : Screen
    {
        protected readonly BackgroundImageBrush backgroundImageBrush;
        protected readonly INavigationService navigationService;
        protected readonly ILog logger;

        public FakeHolidayViewModel(BackgroundImageBrush backgroundImageBrush, INavigationService navigationService, ILog logger)
        {
            this.backgroundImageBrush = backgroundImageBrush;
            this.navigationService = navigationService;
            this.logger = logger;
        }

        public ImageBrush BackgroundImageBrush
        {
            get { return backgroundImageBrush.GetBackground(); }
        }

        public string ApplicationName
        {
            get { return AppResources.ApplicationTitle; }
        }

        private bool isBusy = false;

        public bool IsBusy
        {
            get
            {
                return isBusy;
            }
            set
            {
                isBusy = value;
                NotifyOfPropertyChange(() => IsBusy);
            }
        }

    }
}
