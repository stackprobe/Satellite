using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Charlotte.Satellite.Tools
{
	public class MutexObject : IDisposable
	{
		private Mutex M;

		public MutexObject(string name)
		{
			this.M = new Mutex(false, name);
		}

		public void WaitOne()
		{
			this.M.WaitOne();
		}

		public bool WaitOne(int millis)
		{
			return this.M.WaitOne(millis);
		}

		public void Release()
		{
			this.M.ReleaseMutex();
		}

		public void Close()
		{
			if (this.M != null)
			{
				this.M.Close();
				this.M = null;
			}
		}

		public void Dispose()
		{
			this.Close();
		}

		public SectionObject Section()
		{
			return new SectionObject(this);
		}

		public class SectionObject : IDisposable
		{
			public MutexObject Mo;

			public SectionObject(MutexObject mo)
			{
				this.Mo = mo;
				this.Mo.WaitOne();
			}

			public void Dispose()
			{
				if (this.Mo != null)
				{
					this.Mo.Release();
					this.Mo = null;
				}
			}
		}

		public InverseObject Inverse()
		{
			return new InverseObject(this);
		}

		public class InverseObject : IDisposable
		{
			public MutexObject Mo;

			public InverseObject(MutexObject mo)
			{
				this.Mo = mo;
				this.Mo.Release();
			}

			public void Dispose()
			{
				if (this.Mo != null)
				{
					this.Mo.WaitOne();
					this.Mo = null;
				}
			}
		}
	}
}
