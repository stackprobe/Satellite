using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Diagnostics;

namespace Charlotte.Satellite.Tools
{
	public class Connection
	{
		private const string COMMON_ID = "{f5f0b66c-3aa9-48ef-a924-9918f1c69b7a}"; // shared_uuid@g
		private string Group;
		private string Ident;
		private string Session;
		private string CommonDir;
		private string GroupDir;
		private string IdentDir;
		private string SessionDir;
		private MutexObject Mtx;
		private NamedEventObject Ev;
		private string OtherIdent;
		private string OtherSession;
		private string OtherIdentDir;
		private string OtherSessionDir;
		private NamedEventObject OtherEv;
		private string FirstTimeFile;
		private MutexObject ProcAliveMtx;
		private MutexObject OtherProcAliveMtx;

		public Connection(string group, string ident)
		{
			this.Group = SecurityTools.GetSHA512_128String(group);
			this.Ident = SecurityTools.GetSHA512_128String(ident);
			this.Session = StringTools.GetUUID();
			this.CommonDir = Path.Combine(SystemTools.GetTmp(), COMMON_ID);
			this.GroupDir = Path.Combine(this.CommonDir, this.Group);
			this.IdentDir = Path.Combine(this.GroupDir, this.Ident);
			this.SessionDir = Path.Combine(this.IdentDir, this.Session);
			this.Mtx = new MutexObject(COMMON_ID);
			this.Ev = new NamedEventObject(this.Session);
			this.FirstTimeFile = this.CommonDir + "_1";
			this.ProcAliveMtx = new MutexObject(this.Session + "_PA");
			this.ProcAliveMtx.WaitOne();
		}

		public bool ListenFlag;

		public bool Connect(int millis, MutexObject outerMtx)
		{
			//string pidFile = Path.Combine(this.SessionDir, "_PID");

			using (this.Mtx.Section())
			{
				if (FileTools.ExistFile(this.FirstTimeFile) == false)
				{
					if (FileTools.ExistDir(this.CommonDir))
						FileTools.DeleteDir(this.CommonDir, true);

					FileTools.CreateFile(this.FirstTimeFile);
					SystemTools.MoveFileEx(this.FirstTimeFile, null, SystemTools.MoveFileFlags.DelayUntilReboot);
				}
				FileTools.CreateDir(this.CommonDir);
				FileTools.CreateDir(this.GroupDir);
				FileTools.CreateDir(this.IdentDir);
				FileTools.CreateDir(this.SessionDir);
				//File.WriteAllBytes(pidFile, IntTools.ToBytes(Process.GetCurrentProcess().Id));

				if (this.ListenFlag)
				{
					string listenFile = Path.Combine(this.SessionDir, "_listen");
					FileTools.CreateFile(listenFile);
				}
				if (this.TryConnect())
					return true;
			}
			using (outerMtx.Inverse())
			{
				this.Ev.WaitOne(millis);
			}
			using (this.Mtx.Section())
			{
				if (this.TryConnect())
					return true;

				//FileTools.DeleteFile(pidFile);

				if (this.ListenFlag)
				{
					string listenFile = Path.Combine(this.SessionDir, "_listen");
					FileTools.DeleteFile(listenFile);
				}
				FileTools.DeleteDir(this.SessionDir);
				FileTools.DeleteDir(this.IdentDir);
				FileTools.DeleteDir(this.GroupDir);
				FileTools.DeleteDir(this.CommonDir);
			}
			return false;
		}

		private bool TryConnect()
		{
			{
				string connectFile = Path.Combine(this.SessionDir, "_connect");

				if (FileTools.ExistFile(connectFile))
				{
					byte[] data = File.ReadAllBytes(connectFile);
					string text = StringTools.ENCODING_SJIS.GetString(data);
					string[] tokens = text.Split('\n');

					int c = 0;
					this.OtherIdent = tokens[c++];
					this.OtherIdentDir = tokens[c++];
					this.OtherSession = tokens[c++];
					this.OtherSessionDir = tokens[c++];
					this.OtherEv = new NamedEventObject(this.OtherSession);
					this.OtherProcAliveMtx = new MutexObject(this.OtherSession + "_PA");

					return true;
				}
			}

			this.OtherIdent = this.GetOtherIdent();

			if (this.OtherIdent != null)
			{
				this.OtherIdentDir = Path.Combine(this.GroupDir, this.OtherIdent);
				this.OtherSession = this.GetOtherSession(this.OtherIdentDir);

				if (this.OtherSession != null)
				{
					this.OtherSessionDir = Path.Combine(this.OtherIdentDir, this.OtherSession);

					{
						String[] tokens = new String[]
						{
							this.Ident,
							this.IdentDir,
							this.Session,
							this.SessionDir,
						};
						String text = string.Join("\n", tokens);
						byte[] data = StringTools.ENCODING_SJIS.GetBytes(text);

						string connectFile = Path.Combine(this.OtherSessionDir, "_connect");

						File.WriteAllBytes(connectFile, data);
					}

					{
						string connectFile = Path.Combine(this.SessionDir, "_connect");

						FileTools.CreateFile(connectFile);
					}

					this.OtherEv = new NamedEventObject(this.OtherSession);
					this.OtherEv.Set();

					return true;
				}
			}
			return false;
		}

