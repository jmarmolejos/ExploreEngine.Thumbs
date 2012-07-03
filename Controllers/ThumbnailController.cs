using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExploreEngine.Thumbs.Models;

namespace ExploreEngine.Thumbs.Controllers
{
	public class ThumbnailController : Controller
	{
		// size:250x400
		public FileResult Index(string path, string size) {

			var whiteList = new string[] {"40x40", "80x80", "160x96", "100x60"};

			if (!whiteList.Contains(size)) {
				throw new HttpException(404, "File not foud.");
			}

			var filePath = Server.MapPath(path); // TODO move to FileManager class

			var fileManager = new FileManager(ControllerContext.HttpContext);

			var fileSize = size.Split('x');
			var thumbPath = fileManager.GetThumbPathFromMainFile(int.Parse(fileSize[0]), int.Parse(fileSize[1]), filePath);

			if (System.IO.File.Exists(thumbPath))
			{
				return File(thumbPath, "image/jpg");
			}
			
			MemoryStream stream;

			stream = fileManager.GetThumbnailStream(filePath, int.Parse(fileSize[0]), int.Parse(fileSize[1]));

			return File(stream, "image/jpg");
		}

	}
}