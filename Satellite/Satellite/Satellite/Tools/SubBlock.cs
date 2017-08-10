using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Charlotte.Satellite.Tools
{
	public class SubBlock
	{
		public byte[] Block;
		public int StartPos;
		public int Length;

		public SubBlock(byte[] block)
			: this(block, 0)
		{ }

		public SubBlock(byte[] block, int startPos)
			: this(block, startPos, block.Length - startPos)
		{ }

		public SubBlock(byte[] block, int startPos, int length)
		{
			this.Block = block;
			this.StartPos = startPos;
			this.Length = length;
		}
	}
}
