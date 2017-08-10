using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Charlotte.Satellite.Tools
{
	public class Serializer
	{
		public static readonly byte KIND_NULL = Encoding.ASCII.GetBytes("N")[0];
		public static readonly byte KIND_BYTES = Encoding.ASCII.GetBytes("B")[0];
		public static readonly byte KIND_MAP = Encoding.ASCII.GetBytes("M")[0];
		public static readonly byte KIND_LIST = Encoding.ASCII.GetBytes("L")[0];
		public static readonly byte KIND_STRING = Encoding.ASCII.GetBytes("S")[0];

		private QueueData<SubBlock> Buff = new QueueData<SubBlock>();

		public Serializer()
		{ }

		public Serializer(object obj)
		{
			this.Add(obj);
		}

		public void Add(object obj)
		{
			if (obj == null)
			{
				this.AddByte(KIND_NULL);
			}
			else if (obj is byte[])
			{
				this.AddByte(KIND_BYTES);
				this.AddBlock((byte[])obj);
			}
			else if (obj is ObjectMap)
			{
				ObjectMap om = (ObjectMap)obj;

				this.AddByte(KIND_MAP);
				this.AddInt(om.GetCount());

				foreach (string key in om.GetKeys())
				{
					this.Add(key);
					this.Add(om.GetValue(key));
				}
			}
			else if (obj is ObjectList)
			{
				ObjectList ol = (ObjectList)obj;

				this.AddByte(KIND_LIST);
				this.AddInt(ol.GetCount());

				foreach (object value in ol.GetList())
				{
					this.Add(value);
				}
			}
			else if (obj is string)
			{
				this.AddByte(KIND_STRING);
				this.AddBlock(Encoding.UTF8.GetBytes((string)obj));
			}
			else
			{
				throw new Exception("invalid object type: " + obj.GetType());
			}
		}

		private void AddByte(byte chr)
		{
			this.Buff.Add(new SubBlock(new byte[] { chr }));
		}

		private void AddBlock(byte[] block)
		{
			this.AddInt(block.Length);
			this.Buff.Add(new SubBlock(block));
		}

		/// <summary>
		/// big endian
		/// </summary>
		/// <param name="value"></param>
		private void AddInt(int value)
		{
			byte[] block = new byte[4];

			block[0] = (byte)((value >> 24) & 0xff);
			block[1] = (byte)((value >> 16) & 0xff);
			block[2] = (byte)((value >>  8) & 0xff);
			block[3] = (byte)((value >>  0) & 0xff);

			this.Buff.Add(new SubBlock(block));
		}

		public QueueData<SubBlock> GetBuff()
		{
			return this.Buff;
		}
	}
}
