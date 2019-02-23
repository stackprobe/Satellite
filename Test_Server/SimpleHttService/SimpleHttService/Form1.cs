using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using Charlotte.Htt;
using Charlotte.Htt.Response;

namespace SimpleHttService
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			// noop
		}

		private Thread Th;
		private static bool Dead = false;

		private void Form1_Shown(object sender, EventArgs e)
		{
			Th = new Thread(() => HttServer.Perform(new Test01Service()));
			Th.Start();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void Form1_FormClosed(object sender, FormClosedEventArgs e)
		{
			Dead = true;
			Th.Join();
		}

		private class Test01Service : HttService
		{
			public bool Interlude()
			{
				return Dead == false;
			}

			public HttResponse Service(HttRequest req)
			{
				StringBuilder buff = new StringBuilder();

				buff.Append("<html>");
				buff.Append("<body>");
				buff.Append("<h1>リクエストの内容は以下のとおりです。</h1>");
				buff.Append("<hr/>");
				buff.Append("<table>");
				buff.Append("<tr><td>Client IP address</td><td>");
				buff.Append(req.GetClientIPAddress());
				buff.Append("</td></tr>");
				buff.Append("<tr><td>Method</td><td>");
				buff.Append(req.GetMethod());
				buff.Append("</td></tr>");
				buff.Append("<tr><td>URL</td><td>");
				buff.Append(req.GetUrlString());
				buff.Append("</td></tr>");
				buff.Append("<tr><td>HTTP version</td><td>");
				buff.Append(req.GetHTTPVersion());
				buff.Append("</td></tr>");
				buff.Append("</table>");
				buff.Append("</html>");

				return new HttResHtml(buff.ToString());
			}
		}
	}
}
