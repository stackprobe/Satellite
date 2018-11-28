using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using Charlotte.Htt;
using Charlotte.Htt.Response;

namespace TestHtmlJpegPngJsonXml
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Press ESCAPE to stop the server.");

			HttServer.Perform(new TestServer0001());
		}

		private class TestServer0001 : HttService
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

			public HttResponse Service(HttRequest req)
			{
				String path = req.GetUrl().PathAndQuery;

				if (path == "/test0001.html")
				{
					StringBuilder buff = new StringBuilder();

					buff.Append("<html>");
					buff.Append("<body>");
					buff.Append("This is test0001.html<br/>");
					buff.Append("<h1>☃☃☃☃☃</h1>");
					buff.Append("<h1>☃☃☃☃☃</h1>");
					buff.Append("<h1>☃☃☃☃☃</h1>");
					buff.Append("<a href=\"/\">return</a>");
					buff.Append("</body>");
					buff.Append("</html>");

					return new HttResHtml(buff.ToString());
				}
				else if (path == "/test0002.jpeg")
				{
					Bitmap bmp = new Bitmap(300, 300);

					using (Graphics g = Graphics.FromImage(bmp))
					{
						g.FillRectangle(Brushes.Orange, 0, 0, 300, 300);
						g.FillEllipse(Brushes.Yellow, 50, 50, 200, 200);
						g.DrawString("☃", new Font("メイリオ", 100f, FontStyle.Regular), Brushes.Blue, 70, 70);
					}
					byte[] imageData;

					using (MemoryStream mem = new MemoryStream())
					{
						bmp.Save(mem, ImageFormat.Jpeg);
						imageData = mem.GetBuffer();
					}
					return new HttResFileImage(imageData, "$.jpeg");
				}
				else if (path == "/test0003.png")
				{
					Bitmap bmp = new Bitmap(800, 600);

					using (Graphics g = Graphics.FromImage(bmp))
					{
						g.FillRectangle(Brushes.DarkGray, 0, 0, 800, 600);

						Random rnd = new Random();

						for (int c = 0; c < 10; c++)
						{
							g.FillRectangle(
								new SolidBrush(Color.FromArgb(
									rnd.Next(256),
									rnd.Next(256),
									rnd.Next(256)
									)),
								rnd.Next(400),
								rnd.Next(300),
								10 + rnd.Next(400 - 10),
								10 + rnd.Next(300 - 10)
								);
						}
					}
					byte[] imageData;

					using (MemoryStream mem = new MemoryStream())
					{
						bmp.Save(mem, ImageFormat.Png);
						imageData = mem.GetBuffer();
					}
					return new HttResFileImage(imageData, "$.png");
				}
				else if (path == "/test0004.json")
				{
					string[] lines = new string[]
					{
						"{",
						"\t\"emotion\": \"nothing\"",
						"}",
					};

					string text = string.Join("\r\n", lines) + "\r\n";

					return new HttResFileImage(Encoding.UTF8.GetBytes(text), "$.json");
				}
				else if (path == "/test0005.xml")
				{
					string[] lines = new string[]
					{
						"<?xml version=\"1.0\" encoding=\"UTF-8\"?>",
						"<root>",
						"\t<key>emotion</key>",
						"\t<value>nothing</value>",
						"</root>",
					};

					string text = string.Join("\r\n", lines) + "\r\n";

					return new HttResFileImage(Encoding.UTF8.GetBytes(text), "$.xml");
				}

				{
					StringBuilder buff = new StringBuilder();

					buff.Append("<html>");
					buff.Append("<body>");
					buff.Append("test page<br/>");
					buff.Append("<a href=\"/test0001.html\">/test0001.html</a><br/>");
					buff.Append("<a href=\"/test0002.jpeg\">/test0002.jpeg</a><br/>");
					buff.Append("<a href=\"/test0003.png\">/test0003.png</a><br/>");
					buff.Append("<a href=\"/test0004.json\">/test0004.json</a><br/>");
					buff.Append("<a href=\"/test0005.xml\">/test0005.xml</a><br/>");
					buff.Append("</body>");
					buff.Append("</html>");

					return new HttResHtml(buff.ToString());
				}
			}
		}
	}
}
