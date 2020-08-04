using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Satellite.Tools;

namespace DemoUploader
{
	public class MultiPartContent
	{
		private Dictionary<string, string> Extensions = DictionaryTools.CreateIgnoreCase<string>();
		public byte[] Data;

		public void Load(byte[] body)
		{
			int index = Utils.IndexOf(body, new byte[] { 0x0d, 0x0a }); // find CR-LF

			if (index == -1)
				throw new Exception();

			// HACK: This code is inadequate

			string line = Encoding.UTF8.GetString(Utils.GetRange(body, 0, index)); // maybe Content-Disposition:

			foreach (string entry in line.Split(":".ToArray(), 2)[1].Split(';'))
			{
				string[] tokens = entry.Split("=".ToArray(), 2);

				if (tokens.Length == 2)
				{
					string key = tokens[0].Trim();
					string value = tokens[1].Trim();

					if (value.StartsWith("\""))
						value = value.Substring(1);

					if (value.EndsWith("\""))
						value = value.Substring(0, value.Length - 1);

					this.Extensions.Add(key, value);
				}
			}
			index += 2; // skip CR-LF

			while (body[index] != 0x0d)
			{
				index = Utils.IndexOf(body, new byte[] { 0x0d, 0x0a }, index); // find CR-LF

				if (index == -1)
					throw new Exception();

				index += 2; // skip CR-LF
			}
			index += 2; // skip CR-LF

			this.Data = Utils.GetRange(body, index, (body.Length - index) - 2); // cut CR-LF
		}

		public string Name
		{
			get { return this.Extensions["name"]; }
		}

		public string FileName
		{
			get { return this.Extensions["filename"]; }
		}
	}
}
