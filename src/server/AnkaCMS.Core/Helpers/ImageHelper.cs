using AnkaCMS.Core.Enums;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace AnkaCMS.Core.Helpers
{
    public static class ImageHelper
    {

        #region GlobalDeclerations

        private static int _oldSize;
        private static int _newHeight;
        private static int _newWidth;
        private static Bitmap _newBitmap;
        private static Graphics _graphics;
        private static int _leftCoordinate;
        private static int _topCoordinate;
        private static float _rate;
        private static Stream _stream;
        private static Font _font;
        private static HatchBrush _brush;

        #endregion

        #region Resize

        /// <summary>
        /// Gönderilen bir Imaj dosyasını istenilen kalitede; maksimum yükseklik veya maksimum genişliğe göre kaydeder.
        /// </summary>
        /// <param name="image">Image olarak gönderilen dosyayı ifade eder.</param>
        /// <param name="maxWidth">Maksimum olabilecek genişliği ifade eder.</param>
        /// <param name="maxHeight">Maksimum olabilecek yüksekliği ifade eder.</param>
        /// <param name="quality">Image dosyasının yüzde olarak hangi kalitede kaydedileciğini ifade eder.</param>
        /// <returns>Image</returns>
        public static Image ResizeMax(Image image, int maxWidth, int maxHeight, int quality)
        {

            // Get the image's original width and height
            var originalWidth = image.Width;
            var originalHeight = image.Height;

            // To preserve the aspect ratio
            var ratioX = maxWidth / (float)originalWidth;
            var ratioY = maxHeight / (float)originalHeight;
            var ratio = Math.Min(ratioX, ratioY);

            // New width and height based on aspect ratio
            var newWidth = (int)(originalWidth * ratioY);
            var newHeight = (int)(originalHeight * ratioY);

            if (newWidth > maxWidth)
            {
                newWidth = (int)(originalWidth * ratio);
                newHeight = (int)(originalHeight * ratio);
            }

            // Convert other formats (including CMYK) to RGB.
            var newImage = new Bitmap(newWidth, newHeight, PixelFormat.Format24bppRgb);

            // Draws the image in the specified size with quality mode set to HighQuality
            using (var graphics = Graphics.FromImage(newImage))
            {
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.DrawImage(image, 0, 0, newWidth, newHeight);
            }

            // Create an Encoder object for the Quality parameter.
            var encoder = Encoder.Quality;

            // Create an EncoderParameters object. 
            var encoderParameters = new EncoderParameters(1);

            // Save the image as a JPEG file with quality level.
            var encoderParameter = new EncoderParameter(encoder, quality);
            encoderParameters.Param[0] = encoderParameter;

            return newImage;

        }


        /// <summary>
        /// Gönderilen bir Imaj dosyasını istenilen kalitede; minimum yükseklik veya minimum genişliğe göre kaydeder.
        /// </summary>
        /// <param name="image">Image olarak gönderilen dosyayı ifade eder.</param>
        /// <param name="minHeight"></param>
        /// <param name="quality">Image dosyasının yüzde olarak hangi kalitede kaydedileciğini ifade eder.</param>
        /// <param name="minWidth"></param>
        /// <returns>Image</returns>
        public static Image ResizeMin(Image image, int minWidth, int minHeight, int quality)
        {

            // Get the image's original width and height
            var originalWidth = image.Width;
            var originalHeight = image.Height;

            // To preserve the aspect ratio
            var ratioX = minWidth / (float)originalWidth;
            var ratioY = minHeight / (float)originalHeight;
            var ratio = Math.Max(ratioX, ratioY);

            // New width and height based on aspect ratio

            _newWidth = (int)Math.Ceiling((originalWidth * ratio));
            _newHeight = (int)Math.Ceiling((originalHeight * ratio));

            // Convert other formats (including CMYK) to RGB.

            using (var newImage = new Bitmap(_newWidth, _newHeight, PixelFormat.Format24bppRgb))
            {
                // Draws the image in the specified size with quality mode set to HighQuality
                using (var graphics = Graphics.FromImage(newImage))
                {
                    graphics.CompositingQuality = CompositingQuality.HighQuality;
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphics.SmoothingMode = SmoothingMode.HighQuality;
                    graphics.DrawImage(image, 0, 0, _newWidth, _newHeight);
                }

                // Create an Encoder object for the Quality parameter.
                var encoder = Encoder.Quality;

                // Create an EncoderParameters object. 
                var encoderParameters = new EncoderParameters(1);

                // Save the image as a JPEG file with quality level.
                var encoderParameter = new EncoderParameter(encoder, quality);
                encoderParameters.Param[0] = encoderParameter;

                return newImage;
            }





        }

        #endregion

        #region ToByteArray

        /// <summary>
        /// Gönderilen bir resmi, byte dizisine çevirir.
        /// </summary>
        /// <param name="image">Image olarak gönderilen dosyayı ifade eder.</param>
        /// <param name="format">Gönderilen imaj formatını fade eder.</param>
        /// <returns>byte[]</returns>
        public static byte[] ToByteArray(this Image image, ImageFormat format)
        {
            using (var memoryStream = new MemoryStream())
            {
                image.Save(memoryStream, format);
                return memoryStream.ToArray();
            }
        }

        #endregion

        #region GetImageFormat

        /// <summary>
        /// Bir imajın formatını öğrenmek için kullanılır.
        /// </summary>
        /// <param name="image">Image olarak gönderilen dosyayı ifade eder.</param>
        /// <returns>ImageFormat</returns>
        public static ImageFormat GetImageFormat(this Image image)
        {
            if (image.RawFormat.Equals(ImageFormat.Jpeg))
            {
                return ImageFormat.Jpeg;
            }

            if (image.RawFormat.Equals(ImageFormat.Bmp))
            {
                return ImageFormat.Bmp;
            }

            if (image.RawFormat.Equals(ImageFormat.Png))
            {
                return ImageFormat.Png;
            }

            if (image.RawFormat.Equals(ImageFormat.Emf))
            {
                return ImageFormat.Emf;
            }

            if (image.RawFormat.Equals(ImageFormat.Exif))
            {
                return ImageFormat.Exif;
            }

            if (image.RawFormat.Equals(ImageFormat.Gif))
            {
                return ImageFormat.Gif;
            }

            if (image.RawFormat.Equals(ImageFormat.Icon))
            {
                return ImageFormat.Icon;
            }

            if (image.RawFormat.Equals(ImageFormat.MemoryBmp))
            {
                return ImageFormat.MemoryBmp;
            }

            if (image.RawFormat.Equals(ImageFormat.Tiff))
            {
                return ImageFormat.Tiff;
            }

            else
            {
                return ImageFormat.Wmf;
            }

        }

        #endregion

        #region DirectionSet

        /// <summary>
        /// Bitmap dosyasının üzerindeki bir noktayı parametre olarak ifade eder. 
        /// </summary>
        /// <param name="directionOption">Enum olarak yönleri ifade eder.</param>
        /// <param name="bitmap">Bitmap olarak gönderilen dosyayı ifade eder.</param>
        /// <param name="newWidth">Bitmap olarak eklenecek objenin genişliğini ifade eder.</param>
        /// <param name="newHeight">Bitmap olarak eklenecek objenin yüksekliğini ifade eder.</param>
        private static void DirectionSet(DirectionOption directionOption, Bitmap bitmap, int newWidth, int newHeight)
        {
            switch (directionOption)
            {
                case DirectionOption.TopLeft:
                    _leftCoordinate = 0;
                    _topCoordinate = 0;

                    break;

                case DirectionOption.TopCenter:
                    _leftCoordinate = Convert.ToInt32(Math.Round(bitmap.Width / (float)2));
                    _topCoordinate = 0;

                    break;

                case DirectionOption.TopRight:
                    _leftCoordinate = bitmap.Width - newWidth;
                    _topCoordinate = 0;

                    break;

                case DirectionOption.BottomLeft:
                    _leftCoordinate = 0;
                    _topCoordinate = bitmap.Height - newHeight;

                    break;

                case DirectionOption.BottomCenter:
                    _leftCoordinate = Convert.ToInt32(Math.Round(bitmap.Width / (float)2));
                    _topCoordinate = bitmap.Height;

                    break;

                case DirectionOption.BottomRight:
                    _leftCoordinate = bitmap.Width;
                    _topCoordinate = bitmap.Height - newHeight;

                    break;

                case DirectionOption.MiddleLeft:
                    _leftCoordinate = 0;
                    _topCoordinate = Convert.ToInt32(Math.Round(bitmap.Height / (float)2));

                    break;

                case DirectionOption.MiddleRight:
                    _leftCoordinate = bitmap.Width;
                    _topCoordinate = Convert.ToInt32(Math.Round(bitmap.Height / (float)2));

                    break;

                case DirectionOption.Center:
                    _leftCoordinate = Convert.ToInt32(Math.Round((bitmap.Width / (float)2)) - newWidth / (float)2);
                    _topCoordinate = Convert.ToInt32(Math.Round((bitmap.Height / (float)2)) - newHeight / (float)2);

                    break;
                case DirectionOption.Right:
                    break;
                case DirectionOption.Left:
                    break;
                case DirectionOption.Top:
                    break;
                case DirectionOption.Bottom:
                    break;
                case DirectionOption.Middle:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(directionOption), directionOption, null);
            }
        }

        #endregion

        #region AddImageToImage
        /// <summary>
        /// Bir bitmap dosyasının istenen bir yerine, bir başka bitmap dosyasını eklemek için kullanılır.
        /// </summary>
        /// <param name="imageFrom">Ana resmi ifade eder.</param>
        /// <param name="imageTo">Üzerine eklenecek resmi ifade eder</param>
        /// <param name="opacity">Üzerine eklenecek resmin opasitesini ifade eder.</param>
        /// <param name="imageDirection">Resmin nereye ekleneceğini belirtir.</param>
        /// <returns>Bitmap</returns>
        public static Bitmap AddImageToImage(Bitmap imageFrom, Bitmap imageTo, int opacity, DirectionOption imageDirection)
        {
            if (imageFrom == null)
            {
                throw new NullReferenceException("Üzerine resim konacak olan Bitmap dosyası girilmedi.");
            }

            if (imageTo == null)
            {
                throw new NullReferenceException("Bitmap dosyasının üzerine konulacak olan diğer bitmap dosyası girilmedi.");
            }

            if (imageDirection == 0)
            {
                throw new NullReferenceException("Yazının resmin neresine yerleştirileceği belirlenmedi.");
            }

            var pointTo = new Point();

            var opacityValue = opacity / 10.0F;

            float[][] matrix = { new float[] { 1, 0, 0, 0, 0 }, new float[] { 0, 1, 0, 0, 0 }, new float[] { 0, 0, 1, 0, 0 }, new[] { 0, 0, 0, opacityValue, 1 }, new float[] { 0, 0, 0, 0, 1 } };

            var colorMatrix = new ColorMatrix(matrix);
            var imageAttributes = new ImageAttributes();

            imageAttributes.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

            var graphicsFromImage = Graphics.FromImage(imageTo);

            graphicsFromImage.InterpolationMode = InterpolationMode.High;

            DirectionSet(imageDirection, imageTo, imageFrom.Width, imageFrom.Height);

            pointTo.X = _leftCoordinate;
            pointTo.Y = _topCoordinate;

            graphicsFromImage.DrawImage(imageFrom, new Rectangle(pointTo, imageFrom.Size), 0, 0, imageFrom.Width, imageFrom.Height, GraphicsUnit.Pixel, imageAttributes);

            return imageTo;

        }




        #endregion

        #region AddTextToImage

        /// <summary>
        /// Bir bitmap resminin istenilen yerine istenilen bir yazının eklenmesini sağlar.
        /// </summary>
        /// <param name="bitmap">Bitmap olarak gönderilecek resmi ifade eder.</param>
        /// <param name="text">Bitmap resmine eklenecek olan yazıyı ifade eder.</param>
        /// <param name="opacity">Yazının opasitesini ifade eder.</param>
        /// <param name="directionOption">Yazının nereye yerleştirileceğini ifade eder.</param>
        /// <param name="textFont">Yazının fontunu ifade eder.</param>
        /// <param name="automatic">Yazının resme eklenmesi işleminin otomatik yapılmasını ifade eder.</param>
        /// <param name="textColor">Yazının rengini ifade eder.</param>
        /// <param name="textBackColor">Yazının arkasında kullanılacak rengi ifade eder.</param>
        /// <param name="brushStyle">Yazının arka planı için kullanılacak brush'ı ifade eder.</param>
        /// <returns>Bitmap</returns>
        public static Bitmap AddTextToImage(Bitmap bitmap, string text, int opacity, DirectionOption directionOption, Font textFont, bool automatic, Color textColor, Color textBackColor, HatchStyle brushStyle)
        {
            using (_brush = new HatchBrush(brushStyle, textColor, textBackColor))
            {
                var imageFrom = new Bitmap(10, 10);
                var downGraphics = Graphics.FromImage(imageFrom);
                var textSize = new Size
                {
                    Width = (int)downGraphics.MeasureString(text, textFont).Width,
                    Height = (int)downGraphics.MeasureString(text, textFont).Height
                };

                _font = new Font(textFont.FontFamily, textFont.Size);
                if ((textSize.Width > bitmap.Width || textSize.Height > bitmap.Height) && automatic)
                {
                    for (var i = 0; i < 101; i++)
                    {
                        _font = new Font(textFont.FontFamily, textFont.Size * ((float)(100 - i) / 100));
                        textSize.Width = (int)downGraphics.MeasureString(text, _font).Width;
                        textSize.Height = (int)downGraphics.MeasureString(text, _font).Height;
                        if (textSize.Width <= bitmap.Width && textSize.Height <= bitmap.Height)
                        {
                            break;
                        }
                    }
                }

                imageFrom = new Bitmap(textSize.Width, textSize.Height);
                downGraphics = Graphics.FromImage(imageFrom);
                downGraphics.DrawString(text, _font, _brush, new Point(0, 0));

                var opacityValue = opacity / 10.0F;

                float[][] matrix = { new float[] { 1, 0, 0, 0, 0 }, new float[] { 0, 1, 0, 0, 0 }, new float[] { 0, 0, 1, 0, 0 }, new[] { 0, 0, 0, opacityValue, 1 }, new float[] { 0, 0, 0, 0, 1 } };

                var colorMatrix = new ColorMatrix(matrix);
                var imageAttribute = new ImageAttributes();

                imageAttribute.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

                if (text == "")
                {
                    throw new NullReferenceException("Resmin üstüne konacak yazı girilmemiş.");
                }

                if (directionOption == 0)
                {
                    throw new NullReferenceException("Yazının resmin neresine yerleştirileceği belirlenmedi.");
                }

                var graphicsFromImage = Graphics.FromImage(bitmap);

                graphicsFromImage.InterpolationMode = InterpolationMode.High;
                var pointTo = new Point();
                switch (directionOption)
                {
                    case DirectionOption.TopRight:
                        pointTo.X = bitmap.Width - imageFrom.Width;
                        pointTo.Y = 0;

                        break;

                    case DirectionOption.BottomLeft:
                        pointTo.X = 0;
                        pointTo.Y = bitmap.Height - imageFrom.Height;

                        break;

                    case DirectionOption.TopLeft:
                        pointTo.X = 0;
                        pointTo.Y = 0;

                        break;

                    case DirectionOption.BottomRight:
                        pointTo.X = bitmap.Width - imageFrom.Width;
                        pointTo.Y = bitmap.Height - imageFrom.Height;

                        break;

                    case DirectionOption.MiddleRight:
                        pointTo.X = bitmap.Width - imageFrom.Width;
                        pointTo.Y = bitmap.Height - (imageFrom.Height - (imageFrom.Height / 2));

                        break;

                    case DirectionOption.MiddleLeft:
                        pointTo.X = 0;
                        pointTo.Y = bitmap.Height - (imageFrom.Height - (imageFrom.Height / 2));

                        break;

                    case DirectionOption.TopCenter:
                        pointTo.X = bitmap.Width - (imageFrom.Width - (imageFrom.Width / 2));
                        pointTo.Y = 0;

                        break;

                    case DirectionOption.BottomCenter:
                        pointTo.X = bitmap.Width - (imageFrom.Width - (imageFrom.Width / 2));
                        pointTo.Y = bitmap.Height - imageFrom.Height;

                        break;

                    case DirectionOption.Center:
                        pointTo.X = (bitmap.Width / 2) - (imageFrom.Width / 2);
                        pointTo.Y = (bitmap.Height / 2) - (imageFrom.Height / 2);

                        break;

                    default:
                        pointTo.X = 0;
                        pointTo.Y = 0;

                        break;

                }

                //1. parametre imageFrom:(soyut olarak)eklenecek resim
                //2. parametre new Rectangle(pointTo, imageFrom.Size): grafik nesnemizin hangi noktasından itibaren hangi size da resim ekleneceğini gösteren dikdörtgen
                //3. ve 4. parametre 0,0: eklenecek resmin hangi noktasından itibaren çizileceği (yani 1. parametrede alınan fotoğrafı hangi noktasından itibaren ekleyeceğiz)
                //5. ve 6. parametre imageFrom.Width, imageFrom.Height: resim eklenen dikdörtgenin içine hangi boyutlarda eklecek daha küçük değer verilirse resim küçültülerek eklenir
                //7. parametre GraphicsUnit.Pixel: ölçü birimi olrak ne kullanılcak
                //8. parametre imageAttribute: yukarıda belirlenen renk matrix ine göre çizilecek
                graphicsFromImage.DrawImage(imageFrom, new Rectangle(pointTo, imageFrom.Size), 0, 0, imageFrom.Width, imageFrom.Height, GraphicsUnit.Pixel, imageAttribute);
                return bitmap;
            }
        }

        #endregion

        #region ResizeImage(3 Parametreli)

        /// <summary>
        /// Resim Boyutlandirir eger genislik ve yükseklik verilmezse hata verir.
        /// Resmin baz olarak secilen tarafini istenilen rakama boyutlandirir. 
        /// Buradan cikan boyutlanma oranini diger tarafa uygular.
        /// Resmin Genislik veya yuksekligini baz alarak onu istenilen olcuye getirirken 
        /// baz alinmayan tarafi baz alinanin resize oranina gore boyutlandirir.
        /// </summary>
        /// <param name="bitmap">Boyutlandirilacak bitmap resmini ifade eder.</param>
        /// <param name="scaleOption">Resmi yeniden boyutlandırırken neresi baz alinacek. Eger genislik baz alinacaksa. 
        /// size parametresine istenilen genislik, yukseklik baz alinacaksa istenilen yukseklik verilmeli</param>
        /// <param name="size">İstenilen yükseklik veya genişlik değerini ifade eder.</param>
        /// <returns>Bitmap</returns>
        public static Bitmap ResizeImage(Bitmap bitmap, ScaleOption scaleOption, int size)
        {
            if (size == 0)
            {
                throw new NullReferenceException("Maksimum boyut belirtilmedi.");
            }

            if (bitmap == null)
            {
                throw new NullReferenceException("Ölçeklendirilecek resim belirtilmedi.");
            }

            switch (scaleOption)
            {
                case ScaleOption.Width:
                    _oldSize = bitmap.Width;
                    break;
                case ScaleOption.Height:
                    _oldSize = bitmap.Height;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(scaleOption), scaleOption, null);
            }

            var oran = size / (float)_oldSize;

            _newHeight = (int)(bitmap.Height * oran);
            _newWidth = (int)(bitmap.Width * oran);
            var newBitmap = new Bitmap(_newWidth, _newHeight, PixelFormat.Format64bppArgb);
            var graphicsFromImage = Graphics.FromImage(newBitmap);
            graphicsFromImage.InterpolationMode = InterpolationMode.High;

            //1. parametre bitmap:boyutlanacak resim
            //2. parametre new Rectangle(0, 0, newWidth, newHeight): grafik nesnemizin hangi noktasından itibaren hangi size da resim ekleneceğini gösteren dikdörtgen yani yeni boyutu ne olacak resmin
            //3. ve 4. parametre 0,0: eklenecek resmin hangi noktasından itibaren çizileceği (yani 1. parametrede alınan fotoğrafı hangi noktasından itibaren ekleyeceğiz)
            //5. ve 6. parametre bitmap.Width, bitmap.Height: resim eklenen dikdörtgenin içine hangi boyutlarda eklecek tabiki bize verilen olması gereken genişliğe göre ayarlanan boyutlarda
            //7. parametre GraphicsUnit.Pixel: ölçü birimi olrak ne kullanılcak
            graphicsFromImage.DrawImage(bitmap, new Rectangle(0, 0, _newWidth, _newHeight), 0, 0, bitmap.Width, bitmap.Height, GraphicsUnit.Pixel);

            return newBitmap;

        }

        #endregion

        #region ResizeImage(6 Parametreli)

        /// <summary>
        /// Bitmap dosyasını istenen yerinden, istenen boyutta yeniden ölçeklendirmek için kullanılır.
        /// </summary>
        /// <param name="bitmap">Bitmap olarak gönderilecek resmi ifade eder.</param>
        /// <param name="imageResizeOption">Biçimlendirme şeklini ifade eder.</param>
        /// <param name="directionOption">Kesilecek bitmapın yerini ifade eder.</param>
        /// <param name="newWidth">Ölçeklendirilmiş bitmapın yeni genişliğini ifade eder.</param>
        /// <param name="newHeight">Ölçeklendirilmiş bitmapın yeni yüksekliğini ifade eder.</param>
        /// <param name="scaleOption">Yüksekliğinmi yoksa genişliğinmi esas alınacağını ifade eder.</param>
        /// <returns>Bitmap</returns>
        public static Bitmap ResizeImage(Bitmap bitmap, ImageResizeOption imageResizeOption, DirectionOption directionOption, int newWidth, int newHeight, ScaleOption scaleOption)
        {
            switch (imageResizeOption)
            {
                case ImageResizeOption.Fit:
                    if (bitmap.Height - newHeight > bitmap.Width - newWidth)
                    {
                        _rate = newHeight / (float)bitmap.Height;
                        var myNewWidth = (int)(bitmap.Width * _rate);
                        var newerBitmap = new Bitmap(myNewWidth, newHeight, PixelFormat.Format64bppArgb);
                        var graphicsFromImge = Graphics.FromImage(newerBitmap);
                        graphicsFromImge.DrawImage(bitmap, new Rectangle(0, 0, myNewWidth, newHeight), new Rectangle(0, 0, bitmap.Width, bitmap.Height), GraphicsUnit.Pixel);
                        return newerBitmap;

                    }
                    else
                    {
                        _rate = newWidth / (float)bitmap.Width;
                        var myNewHeight = (int)(bitmap.Height * _rate);
                        var newerBitmap = new Bitmap(newWidth, myNewHeight, PixelFormat.Format64bppArgb);
                        using (_graphics = Graphics.FromImage(newerBitmap))
                        {
                            _graphics.DrawImage(bitmap, new Rectangle(0, 0, newWidth, myNewHeight), new Rectangle(0, 0, bitmap.Width, bitmap.Height), GraphicsUnit.Pixel);
                            return newerBitmap;
                        }

                    }
                case ImageResizeOption.Stretch:
                    var newlyBitmap = new Bitmap(newWidth, newHeight, PixelFormat.Format64bppArgb);
                    using (_graphics = Graphics.FromImage(newlyBitmap))
                    {
                        _graphics.DrawImage(bitmap, 0, 0, newWidth, newHeight);
                    }
                    return newlyBitmap;
                case ImageResizeOption.Cut:
                    _newBitmap = new Bitmap(newWidth, newHeight, PixelFormat.Format64bppArgb);
                    using (_graphics = Graphics.FromImage(_newBitmap))
                    {
                        DirectionSet(directionOption, bitmap, newWidth, newHeight);
                        _graphics.DrawImage(bitmap, new Rectangle(0, 0, newWidth, newHeight), _leftCoordinate, _topCoordinate, newWidth, newHeight, GraphicsUnit.Pixel);
                    }
                    return _newBitmap;
                case ImageResizeOption.Scale:
                    if ((newWidth / (float)bitmap.Width > newHeight / (float)bitmap.Height && newWidth / (float)bitmap.Width > 1) || (newWidth / (float)bitmap.Width > newHeight / (float)bitmap.Height && (float)newWidth / bitmap.Width < 1 && (float)newHeight / bitmap.Height < 1))
                    {
                        var myImage = ResizeImage(bitmap, ScaleOption.Width, newWidth);
                        return ResizeImage(myImage, ImageResizeOption.Cut, directionOption, newWidth, newHeight, scaleOption);
                    }
                    else if (bitmap.Width == bitmap.Height)
                    {

                        var myImage = ResizeImage(bitmap, scaleOption, newWidth);
                        return myImage;
                    }
                    else
                    {
                        //yukseklige gore resim boyutlandirilip sonra fazlalik kesilecek
                        var myImage2 = ResizeImage(bitmap, ScaleOption.Height, newHeight);
                        return ResizeImage(myImage2, ImageResizeOption.Cut, directionOption, newWidth, newHeight, scaleOption);
                    }
                default:
                    throw new ArgumentOutOfRangeException(nameof(imageResizeOption), imageResizeOption, null);
            }




            // return new Bitmap(600, 600);

        }

        #endregion

        #region Resim Kesme

        /// <summary>
        /// Bir Bitmap dosyasını istenilen boyutta keser.
        /// </summary>
        /// <param name="bitmap">Bitmap olarak gönderilecek resmi ifade eder.</param>
        /// <param name="width">Kesilecek olan resmin genişliğini ifade eder.</param>
        /// <param name="height">Kesilecek olan resmin yüksekliğini ifade eder.</param>
        /// <param name="directionOption">Bitmap'ın neresinden kesileceğini ifade eder.</param>
        /// <returns>Bitmap</returns>
        public static Bitmap CutImage(Bitmap bitmap, int width, int height, DirectionOption directionOption)
        {
            return bitmap.Clone(new Rectangle(0, 0, width, height), bitmap.PixelFormat);
        }

        /// <summary>
        /// Bir image dosyasını istenilen boyutta keser.
        /// </summary>
        /// <param name="image">image olarak gönderilecek resmi ifade eder.</param>
        /// <param name="width">Kesilecek olan resmin genişliğini ifade eder.</param>
        /// <param name="height">Kesilecek olan resmin yüksekliğini ifade eder.</param>
        /// <param name="directionOption">Bitmap'ın neresinden kesileceğini ifade eder.</param>
        /// <returns>Bitmap</returns>
        public static Image CutImage(Image image, int width, int height, DirectionOption directionOption)
        {
            using (var bitmap = ImageToBitmap(image))
            {
                using (var yeniBitmap = bitmap.Clone(new Rectangle(0, 0, width, height), bitmap.PixelFormat))
                {
                    return BitmapToImage(yeniBitmap);
                }
            }
        }
        #endregion

        #region Resim Convert

        /// <summary>
        /// Gönderilen bir Image dosyasını Bitmap'e çevirir.
        /// </summary>
        /// <param name="image">Image olarak gönderilen dosyayı ifade eder.</param>
        /// <returns>Bitmap </returns>
        public static Bitmap ImageToBitmap(Image image)
        {
            if (image == null)
            {
                throw new NullReferenceException("Bitmap'e dönüştürülecek İmaj dosyası girilmedi.");
            }
            return new Bitmap(image);
        }

        public static Bitmap BitmapFromFile(string path)
        {
            try
            {
                using (var img = Image.FromFile(path))
                {
                    return ImageToBitmap(img);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        public static Image Base64ToImage(string base64String)
        {
            // Convert base 64 string to byte[]
            var imageBytes = Convert.FromBase64String(base64String);
            // Convert byte[] to Image
            using (var ms = new MemoryStream(imageBytes, 0, imageBytes.Length))
            {
                var image = Image.FromStream(ms, true);
                return image;
            }
        }



        /// <summary>
        /// Gönderilen bir Bitmap dosyasını Image'e çevirir.
        /// </summary>
        /// <param name="bitmap">Bitmap olarak gönderilecek resmi ifade eder.</param>
        /// <returns>image</returns>
        public static Image BitmapToImage(Bitmap bitmap)
        {
            if (bitmap == null)
            {
                throw new NullReferenceException("İmaj dosyasına dönüştürülecek Bitmap girilmedi.");
            }
            var image = bitmap as Image;
            return image;

        }

        /// <summary>
        /// Gönderilen bir Bitmap dosyasını Stream'e çevirir.
        /// </summary>
        /// <param name="bitmap">Bitmap olarak gönderilecek resmi ifade eder.</param>
        /// <param name="imageFormat">Stream olarak döndürülecek verinin formatını ifade eder.</param>
        /// <returns>Stream</returns>
        public static Stream BitmapToStream(Bitmap bitmap, ImageFormat imageFormat)
        {
            using (var memoryStream = new MemoryStream())
            {
                _stream = memoryStream;
                bitmap.Save(_stream, imageFormat);
                return _stream;
            }
        }

        /// <summary>
        /// Gönderilen bir Stream dosyasını Bitmap'e çevirir.
        /// </summary>
        /// <param name="stream">Stream olarak gönderilen dosyayı ifade eder.</param>
        /// <returns>Bitmap</returns>
        public static Bitmap StreamToBitmap(Stream stream)
        {
            var bitmap = new Bitmap(stream);
            return bitmap;
        }

        /// <summary>
        /// Gönderilen bir image dosyasını Stream'e çevirir.
        /// </summary>
        /// <param name="image">Image olarak gönderilen dosyayı ifade eder.</param>
        /// <returns>Bitmap</returns>
        public static Stream ImageToStream(Image image)
        {
            var bitmap = new Bitmap(image);
            return BitmapToStream(bitmap, image.RawFormat);
        }

        /// <summary>
        /// Gönderilen bit Bitmap dosyasını istenilen isimle ve formatla kaydeder.
        /// </summary>
        /// <param name="bitmap">Bitmap olarak gönderilen dosyayı ifade eder.</param>
        /// <param name="filePath">Bitmap dosyasının kaydedileceği yeri ifade eder.(Ör: C:\Users\resim.jpg)</param>
        /// <param name="imageFormat">Bitmap dosyasının hangi formatta kaydedileciğini ifade eder.</param>
        public static void SaveBitmap(Bitmap bitmap, string filePath, ImageFormat imageFormat)
        {
            bitmap.Save(filePath, imageFormat);
        }

        /// <summary>
        /// Gönderilen bit Bitmap dosyasını istenilen isimle ve formatla kaydeder.
        /// </summary>
        /// <param name="image">Image olarak gönderilen dosyayı ifade eder.</param>
        /// <param name="filePath">Image dosyasının kaydedileceği yeri ifade eder.(Ör: C:\Users\resim.jpg)</param>
        /// <param name="imageFormat">Image dosyasının hangi formatta kaydedileciğini ifade eder.</param>
        public static void SaveImage(Image image, string filePath, ImageFormat imageFormat)
        {
            image.Save(filePath, imageFormat);
        }

        /// <summary>
        /// Gönderilen base64 string verisini istenilen isimle kaydeder.
        /// </summary>
        /// <param name="base64String">Base64String verisi</param>
        /// <param name="filePath">Image dosyasının kaydedileceği yeri ifade eder.(Ör: D:\resim.jpg)</param>

        public static void SaveImageFromBase64String(string base64String, string filePath)
        {
            var bytes = Convert.FromBase64String(base64String);
            using (var imageFile = new FileStream(filePath, FileMode.Create))
            {
                imageFile.Write(bytes, 0, bytes.Length);
                imageFile.Flush();
            }

        }

        public static string AddCharacterForSplitedText(string text, string joinText)
        {
            var textList = new List<string>();
            var lastIndex = 0;
            for (var i = 0; i < text.Length; i++)
            {
                var c = text[i];
                if (c.ToString() != c.ToString().ToUpper() || c.ToString() == " ")
                {
                    continue;
                }
                var substring = text.Substring(lastIndex, i - lastIndex);
                if (substring != string.Empty)
                {
                    textList.Add(substring);
                }
                lastIndex = i;
            }
            textList.Add(text.Substring(lastIndex, text.Length - lastIndex));
            return string.Join(joinText, textList);
        }

        #endregion

    }
}
