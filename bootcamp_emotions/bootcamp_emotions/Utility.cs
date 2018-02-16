using OpenCVWrappers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Bootcamp.CompVis.Emotions
{
    public static class Utility
    {
        /// <summary>
        /// Convert a RawImage instance to a bitmap.
        /// </summary>
        /// <param name="raw">The RawImage instance to convert.</param>
        /// <returns>The raw image converted to a Bitmap instance.</returns>
        public static Bitmap ToBitmap(this RawImage raw)
        {
            // first create a writeable bitmap
            var wbm = raw.CreateWriteableBitmap();
            raw.UpdateWriteableBitmap(wbm);

            // write the writeablebitmap into a memory stream
            using (var stream = new MemoryStream())
            {
                var encoder = new BmpBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create((BitmapSource)wbm));
                encoder.Save(stream);

                // construct a new bitmap from the memorystream data
                return new Bitmap(stream) as Bitmap;
            }
        }
    }
}
