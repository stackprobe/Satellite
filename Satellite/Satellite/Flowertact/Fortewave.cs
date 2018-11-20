using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Charlotte.Flowertact.Tools;
using Charlotte.Satellite.Tools;

namespace Charlotte.Flowertact
{
	public class Fortewave : IDisposable
	{
		private object SYNCROOT = new object();
		private PostOfficeBox _rPob; // closed 確認用
		private PostOfficeBox _wPob;

		public Fortewave(string ident)
			: this(ident, ident)
		{ }

		public Fortewave(string rIdent, string wIdent)
		{
			if (rIdent == null)
				throw new ArgumentNullException("rIdent");

			if (wIdent == null)
				throw new ArgumentNullException("wIdent");

			_rPob = new PostOfficeBox(rIdent);
			_wPob = new PostOfficeBox(wIdent);
		}

		public void Clear()
		{
			lock (SYNCROOT)
			{
				if (_rPob == null)
					throw new Exception("already closed");

				_rPob.Clear();
				_wPob.Clear();
			}
		}

		public void Send(object sendObj)
		{
			if (sendObj == null)
				throw new ArgumentNullException("sendObj");

			lock (SYNCROOT)
			{
				if (_rPob == null)
					throw new Exception("already closed");

				QueueData<SubBlock> sendData = new Serializer(sendObj).GetBuff();
				_wPob.Send(sendData);
			}
		}

		public object Recv(int millis)
		{
			if (millis < Timeout.Infinite)
				throw new ArgumentException("millis lt min");

			lock (SYNCROOT)
			{
				if (_rPob == null)
					throw new Exception("already closed");

				byte[] recvData = _rPob.Recv(millis);

				if (recvData == null)
					return null;

				object recvObj = new Deserializer(recvData).Next();
				return recvObj;
			}
		}

		public void Pulse()
		{
			lock (SYNCROOT)
			{
				if (_rPob == null)
					throw new Exception("already closed");

				_rPob.Pulse();
				_wPob.Pulse();
			}
		}

		public void Close()
		{
			lock (SYNCROOT)
			{
				if (_rPob != null) // ? not closed
				{
					_rPob.Close();
					_wPob.Close();
					_rPob = null;
					_wPob = null;
				}
			}
		}

		public void Dispose()
		{
			this.Close();
		}
	}
}
