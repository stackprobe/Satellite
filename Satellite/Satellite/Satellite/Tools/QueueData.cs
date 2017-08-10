using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Charlotte.Satellite.Tools
{
	public class QueueData<T>
	{
		private LinkNode Top;
		private LinkNode Last;
		private int Count;

		public QueueData()
		{
			this.Top = new LinkNode();
			this.Last = this.Top;
		}

		public void Add(T e)
		{
			this.Last.Element = e;
			this.Last.Next = new LinkNode();
			this.Last = this.Last.Next;
			this.Count++;
		}

		public T Poll(T defval)
		{
			if (this.Count <= 0)
				return defval;

			T ret = this.Top.Element;
			this.Top = this.Top.Next;
			this.Count--;
			return ret;
		}

		public int GetCount()
		{
			return this.Count;
		}

		private class LinkNode
		{
			public T Element;
			public LinkNode Next;
		}
	}
}
