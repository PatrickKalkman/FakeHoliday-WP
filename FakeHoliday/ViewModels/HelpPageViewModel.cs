using Caliburn.Micro;

using FakeHoliday.Resources;

namespace FakeHoliday.ViewModels
{
    public class HelpPageViewModel : FakeHolidayViewModel
    {
        public HelpPageViewModel(BackgroundImageBrush backgroundImageBrush, INavigationService navigationService, ILog logger)
            : base(backgroundImageBrush, navigationService, logger)
        {
        }

        public string PageTitle
        {
            get { return AppResources.HelpPageTitle; }
        }
    }
}
