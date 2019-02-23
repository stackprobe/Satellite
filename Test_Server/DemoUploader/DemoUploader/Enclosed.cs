using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DemoUploader
{
	public class Enclosed
	{
		private string Text;
		private int BgnBgn;
		private int BgnEnd;
		private int EndBgn;
		private int EndEnd;

		public Enclosed(string text, string bgnTag, string endTag)
		{
			this.Text = text;
			this.BgnBgn = text.IndexOf(bgnTag);

			if (this.BgnBgn == -1)
				throw new Exception("no bgnTag");

			this.BgnEnd = this.BgnBgn + bgnTag.Length;
			this.EndBgn = text.IndexOf(endTag, this.BgnEnd);

			if (this.EndBgn == -1)
				throw new Exception("no endTag");

			this.EndEnd = this.EndBgn + endTag.Length;
		}

		public string GetInner()
		{
			return this.Text.Substring(this.BgnEnd, this.EndBgn - this.BgnEnd);
		}

		public string GetLeft()
		{
			return this.Text.Substring(0, this.BgnBgn);
		}

		public string GetRight()
		{
			return this.Text.Substring(this.EndEnd);
		}
	}
}
