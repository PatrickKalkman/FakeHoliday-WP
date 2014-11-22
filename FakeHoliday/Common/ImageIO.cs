using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Windows.Storage;

namespace FakeHoliday.Common
{
    public class ImageIO
    {
        public void SaveFile(BitmapImage bitmapImage, string fileName)
        {
            var bmp = new WriteableBitmap(bitmapImage);
            SaveFile(bmp, fileName);
        }

        public void SaveFile(WriteableBitmap bitmapImage, string fileName)
        {
            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (IsolatedStorageFileStream stream = storage.CreateFile(fileName))
                {
                    bitmapImage.SaveJpeg(stream, bitmapImage.PixelWidth, bitmapImage.PixelHeight, 0, 95);
                    stream.Close();
                }
            }
        }
    }
}
