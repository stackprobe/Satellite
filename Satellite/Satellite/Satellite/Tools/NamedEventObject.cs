using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Charlotte.Satellite.Tools
{
	public class NamedEventObject : IDisposable
	{
		private EventWaitHandle E;

		public NamedEventObject(string name)
		{
			this.E = new EventWaitHandle(false, EventResetMode.AutoReset, name);
		}

		public void WaitOne()
		{
			this.E.WaitOne();
		}

		public bool WaitOne(int millis)
		{
			return this.E.WaitOne(millis);
		}

		public void Set()
		{
			this.E.Set();
		}

		public void Close()
		{
			if (this.E != null)
			{
				this.E.Close();
				this.E = null;
			}
		}

		public void Dispose()
		{
			this.Close();
		}
	}
}
