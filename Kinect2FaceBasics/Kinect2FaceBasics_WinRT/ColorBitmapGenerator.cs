using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;
using WindowsPreview.Kinect;

namespace Kinect2FaceBasics_WinRT
{
    /// <summary>
    /// Creates the bitmap representation of a Kinect color frame.
    /// </summary>
    public class ColorBitmapGenerator
    {
        #region Properties

        /// <summary>
        /// Returns the RGB pixel values.
        /// </summary>
        byte[] _pixels;

        /// <summary>
        /// Returns the width of the bitmap.
        /// </summary>
        int _width;

        /// <summary>
        /// Returns the height of the bitmap.
        /// </summary>
        int _height;

        /// <summary>
        /// Returns the stream of the bitmap.
        /// </summary>
        Stream _stream;

        #endregion

        #region Properties

        /// <summary>
        /// Returns the actual bitmap.
        /// </summary>
        public WriteableBitmap Bitmap { get; protected set; }

        #endregion

        #region Methods

        /// <summary>
        /// Updates the bitmap with new frame data.
        /// </summary>
        /// <param name="frame">The specified Kinect color frame.</param>
        /// <param name="format">The specified color format.</param>
        public void Update(ColorFrame frame, ColorImageFormat format)
        {
            if (Bitmap == null)
            {
                _width = frame.FrameDescription.Width;
                _height = frame.FrameDescription.Height;
                _pixels = new byte[_width * _height * 4];
                Bitmap = new WriteableBitmap(_width, _height);
                _stream = Bitmap.PixelBuffer.AsStream();
            }

            if (frame.RawColorImageFormat == ColorImageFormat.Bgra)
            {
                frame.CopyRawFrameDataToArray(_pixels);
            }
            else
            {
                frame.CopyConvertedFrameDataToArray(_pixels, format);
            }

            _stream.Seek(0, SeekOrigin.Begin);
            _stream.Write(_pixels, 0, _pixels.Length);

            Bitmap.Invalidate();
        }

        /// <summary>
        /// Updates the bitmap with new frame data.
        /// </summary>
        /// <param name="frame">The specified Kinect color frame.</param>
        public void Update(ColorFrame frame)
        {
            Update(frame, ColorImageFormat.Bgra);
        }

        #endregion
    }

    /// <summary>
    /// Provides some common functionality for manipulating color frames.
    /// </summary>
    public static class BitmapExtensions
    {
        #region Members

        /// <summary>
        /// The color bitmap creator.
        /// </summary>
        static ColorBitmapGenerator _colorBitmapGenerator = new ColorBitmapGenerator();

        #endregion

        #region Public methods

        /// <summary>
        /// Converts the specified color frame to a bitmap image.
        /// </summary>
        /// <param name="frame">The specified color frame.</param>
        /// <returns>The bitmap representation of the color frame.</returns>
        public static WriteableBitmap ToBitmap(this ColorFrame frame)
        {
            _colorBitmapGenerator.Update(frame);

            return _colorBitmapGenerator.Bitmap;
        }

        #endregion
    }
}
