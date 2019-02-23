using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DemoUploader
{
	public class MultiPartFormData
	{
		private List<MultiPartContent> Contents = new List<MultiPartContent>();

		public void Load(byte[] body)
		{
			int index = Utils.IndexOf(body, new byte[] { 0x0d, 0x0a }); // find CR-LF

			if (index == -1)
				throw new Exception();

			byte[] boundary = Utils.GetRange(body, 0, index);
			index += 2; // skip CR-LF

			for (; ; )
			{
				int next = Utils.IndexOf(body, boundary, index);

				if (next == -1)
					throw new Exception();

				byte[] contentBody = Utils.GetRange(body, index, next - index);

				MultiPartContent content = new MultiPartContent();
				content.Load(contentBody);
				this.Contents.Add(content);

				index = next + boundary.Length;

				/*
				 * boundary + CR-LF  -->  continue
				 * boundary + "--"   -->  end
				 *
				 */
				if (body[index] == 0x2d)
					break;

				index += 2; // skip CR-LF
			}
		}

		public MultiPartContent Get(string name)
		{
			foreach (MultiPartContent content in this.Contents)
				if (content.Name == name)
					return content;

			return null;
		}
	}
}
