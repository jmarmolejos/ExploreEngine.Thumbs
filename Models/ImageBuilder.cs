using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;

namespace ExploreEngine.Thumbs.Models
{
	public class ImageBuilder
	{
		public MemoryStream BuildThumb(string absolutePath, int width, int height)
		{
			var image = Image.FromFile(absolutePath);

			double aspectRatio = image.Width/(double)image.Height;
			int resizeWidth, resizeHeight;

			resizeHeight = height;
			resizeWidth = (int)Math.Ceiling(height * aspectRatio);

			if (resizeWidth < width) {
				resizeWidth = width;
				resizeHeight = (int) Math.Ceiling(width/aspectRatio);
			}
			
			var resize = Resize(image, resizeWidth, resizeHeight);
			var thumb = CropImage(resize, width, height, (resize.Width / 2) - (width / 2), (resize.Height / 2) - (height / 2));

			EncoderParameters encoderParameters = GetEncodingParameters();
			ImageCodecInfo jpegEncoder = GetEncoder(ImageFormat.Jpeg);

			var memoryStream = new MemoryStream();
			thumb.Save(memoryStream, jpegEncoder, encoderParameters);
			memoryStream.Seek(0, 0);

			image.Dispose();
			thumb.Dispose();
			resize.Dispose();

			return memoryStream;
		}

		private static EncoderParameters GetEncodingParameters()
		{
			Encoder encoder = Encoder.Quality;

			EncoderParameters encoderParameters = new EncoderParameters(1);

			EncoderParameter encoderParameter = new EncoderParameter(encoder, 80L);

			encoderParameters.Param[0] = encoderParameter;
			return encoderParameters;
		}

		private static System.Drawing.Image Resize(System.Drawing.Image image, int width, int height)
		{
			System.Drawing.Image fullsizeImage = image;
			double aspectRatio = Math.Round(Convert.ToDouble(image.Width) / Convert.ToDouble(image.Height), 2);

			// Prevent using images internal thumbnail
			fullsizeImage.RotateFlip(RotateFlipType.Rotate180FlipNone);
			fullsizeImage.RotateFlip(RotateFlipType.Rotate180FlipNone);

			int targetHeight;
			int targetWidth;

			if (image.Width > image.Height)
			{
				targetHeight = height;
				targetWidth = (int)(targetHeight * aspectRatio);
			}
			else
			{
				targetHeight = Convert.ToInt16(width / aspectRatio);
				targetWidth = width;
			}

			System.Drawing.Image resizedImage = fullsizeImage.GetThumbnailImage(targetWidth, targetHeight, null, IntPtr.Zero);

			fullsizeImage.Dispose();

			return resizedImage;
		}

		public static System.Drawing.Image CropImage(System.Drawing.Image image, int width, int height, int xPosition, int yPosition)
		{
			using (Bitmap bmp = new Bitmap(width, height, image.PixelFormat))
			{
				bmp.SetResolution(image.HorizontalResolution, image.VerticalResolution);
				using (Graphics graphic = Graphics.FromImage(bmp))
				{
					graphic.SmoothingMode = SmoothingMode.None;
					graphic.InterpolationMode = InterpolationMode.Low;
					graphic.PixelOffsetMode = PixelOffsetMode.None;
					graphic.DrawImage(image, new Rectangle(0, 0, width, height), xPosition, yPosition, width, height, GraphicsUnit.Pixel);
					return (Image)bmp.Clone();
				}
			}
		}

		private ImageCodecInfo GetEncoder(ImageFormat format)
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