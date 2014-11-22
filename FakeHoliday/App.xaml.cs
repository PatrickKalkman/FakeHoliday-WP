using System;
using System.Diagnostics;
using System.Resources;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Navigation;

using BugSense;
using BugSense.Core.Model;

using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using FakeHoliday.Resources;

namespace FakeHoliday
{
    public partial class App : Application
    {
        /// <summary>
        /// Provides easy access to the root frame of the Phone Application.
        /// </summary>
        /// <returns>The root frame of the Phone Application.</returns>
        public static PhoneApplicationFrame RootFrame { get; private set; }

        /// <summary>
        /// Constructor for the Application object.
        /// </summary>
        public App()
        {
            //BugSenseHandler.Instance.InitAndStartSession(new ExceptionManager(Current), RootFrame, "YourOwn");

            // Standard XAML initialization
            InitializeComponent();
        }
    }
}