		private string GetOtherIdent()
		{
			foreach (string otherIdent in FileTools.List(this.GroupDir))
				if (StringTools.EqualsIgnoreCase(otherIdent, this.Ident) == false)
					return otherIdent;

			return null;
		}

		private string GetOtherSession(string otherIdentDir)
		{
			foreach (string otherSession in FileTools.List(otherIdentDir))
			{
				string otherSessionDir = Path.Combine(otherIdentDir, otherSession);
				string connectFile = Path.Combine(otherSessionDir, "_connect");

				if (FileTools.ExistFile(connectFile))
					continue;

				this.OtherProcAliveMtx = new MutexObject(this.OtherSession + "_PA");

				if (this.CheckDisconnected(otherSessionDir))
				{
					this.OtherProcAliveMtx.Close();
					continue;
				}
				if (this.ListenFlag)
				{
					string listenFile = Path.Combine(otherSessionDir, "_listen");

					if (FileTools.ExistFile(listenFile))
					{
						this.OtherProcAliveMtx.Close();
						continue;
					}
				}
				return otherSession;
			}
			return null;
		}

		public void Send(QueueData<SubBlock> sendData)
		{
			using (this.Mtx.Section())
			{
				if (this.IsDisconnected_nx())
					return;

				for (int index = 0; ; index++)
				{
					string dataFile = Path.Combine(this.OtherSessionDir, "" + index);

					if (FileTools.ExistFile(dataFile) == false)
					{
						FileTools.WriteAllBytes(dataFile, sendData);
						break;
					}
				}
			}
			this.OtherEv.Set();
		}

		private QueueData<byte[]> RecvDataQueue = new QueueData<byte[]>();
		private int RecvDisconCount;

		public byte[] Recv(int millis)
		{
			if (this.RecvDataQueue.GetCount() == 0)
			{
				using (this.Mtx.Section())
				{
					this.TryRecv();

					if (this.RecvDataQueue.GetCount() == 0)
					{
						if (this.IsDisconnected_nx())
						{
							if (this.RecvDisconCount < 10)
							{
								this.RecvDisconCount++;
							}
							else
							{
								using (this.Mtx.Inverse())
								{
									Thread.Sleep(Math.Min(millis, 200));
								}
							}
							return null; // データ無し
						}
						using (this.Mtx.Inverse())
						{
							this.Ev.WaitOne();
						}
						this.TryRecv();
					}
				}
			}
			return this.RecvDataQueue.Poll(null); // null == データ無し
		}

		private void TryRecv()
		{
			for (int index = 0; ; index++)
			{
				string dataFile = Path.Combine(this.SessionDir, "" + index);

				if (FileTools.ExistFile(dataFile) == false)
					break;

				this.RecvDataQueue.Add(File.ReadAllBytes(dataFile));
				FileTools.DeleteFile(dataFile);
			}
		}

		public bool IsDisconnected()
		{
			using (this.Mtx.Section())
			{
				return FileTools.ExistDir(this.OtherSessionDir) == false;
			}
		}

		private bool IsDisconnected_nx()
		{
			if (FileTools.ExistDir(this.OtherSessionDir) == false)
				return true;

			return this.CheckDisconnected(this.OtherSessionDir);
		}

		private bool CheckDisconnected(string otherSessionDir)
		{
			if (this.CD_IsDisconnected(otherSessionDir))
			{
				FileTools.DeleteDir(otherSessionDir, true);
				return true;
			}
			return false;
		}

		private bool CD_IsDisconnected(string otherSessionDir)
		{
			if (this.OtherProcAliveMtx.WaitOne(0))
			{
				this.OtherProcAliveMtx.Release();
				return true;
			}

#if false // test
			{
				string pidFile = Path.Combine(otherSessionDir, "_PID");

				if (FileTools.ExistFile(pidFile) == false)
					return true;

				int pid = IntTools.Read(File.ReadAllBytes(pidFile));

				if (SystemTools.IsProcessAlive(pid) == false)
					return true;
			}
#endif

			return false;
		}

		public void Disconnect()
		{
			using (this.Mtx.Section())
			{
				FileTools.DeleteDir(this.SessionDir, true);
				FileTools.DeleteDir(this.IdentDir);
				FileTools.DeleteDir(this.GroupDir);
				FileTools.DeleteDir(this.CommonDir);
			}
			this.OtherEv.Set();
			this.OtherEv.Close();
			this.OtherProcAliveMtx.Close();
		}

		public void Close()
		{
			this.Mtx.Close();
			this.Ev.Close();
			this.ProcAliveMtx.Release();
			this.ProcAliveMtx.Close();
		}
	}
}
