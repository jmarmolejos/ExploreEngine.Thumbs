using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace ExploreEngine.Thumbs.Models
{
	public class FileManager
	{
		private HttpContextBase _context;
		private string _filesPath;

		public FileManager(HttpContextBase context) {
			_context = context;

			_filesPath = _context.Server.MapPath("~/Content/Thumbs/");
		}

		public MemoryStream GetThumbnailStream(string absolutePath, int width, int height) {
			var builder = new ImageBuilder();

			var stream = builder.BuildThumb(absolutePath, width, height);

			var fileName = Path.GetFileNameWithoutExtension(absolutePath);
			var fileExtension = Path.GetExtension(absolutePath);

			if (!Directory.Exists(_filesPath)) {
				Directory.CreateDirectory(_filesPath);
			}

			var newPath = GetThumbPath(width, height, fileExtension, _filesPath, fileName);

			if (!File.Exists(newPath)) {

				using (var file = new FileStream(newPath, FileMode.Create, System.IO.FileAccess.Write))
				{
					stream.Seek(0, 0);
					stream.CopyTo(file);

					file.Flush();
				}
			}

			stream.Seek(0, 0);
			return stream;
		}

		public string GetThumbPath(int width, int height, string fileExtension, string filesPath, string fileName) {
			return filesPath + fileName + "_" + width.ToString() + "x" + height.ToString() + fileExtension;
		}

		public string GetThumbPathFromMainFile(int width, int height, string absolutePath) {

			var fileName = Path.GetFileNameWithoutExtension(absolutePath);
			var fileExtension = Path.GetExtension(absolutePath);

			return _filesPath + fileName + "_" + width.ToString() + "x" + height.ToString() + fileExtension;
		}
	}
}