using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Charlotte.Satellite.Tools;

#pragma warning disable 162 // 到達できないコードが検出されました。

namespace Charlotte.Satellite
{
	public class Satellizer : IDisposable
	{
		private const string COMMON_ID = "{cb88beb3-4661-4399-98a8-a5dff3654347}"; // shared_uuid@g
		private object SYNCROOT = new object();
		private Connection Conn;
		private bool Connected;

		public Satellizer(string group, string ident)
		{
			throw new NotImplementedException("Sorry, This class works improperly !!!"); // NOTE: 使わないので放置

			if (group == null)
				throw new ArgumentNullException("group");

			if (ident == null)
				throw new ArgumentNullException("ident");

			this.Conn = new Connection(group, ident);
		}

		public static void Listen(string group, string ident, int millis, Server server)
		{
			throw new NotImplementedException("Sorry, This method works improperly !!!"); // NOTE: 使わないので放置

			if (group == null)
				throw new ArgumentNullException("group");

			if (ident == null)
				throw new ArgumentNullException("ident");

			if (millis < Timeout.Infinite)
				throw new ArgumentException("millis lt min");

			if (server == null)
				throw new ArgumentNullException("server");

			Satellizer stllzr = null;

			try
			{
				using (MutexObject mtx = new MutexObject(COMMON_ID))
				using (mtx.Section())
				{
					while (server.Interlude())
					{
						if (stllzr == null)
						{
							stllzr = new Satellizer(group, ident);
							stllzr.Conn.ListenFlag = true;
						}
						stllzr.Connected = stllzr.Conn.Connect(millis, mtx);

						if (stllzr.Connected)
						{
							Satellizer f_stllzr = stllzr;
							stllzr = null;

							new Thread((ThreadStart)delegate
							{
								try
								{
									server.ServiceTh(f_stllzr);
								}
								catch (Exception e)
								{
									SystemTools.Error(e);
								}
								f_stllzr.Close();
							})
							.Start();
						}
					}
				}
			}
			finally
			{
				if (stllzr != null)
					stllzr.Close();
			}
		}

		public interface Server
		{
			bool Interlude();
			void ServiceTh(Satellizer stllzr);
		}

		public bool Connect(int millis)
		{
			if (millis < Timeout.Infinite)
				throw new ArgumentException("millis lt min");

			lock (SYNCROOT)
			{
				if (this.Conn == null)
					throw new Exception("already closed");

				if (this.Connected)
					throw new Exception("already connected");

				using (MutexObject mtx = new MutexObject(COMMON_ID))
				using (mtx.Section())
				{
					this.Connected = this.Conn.Connect(millis, mtx);
					return this.Connected;
				}
			}
		}

		public void Send(object sendObj)
		{
			if (sendObj == null)
				throw new ArgumentNullException("sendObj");

			lock (SYNCROOT)
			{
				if (this.Conn == null)
					throw new Exception("already closed");

				if (this.Connected == false)
					throw new Exception("not connected");

				QueueData<SubBlock> sendData = new Serializer(sendObj).GetBuff();
				this.Conn.Send(sendData);
			}
		}

		public object Recv(int millis)
		{
			if (millis < Timeout.Infinite)
				throw new ArgumentException("millis lt min");

			lock (SYNCROOT)
			{
				if (this.Conn == null)
					throw new Exception("already closed");

				if (this.Connected == false)
					throw new Exception("not connected");

				byte[] recvData = this.Conn.Recv(millis);

				if (recvData == null)
					return null;

				object recvObj = new Deserializer(recvData).Next();
				return recvObj;
			}
		}

		public bool IsOtherSideDisconnected()
		{
			lock (SYNCROOT)
			{
				if (this.Conn == null)
					throw new Exception("already closed");

				if (this.Connected == false)
					throw new Exception("not connected");

				return this.Conn.IsDisconnected();
			}
		}

		public void Disconnect()
		{
			lock (SYNCROOT)
			{
				if (this.Connected)
				{
					this.Conn.Disconnect();
					this.Connected = false;
				}
			}
		}

		public void Close()
		{
			lock (SYNCROOT)
			{
				if (this.Conn != null)
				{
					this.Disconnect();
					this.Conn.Close();
					this.Conn = null;
				}
			}
		}

		public void Dispose()
		{
			this.Close();
		}
	}
}
