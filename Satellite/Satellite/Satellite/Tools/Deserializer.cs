using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Charlotte.Satellite.Tools
{
	public class Deserializer
	{
		private byte[] Data;
		private int Index;

		public Deserializer(byte[] data)
		{
			this.Data = data;
		}

		private byte ReadByte()
		{
			return this.Data[this.Index++];
		}

		/// <summary>
		/// big endian
		/// </summary>
		/// <returns></returns>
		private int ReadInt()
		{
			byte b1 = this.ReadByte();
			byte b2 = this.ReadByte();
			byte b3 = this.ReadByte();
			byte b4 = this.ReadByte();

			return
				((int)b1 << 24) |
				((int)b2 << 16) |
				((int)b3 << 8) |
				((int)b4 << 0);
		}

		private byte[] ReadBlock(int size)
		{
			byte[] dest = new byte[size];
			Array.Copy(this.Data, this.Index, dest, 0, size);
			this.Index += size;
			return dest;
		}

		private byte[] ReadBlock()
		{
			return this.ReadBlock(this.ReadInt());
		}

		public object Next()
		{
			byte kind = this.ReadByte();

			if (kind == Serializer.KIND_NULL)
			{
				return null;
			}
			if (kind == Serializer.KIND_BYTES)
			{
				return this.ReadBlock();
			}
			if (kind == Serializer.KIND_MAP)
			{
				ObjectMap om = new ObjectMap();
				int size = this.ReadInt();

				for (int index = 0; index < size; index++)
				{
					object key = this.Next();
					object value = this.Next();

					om.Add(key, value);
				}
				return om;
			}
			if (kind == Serializer.KIND_LIST)
			{
				ObjectList ol = new ObjectList();
				int size = this.ReadInt();

				for (int index = 0; index < size; index++)
				{
					ol.Add(this.Next());
				}
				return ol;
			}
			if (kind == Serializer.KIND_STRING)
			{
				return Encoding.UTF8.GetString(this.ReadBlock());
			}
			throw new Exception("kind: " + kind);
		}
	}
}
