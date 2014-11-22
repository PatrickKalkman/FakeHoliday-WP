using System;
using System.Collections.Generic;

using Caliburn.Micro;
using Microsoft.Phone.Controls;
using Caliburn.Micro.BindableAppBar;
using System.Windows.Controls;
using FakeHoliday.ViewModels;
using FakeHoliday.Common;

namespace FakeHoliday
{
    public class Bootstrapper : PhoneBootstrapper
    {
        private PhoneContainer container;
        
        public Bootstrapper()
        {
            LogManager.GetLog = type => new DebugLogger(type);
        }

        protected override PhoneApplicationFrame CreatePhoneApplicationFrame()
        {
            return new TransitionFrame();
        }

        protected override void Configure()
        {
            container = new PhoneContainer();

            container.RegisterPhoneServices(RootFrame);
            container.PerRequest<MainPageViewModel>();
            container.PerRequest<BlendPageViewModel>();
            container.PerRequest<PrivacyPageViewModel>();
            container.PerRequest<BackgroundImageBrush>();
            container.PerRequest<HelpPageViewModel>();
            container.PerRequest<ImageIO>();
            container.PerRequest<BackgroundSelectionPageViewModel>();
            container.PerRequest<FlipTileCreator>();
            container.RegisterSingleton(typeof(ILog), "", typeof(DebugLogger));
            container.RegisterSingleton(typeof(IMessageStorage), "", typeof(MessageStore));

            AddCustomConventions();
        }

        static void AddCustomConventions()
        {
            ConventionManager.AddElementConvention<BindableAppBarButton>(
                Control.IsEnabledProperty, "DataContext", "Click");
            ConventionManager.AddElementConvention<BindableAppBarMenuItem>(
                Control.IsEnabledProperty, "DataContext", "Click");
        }

        protected override void OnActivate(object sender, Microsoft.Phone.Shell.ActivatedEventArgs e)
        {
            GoogleAnalytics.EasyTracker.GetTracker().SendEvent("Bootstrapper", "Activate", null, 0);
            base.OnActivate(sender, e);
        }

        protected override void OnLaunch(object sender, Microsoft.Phone.Shell.LaunchingEventArgs e)
        {
            GoogleAnalytics.EasyTracker.GetTracker().SendEvent("Bootstrapper", "Launch", null, 0);
            base.OnLaunch(sender, e);
        }

        protected override void OnDeactivate(object sender, Microsoft.Phone.Shell.DeactivatedEventArgs e)
        {
            GoogleAnalytics.EasyTracker.GetTracker().SendEvent("Bootstrapper", "Deactivate", null, 0);
            base.OnDeactivate(sender, e);
        }

        protected override void OnClose(object sender, Microsoft.Phone.Shell.ClosingEventArgs e)
        {
            GoogleAnalytics.EasyTracker.GetTracker().SendEvent("Bootstrapper", "Close", null, 0);
            base.OnClose(sender, e);
        }

        protected override object GetInstance(Type service, string key)
        {
            return container.GetInstance(service, key);
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return container.GetAllInstances(service);
        }

        protected override void BuildUp(object instance)
        {
            container.BuildUp(instance);
        }
    }
}