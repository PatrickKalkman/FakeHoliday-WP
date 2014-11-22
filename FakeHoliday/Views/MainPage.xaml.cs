using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using FakeHoliday.Resources;
using System.Windows.Shapes;
using System.Windows.Media;

namespace FakeHoliday.Views
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
            GoogleAnalytics.EasyTracker.GetTracker().SendView("MainPage");
        }
    }
}