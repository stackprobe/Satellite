using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Htt;
using Charlotte.Htt.Response;

namespace EasyBBS
{
	public class EasyBBSService : HttService
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

		private List<RemarkInfo> Remarks = new List<RemarkInfo>();

		public HttResponse Service(HttRequest req)
		{
			if (req.GetUrl().AbsolutePath == "/remark")
			{
				byte[] body = req.GetBodyPart();
				string sBody = Encoding.ASCII.GetString(body);
				Dictionary<string, string> q = Utils.ParseQuery(sBody);

				string user = q["user"];
				string eMailAddress = q["e-mail"];
				string message = q["message"];

				user = user.Trim();
				user = Utils.CutTrail(user, 100);
				eMailAddress = eMailAddress.Trim();
				eMailAddress = Utils.CutTrail(eMailAddress, 100);
				message = message.Trim();
				message = Utils.CutTrail(message, 500);

				if (user == "")
					user = "anonymous";

				if (message == "")
					message = "(silent)";

				RemarkInfo remark = new RemarkInfo()
				{
					TimeStamp = DateTime.Now,
					IPAddress = req.GetClientIPAddress(),
					User = user,
					EMailAddress = eMailAddress,
					Message = message,
				};

				if (1000 < this.Remarks.Count)
					this.Remarks.RemoveAt(0);

				this.Remarks.Add(remark);

				string html = Resource.HTML_REMARKED;

				html = html.Replace("${USER}", Utils.Encode(user));
				html = html.Replace("${E-MAIL}", Utils.Encode(eMailAddress));

				return new HttResHtml(html);
			}
			else
			{
				string user = null;
				string eMailAddress = null;

				string query = req.GetUrl().Query;

				if (query != null)
				{
					if (query.StartsWith("?"))
						query = query.Substring(1);

					Dictionary<string, string> q = Utils.ParseQuery(query);

					q.TryGetValue("user", out user);
					q.TryGetValue("e-mail", out eMailAddress);
				}

				if (user == null)
					user = "anonymous@" + new Random().Next(10000);

				if (eMailAddress == null)
					eMailAddress = "sage";

				string html = Resource.HTML_MAIN;

				html = html.Replace("${USER}", Utils.Encode(user));
				html = html.Replace("${E-MAIL}", Utils.Encode(eMailAddress));
				html = html.Replace("${TIMELINE}", this.GetTimeline());

				return new HttResHtml(html);
			}
		}

		private string GetTimeline()
		{
			StringBuilder html = new StringBuilder();

			foreach (RemarkInfo remark in this.Remarks)
			{
				String text = Resource.HTML_REMARK;

				text = text.Replace("${DATE-TIME}", remark.TimeStamp.ToString());
				text = text.Replace("${IP}", remark.IPAddress);
				text = text.Replace("${USER}", Utils.Encode(remark.User));
				text = text.Replace("${E-MAIL}", Utils.Encode(remark.EMailAddress));
				text = text.Replace("${MESSAGE}", Utils.EncodeMultiLineText(remark.Message));

				html.Append(text);
			}
			return html.ToString();
		}
	}
}
