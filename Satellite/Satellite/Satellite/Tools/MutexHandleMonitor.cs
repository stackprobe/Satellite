using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Charlotte.Satellite.Tools
{
	public class MutexHandleMonitor
	{
		public object SYNCROOT = new object();

		private const int INDEX_START = 0;
		private const int INDEX_END = 30;

		private string _name;
		private MutexObject _mtx = null;
		private int _index = -1;
		private int _overEnterCount = 0;

		public MutexHandleMonitor(string name)
		{
			_name = name;
		}

		public void Enter()
		{
			if (_mtx != null)
			{
				_overEnterCount++;
				return;
			}
			for (_index = INDEX_START; _index < INDEX_END; _index++)
			{
				_mtx = new MutexObject(_name + "_" + _index);

				if (_mtx.WaitOne(0))
					return;

				_mtx.Close();
			}
			throw new Exception("Too many Satellite processes running !!!");
		}

		public int GetOtherHandleCount()
		{
			int c = 0;

			for (int i = INDEX_START; i < INDEX_END; i++)
			{
				if (i != _index)
				{
					MutexObject m = new MutexObject(_name + "_" + i);

					if (m.WaitOne(0))
						m.Release();
					else
						c++;

					m.Close();
				}
			}
			return c + _overEnterCount;
		}

		public void Leave()
		{
			if (1 <= _overEnterCount)
			{
				_overEnterCount--;
				return;
			}
			_mtx.Release();
			_mtx.Close();
			_mtx = null;
		}
	}
}
