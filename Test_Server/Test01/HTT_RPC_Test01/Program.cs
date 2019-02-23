using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Htt;
using Charlotte.Htt.Response;

namespace HTT_RPC_Test01
{
	/// <summary>
	/// これは「コンソール アプリケーション」です。
	/// エスケープキーを押すと終了します。
	/// </summary>
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Press ESCAPE to stop the server.");

			HttServer.Perform(new Test01Service());
		}

		private class Test01Service : HttService
		{
			public bool Interlude()
			{
				return Console.KeyAvailable == false || Console.ReadKey().KeyChar != (char)27;
			}

			private StringBuilder _buff;

			public HttResponse Service(HttRequest req)
			{
				_buff = new StringBuilder();

				_buff.Append("<html>");
				_buff.Append("<body>");
				_buff.Append("<table border=\"1\">");

				AddTr("クライアントのIPアドレス", req.GetClientIPAddress());
				AddTr("Method", req.GetMethod());
				AddTr("Url", req.GetUrl().ToString());
				AddTr("HTTP_Version", req.GetHTTPVersion());

				foreach (string headerKey in req.GetHeaderFields().Keys)
				{
					AddTr("Header_" + headerKey, req.GetHeaderFields()[headerKey]);
				}
				AddTr("Body", ToAsciiString(req.GetBodyPart()));

				_buff.Append("</table>");
				_buff.Append("</body>");
				_buff.Append("</html>");

				return new HttResHtml(_buff.ToString());
			}

			private void AddTr(string title, string value)
			{
				_buff.Append("<tr>");
				_buff.Append("<td>");
				_buff.Append(title);
				_buff.Append("</td>");
				_buff.Append("<td>");
				_buff.Append(value);
				_buff.Append("</td>");
				_buff.Append("</tr>");
			}

			private static string ToAsciiString(byte[] data)
			{
				try
				{
					return Encoding.ASCII.GetString(data);
				}
				catch (Exception e)
				{
					return e.Message;
				}
			}
		}
	}
}
