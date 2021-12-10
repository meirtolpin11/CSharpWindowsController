using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

using System.Windows.Forms;

namespace Meir_Tolpin_Project.Stream
{
    class PicturesHelper
    {
        
        /** Gets screenshot and changes it's quality
         * input:
         *      None
         * output:
         *      Byte array that represents the bitmap image
         */
        public static byte[] getScreenShot()
        {
            //Create a new bitmap.
            var bmpScreenshot = new Bitmap(Screen.PrimaryScreen.Bounds.Width,
                                           Screen.PrimaryScreen.Bounds.Height,
                                           PixelFormat.Format32bppArgb);
            bmpScreenshot = ScreenShotHelper.CaptureScreen(true);

            return changeQuality(20, bmpScreenshot);
        }


        /**
         * changing the bitmap image quality
         * input:
         *      int q: the quality of the image in %. example - if q = 20, 
         *          the quality will be 0.2 from the original 
         *      Bitmap bmp: the image to modify
         * output:
         *      Byte array that represents the bimap img with new resolution 
         */
        private static Byte[] changeQuality(int q, Bitmap bmp)
        {
            ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);

            // Create an Encoder object based on the GUID
            // for the Quality parameter category.
            System.Drawing.Imaging.Encoder myEncoder =
                System.Drawing.Imaging.Encoder.Quality;

            // Create an EncoderParameters object.
            // An EncoderParameters object has an array of EncoderParameter
            // objects. In this case, there is only one
            // EncoderParameter object in the array.
            EncoderParameters myEncoderParameters = new EncoderParameters(1);

            EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, q);
            myEncoderParameters.Param[0] = myEncoderParameter;
            byte[] byteArray = new byte[0];
            using (MemoryStream stream = new MemoryStream())
            {
                bmp.Save(stream, jpgEncoder, myEncoderParameters);
                stream.Close();

                byteArray = stream.ToArray();
            }
            return byteArray;
        }


        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }


    }
}
