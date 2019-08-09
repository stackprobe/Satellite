using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Charlotte.Satellite.Tools
{
	public static class IntTools
	{
		public static byte[] ToBytes(int value)
		{
			byte[] block = new byte[4];
			Write(block, 0, value);
			return block;
		}

		/// <summary>
		/// little endian
		/// </summary>
		/// <param name="block"></param>
		/// <param name="wPos"></param>
		/// <param name="value"></param>
		public static void Write(byte[] block, int wPos, int value)
		{
			block[wPos + 0] = (byte)((value >> 0) & 0xff);
			block[wPos + 1] = (byte)((value >> 8) & 0xff);
			block[wPos + 2] = (byte)((value >> 16) & 0xff);
			block[wPos + 3] = (byte)((value >> 24) & 0xff);
		}

		/// <summary>
		/// little endian
		/// </summary>
		/// <param name="block"></param>
		/// <param name="rPos"></param>
		/// <returns></returns>
		public static int Read(byte[] block, int rPos = 0)
		{
			return
				((int)block[rPos + 0] << 0) |
				((int)block[rPos + 1] << 8) |
				((int)block[rPos + 2] << 16) |
				((int)block[rPos + 3] << 24);
		}
	}
}
