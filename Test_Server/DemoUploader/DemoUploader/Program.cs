﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Htt;

namespace DemoUploader
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Press ESCAPE to stop the server.");

			HttServer.Perform(new DemoUploaderService());
		}
	}
}
