﻿using DummyClient.Session;
using ServerCore;
using System;
using System.Net;
using System.Threading;

namespace DummyClient
{
	class Program
	{
		static int DummyClientCount { get; } = 500;

		static void Main(string[] args)
		{
			Thread.Sleep(3000);

			// DNS (Domain Name System)
			string host = Dns.GetHostName();
			IPHostEntry ipHost = Dns.GetHostEntry(host);
			//IPAddress ipAddr = ipHost.AddressList[1]; (수업코드)
			//IPAddress ipAddr = IPAddress.Parse("192.168.219.109");
			IPAddress ipAddr = IPAddress.Parse("192.168.123.103");
			IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

			Connector connector = new Connector();

			connector.Connect(endPoint,
				() => { return SessionManager.Instance.Generate(); },
				Program.DummyClientCount);

			while (true)
			{
				Thread.Sleep(10000);
			}
		}
	}
}
