using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.Windows;

namespace EasyAppointment
{
    class Helper
    {
        //Block Memory Leak
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr handle);
        public static BitmapSource bs;
        public static IntPtr ip;
        public static BitmapSource LoadBitmap(System.Drawing.Bitmap source)
        {

            ip = source.GetHbitmap();

            bs = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(ip, IntPtr.Zero, System.Windows.Int32Rect.Empty,

                System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());

            DeleteObject(ip);

            return bs;

        }
        public static byte[] SaveImageCapture(BitmapSource bitmap)
        {
            MemoryStream memStream = new MemoryStream();
            try
            {
                JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmap));
                encoder.QualityLevel = 100;
                
                

                encoder.Save(memStream);
            }
            catch (ArgumentNullException ex)
            {
                MessageBox.Show("Error capturing photo. " + ex.Message,
                    "EasyAppointment", MessageBoxButton.OK, MessageBoxImage.Error);
            }
                return memStream.ToArray();


        }
    }
}
