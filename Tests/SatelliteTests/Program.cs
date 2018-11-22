using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Charlotte.Satellite;
using System.Diagnostics;
using System.Threading;
using Charlotte.Flowertact;

namespace SatelliteTests
{
	class Program
	{
		static void Main(string[] args)
		{
			try
			{
				new Program().Main2(args);
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}
			Console.WriteLine("### 終了しました。エンターキーを押してね ###");
			Console.ReadLine();
		}

		private void Main2(string[] args)
		{
			if (1 <= args.Length)
			{
				if (args[0] == "/T1C")
				{
					Test01_Client(args[1]);
					return;
				}
				if (args[0] == "/T1S")
				{
					Test01_Server();
					return;
				}
				if (args[0] == "/T2C")
				{
					Test02_Client();
					return;
				}
				if (args[0] == "/T2S")
				{
					Test02_Server();
					return;
				}
			}
			Test01();
			//Test02();
			//Test02_b();
			//Test03();
		}

		private static readonly string SELF_FILE = @"C:\Dev\Main2\Satellite\Tests\SatelliteTests\bin\Debug\SatelliteTests.exe";

#if true // single ver
		private void Test01()
		{
			Process.Start(SELF_FILE, "/T1S");
			Process.Start(SELF_FILE, "/T1C " + 1);
		}

		private void Test01_Client(string c)
		{
			using (Satellizer stllzr = new Satellizer("TEST_GROUP", "CLIENT"))
			{
				for (int d = 0; d < 20 * 50; d++)
				{
					string testData = "TEST_STRING_" + c + "_" + d;

					Console.WriteLine("testData: " + testData);

					while (stllzr.Connect(2000) == false)
					{
						Console.WriteLine("接続ナシ_リトライします。" + c);
					}
					stllzr.Send(testData);

					for (; ; )
					{
						string retData = (string)stllzr.Recv(2000);

						if (retData != null)
						{
							string assumeData = testData + "_RET";

							Console.WriteLine("retData: " + retData);
							Console.WriteLine("assumeData: " + assumeData);

							if (retData != assumeData)
								throw new Exception("想定したデータと違う。");

							break;
						}
					}
					stllzr.Disconnect();
				}
			}
		}

#elif true // mlt-proc ver
		private void Test01()
		{
			Process.Start(SELF_FILE, "/T1S");

			for (int cc = 0; cc < 20; cc++)
			{
				Process.Start(SELF_FILE, "/T1C " + cc);
			}
		}

		private void Test01_Client(string c)
		{
			using (Satellizer stllzr = new Satellizer("TEST_GROUP", "CLIENT"))
			{
				for (int d = 0; d < 50; d++)
				{
					string testData = "TEST_STRING_" + c + "_" + d;

					Console.WriteLine("testData: " + testData);

					while (stllzr.Connect(2000) == false)
					{
						Console.WriteLine("接続ナシ_リトライします。" + c);
					}
					stllzr.Send(testData);

					for (; ; )
					{
						string retData = (string)stllzr.Recv(2000);

						if (retData != null)
						{
							string assumeData = testData + "_RET";

							Console.WriteLine("retData: " + retData);
							Console.WriteLine("assumeData: " + assumeData);

							if (retData != assumeData)
								throw new Exception("想定したデータと違う。");

							break;
						}
					}
					stllzr.Disconnect();
				}
			}
		}

#else // mlt-th ver
		private void Test01()
		{
			Process.Start(SELF_FILE, "/T1S");
			Test01_Client(null);
		}

		private void Test01_Client(string dummy)
		{
			Thread[] thl = new Thread[20];

			for (int cc = 0; cc < 20; cc++)
			{
				int c = cc;

				thl[c] = new Thread((ThreadStart)delegate
				{
					using (Satellizer stllzr = new Satellizer("TEST_GROUP", "CLIENT"))
					{
						for (int d = 0; d < 50; d++)
						{
							string testData = "TEST_STRING_" + c + "_" + d;

							Console.WriteLine("testData: " + testData);

							while (stllzr.Connect(2000) == false)
							{
								Console.WriteLine("接続ナシ_リトライします。" + c);
							}
							stllzr.Send(testData);

							for (; ; )
							{
								string retData = (string)stllzr.Recv(2000);

								if (retData != null)
								{
									string assumeData = testData + "_RET";

									Console.WriteLine("retData: " + retData);
									Console.WriteLine("assumeData: " + assumeData);

									if (retData != assumeData)
										throw new Exception("想定したデータと違う。");

									break;
								}
							}
							stllzr.Disconnect();
						}
					}
				});
			}
			for (int c = 0; c < 20; c++)
				thl[c].Start();

			for (int c = 0; c < 20; c++)
				thl[c].Join();
		}
#endif

		private void Test01_Server()
		{
			Satellizer.Listen("TEST_GROUP", "SERVER", 2000, new Server_Test01());
		}

		private class Server_Test01 : Satellizer.Server
		{
			private object SYNCROOT = new object();
			private int _c;

			public bool Interlude()
			{
				lock (SYNCROOT)
				{
					return _c < 20 * 50;
				}
			}

			public void ServiceTh(Satellizer stllzr)
			{
				for (; ; )
				{
					string retData = (string)stllzr.Recv(2000);

					if (retData != null)
					{
						Console.WriteLine("retData: " + retData);

						stllzr.Send(retData + "_RET");

						lock (SYNCROOT)
						{
							_c++;
						}
						break;
					}
				}
			}
		}

		private void Test02()
		{
			Process.Start(SELF_FILE, "/T2S");
			Test02_Client();
		}

		private Fortewave GetT2Client()
		{
			return new Fortewave("CLIENT", "SERVER");
		}

		private Fortewave GetT2Server()
		{
			return new Fortewave("SERVER", "CLIENT");
		}

		private void Test02_Client()
		{
			using (Fortewave client = GetT2Client())
			{
				for (int c = 0; c < 100; c++)
				{
					Console.WriteLine("c_send: " + c);

					client.Send("TEST_STRING_" + c);
				}
				for (int c = 0; c < 100; c++)
				{
					string assumeRet = "TEST_STRING_" + c + "_RET";

					Console.WriteLine("assumeRet: " + assumeRet);

					for (; ; )
					{
						string ret = (string)client.Recv(2000);

						Console.WriteLine("c_ret: " + ret);

						if (ret != null)
						{
							if (ret != assumeRet)
								throw new Exception("ng");

							break;
						}
					}
				}
			}
		}

		private void Test02_Server()
		{
			using (Fortewave server = GetT2Server())
			{
				for (int c = 0; c < 100; c++)
				{
					for (; ; )
					{
						string ret = (string)server.Recv(2000);

						Console.WriteLine("s_ret: " + ret);

						if (ret != null)
						{
							server.Send(ret + "_RET");
							break;
						}
					}
				}
			}
		}

		private void Test02_b() // リーク
		{
			for (int c = 0; c < 10000; c++)
			{
				using (GetT2Client())
				using (GetT2Server())
				{ }
			}
			for (int c = 0; c < 10000; c++)
			{
				using (Fortewave client = GetT2Client())
				using (Fortewave server = GetT2Server())
				{
					client.Send("TEST_STRING_" + c);
					client.Recv(0);
					server.Send("TEST_STRING_" + c);
					client.Recv(0);
				}
			}
		}

		private void Test03()
		{
			// todo ???
		}
	}
}
