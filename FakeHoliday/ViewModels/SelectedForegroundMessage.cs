using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

namespace FakeHoliday.ViewModels
{
    public class SelectedForegroundMessage
    {
        public WriteableBitmap ForegroundSelectionMask { get; set; }

        public Stream PictureWithForegound { get; set; }
    }
}
