using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
 
namespace Server.Game
{
	public class GameLogic : JobSerializer
	{
		public static GameLogic Instance { get; } = new GameLogic();
	
		public void Update()
		{
			Flush();

			UserManager.Instance.Update();
		}
	}
}
