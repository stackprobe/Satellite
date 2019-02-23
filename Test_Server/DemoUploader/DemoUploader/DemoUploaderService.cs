using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Charlotte.Htt;
using Charlotte.Htt.Response;
using Charlotte.Satellite.Tools;

namespace DemoUploader
{
	public class DemoUploaderService : HttService
	{
		public bool Interlude()
		{
			while (Console.KeyAvailable)
			{
				ConsoleKeyInfo cki = Console.ReadKey(true);

				if (cki.KeyChar == (char)27) // ? press escape
				{
					return false;
				}
			}
			return true;
		}

		private byte[] LastUploadedFileData;
		private string LastUploadedFile;

		public HttResponse Service(HttRequest req)
		{
			if (req.GetUrl().AbsolutePath.StartsWith("/uploaded-file/"))
				return new HttResFileImage(this.LastUploadedFileData, this.LastUploadedFile);

			if (req.GetUrl().AbsolutePath == "/upload")
			{
				byte[] body = req.GetBodyPart();

				MultiPartFormData mpfd = new MultiPartFormData();
				mpfd.Load(body);

				MultiPartContent uploadFile = mpfd.Get("upload-file");
				MultiPartContent supplement = mpfd.Get("supplement");

				string html = Resource.HTML_UPLOADED;

				string htmlFileName = uploadFile.FileName;
				int i = htmlFileName.LastIndexOf('/');
				htmlFileName = htmlFileName.Substring(i + 1);
				i = htmlFileName.LastIndexOf('\\');
				htmlFileName = htmlFileName.Substring(i + 1);
				htmlFileName = Utils.EncodeUrl(htmlFileName);

				html = ProcessTag(html, "_overview", innerHtml => GetOverview(innerHtml, htmlFileName, GetFileType(uploadFile.FileName)));
				html = html.Replace("${FILE-NAME}", uploadFile.FileName);
				html = html.Replace("${FILE-SIZE}", "" + uploadFile.Data.Length);
				html = html.Replace("${SUPPLEMENT}", Encoding.UTF8.GetString(supplement.Data));
				html = html.Replace("${FILE-DATA-HEX}", ToHex(uploadFile.Data));

				this.LastUploadedFileData = uploadFile.Data;
				this.LastUploadedFile = uploadFile.FileName;

				return new HttResHtml(html);
			}

			return new HttResHtml(Resource.HTML_MAIN);
		}

		private string ProcessTag(string html, string tagName, Func<string, string> processor)
		{
			string openTag = "<" + tagName + ">";
			string closeTag = "</" + tagName + ">";

			Enclosed encl = new Enclosed(html, openTag, closeTag);
			string innerHtml = encl.GetInner();

			innerHtml = processor(innerHtml);

			html = encl.GetLeft() + innerHtml + encl.GetRight();
			return html;
		}

		private string GetOverview(string innerHtml, string fileName, string fileType)
		{
			string openTag = "<" + fileType + ">";
			string closeTag = "</" + fileType + ">";

			Enclosed encl = new Enclosed(innerHtml, openTag, closeTag);
			innerHtml = encl.GetInner();

			innerHtml = innerHtml.Replace("${FILE-NAME}", fileName);

			return innerHtml;
		}

		private string GetFileType(string fileName)
		{
			string contentType = ExtToContentType.GetContentType(Path.GetExtension(fileName));

			if (contentType.StartsWith("image/"))
				return "_image";

			if (contentType.StartsWith("video/"))
				return "_video";

			if (contentType.StartsWith("audio/"))
				return "_audio";

			return "_default";
		}

		private static string ToHex(byte[] data)
		{
			StringBuilder buff = new StringBuilder();

			for (int index = 0; index < data.Length; index++)
			{
				int i = data[index];

				if (index == 1000)
				{
					buff.Append("...");
					break;
				}
				buff.Append(StringTools.hexadecimal[i / 16]);
				buff.Append(StringTools.hexadecimal[i % 16]);
			}
			return buff.ToString();
		}
	}
}
