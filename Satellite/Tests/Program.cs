using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Satellite.Tools;
using System.Diagnostics;
using Charlotte.Satellite;
using System.Threading;
using Charlotte.Flowertact;
using Charlotte.Htt;
using Charlotte.Htt.Response;

namespace Charlotte
{
	class Program
	{
		static void Main(string[] args)
		{
			try
			{
				Main2();
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}
			Console.WriteLine("\\e");
			//Console.ReadLine();
		}

		private static void Main2()
		{
			HttServer.Perform(new Test01());
		}

		private class Test01 : HttService
		{
			public bool Interlude()
			{
				while (Console.KeyAvailable)
				{
					ConsoleKeyInfo cki = Console.ReadKey(true);

					if (cki.KeyChar == (char)27) // escape
						return false;

					Console.WriteLine("Press ESCAPE to exit.");
				}
				return true;
			}

			public HttResponse Service(HttRequest req)
			{
				return new HttResHtml("<h1>200</h1>");
			}
		}
	}
}
