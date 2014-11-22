using Caliburn.Micro;

using FakeHoliday.Resources;

namespace FakeHoliday.ViewModels
{
    public class PrivacyPageViewModel : FakeHolidayViewModel
    {
        public PrivacyPageViewModel(BackgroundImageBrush backgroundImageBrush, INavigationService navigationService, ILog logger)
            : base(backgroundImageBrush, navigationService, logger)
        {
        }

        public string PageTitle
        {
            get { return AppResources.PrivacyPageTitle; }
        }
    }
}