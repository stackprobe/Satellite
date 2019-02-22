using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Charlotte.Htt;
using Charlotte.Htt.Response;
using Charlotte.Satellite.Tools;

namespace LiteFiler
{
	public class LiteFilerService : HttService
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

		private const string ROOT_DIR = @"C:\";

		public HttResponse Service(HttRequest req)
		{
			string reqPath = Utils.DecodeUrl(req.GetUrl().AbsolutePath);
			string[] pTkns = StringTools.Tokenize(reqPath, '/').Where(pTkn => pTkn != "").ToArray();
			string path = string.Join("/", pTkns);

			path = Path.Combine(ROOT_DIR, path);

			if (File.Exists(path))
				return new HttResFile(path);

			if (Directory.Exists(path))
			{
				if (reqPath.EndsWith("/") == false)
					return new HttRes301(Utils.EncodeUrl(reqPath + "/"));

				string[] subDirs = Directory.GetDirectories(path);
				string[] subFiles = Directory.GetFiles(path);

				Array.Sort(subDirs);
				Array.Sort(subFiles);

				StringBuilder html = new StringBuilder();

				html.Append("<html>");
				html.Append("<body>");
				html.Append("<h1>");
				html.Append(path);
				html.Append("</h1>");
				html.Append("<table border='1'>");
				html.Append("<tr>");
				html.Append("<th>name</th>");
				html.Append("<th>isDirectory</th>");
				html.Append("<th>size</th>");
				html.Append("<th>lastUpdateTime</th>");
				html.Append("</tr>");

				if (1 <= pTkns.Length)
				{
					html.Append("<tr>");
					html.Append("<td><a href='..'>&lt;Parent Directory&gt;</a></td>");
					html.Append("<td></td>");
					html.Append("<td></td>");
					html.Append("<td></td>");
					html.Append("</tr>");
				}
				foreach (string subDir in subDirs)
					AppendSubFileOrDirectory(html, subDir, true);

				foreach (string subFile in subFiles)
					AppendSubFileOrDirectory(html, subFile, false);

				html.Append("</table>");
				html.Append("</body>");
				html.Append("</html>");

				return new HttResHtml(html.ToString());
			}
			return new HttRes404();
		}

		private static void AppendSubFileOrDirectory(StringBuilder html, string subPath, bool dirFlag)
		{
			html.Append("<tr>");
			html.Append("<td>");
			html.Append("<a href='");
			html.Append(Utils.EncodeUrl(Path.GetFileName(subPath)));
			html.Append("'>");
			html.Append(Path.GetFileName(subPath));
			html.Append("</a>");
			html.Append("</td>");
			html.Append("<td>");
			html.Append(dirFlag);
			html.Append("</td>");

			if (dirFlag)
			{
				html.Append("<td></td>");
				html.Append("<td></td>");
			}
			else
			{
				html.Append("<td>");
				html.Append(new FileInfo(subPath).Length);
				html.Append("</td>");
				html.Append("<td>");
				html.Append(new FileInfo(subPath).LastWriteTime);
				html.Append("</td>");
			}
			html.Append("</tr>");
		}
	}
}
