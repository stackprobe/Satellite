using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Satellite.Tools;
using System.IO;

namespace Charlotte.Flowertact.Tools
{
	public class PostOfficeBox
	{
		private const string IDENT_PREFIX = "Fortewave_{d8600f7d-1ff4-47f3-b1c9-4b5aa15b6461}_"; // shared_uuid@g
		private string _ident;
		private MutexObject _mutex;
		private NamedEventObject _messagePostEvent;
		private string _messageDir;

		public PostOfficeBox(String ident)
		{
			_ident = IDENT_PREFIX + SecurityTools.GetSHA512_128String(ident);
			_mutex = new MutexObject(_ident + "_m");
			_messagePostEvent = new NamedEventObject(_ident + "_e");
			_messageDir = Path.Combine(SystemTools.GetTmp(), _ident);
		}

		public void Clear()
		{
			using (_mutex.Section())
			{
				FileTools.DeleteDir(_messageDir, true);
			}
		}

		public void Send(QueueData<SubBlock> sendData)
		{
			using (_mutex.Section())
			{
				this.GetMessageRange();
				this.TryRenumber();

				if (this.GMR_LastNo == -1)
					FileTools.CreateDir(_messageDir);

				FileTools.WriteAllBytes(
						Path.Combine(_messageDir, StringTools.ZPad(this.GMR_LastNo + 1, 4)),
						sendData
						);
			}
			_messagePostEvent.Set();
		}

		public byte[] Recv(int millis)
		{
			byte[] recvData = this.TryRecv();

			if (recvData == null)
			{
				_messagePostEvent.WaitOne(millis);
				recvData = this.TryRecv();
			}
			return recvData;
		}

		private byte[] TryRecv()
		{
			using (_mutex.Section())
			{
				this.GetMessageRange();

				if (this.GMR_FirstNo != -1)
				{
					String file = Path.Combine(_messageDir, StringTools.ZPad(this.GMR_FirstNo, 4));
					byte[] recvData = File.ReadAllBytes(file);

					FileTools.DeleteFile(file);

					if (this.GMR_FirstNo == this.GMR_LastNo)
						FileTools.DeleteDir(_messageDir);

					return recvData;
				}
			}
			return null;
		}

		private int GMR_FirstNo;
		private int GMR_LastNo;

		private void GetMessageRange()
		{
			this.GMR_FirstNo = -1;
			this.GMR_LastNo = -1;

			if (FileTools.ExistDir(_messageDir) == false)
				return;

			List<string> files = FileTools.List(_messageDir);

			if (files.Count == 0)
				return;

			files.Sort(delegate(string a, string b)
			{
				return int.Parse(a) - int.Parse(b);
			});

			this.GMR_FirstNo = int.Parse(files[0]);
			this.GMR_LastNo = int.Parse(files[files.Count - 1]);
		}

		private void TryRenumber()
		{
			if (1000 < this.GMR_FirstNo)
			{
				this.Renumber();
			}
		}

		private void Renumber()
		{
			for (int no = this.GMR_FirstNo; no <= this.GMR_LastNo; no++)
			{
				File.Move(
					Path.Combine(_messageDir, StringTools.ZPad(no, 4)),
					Path.Combine(_messageDir, StringTools.ZPad(no - this.GMR_FirstNo, 4))
					);
			}
			this.GMR_LastNo -= this.GMR_FirstNo;
			this.GMR_FirstNo = 0;
		}

		public void Pulse()
		{
			_messagePostEvent.Set();
		}

		public void Close()
		{
			_mutex.Close();
			_messagePostEvent.Close();
		}
	}
}
