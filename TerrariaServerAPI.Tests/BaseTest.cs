using NUnit.Framework;
using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace TerrariaServerAPI.Tests;

public class BaseTest
{
	private static bool _initialized;

	[OneTimeSetUp]
	public void EnsureInitialised()
	{
		if (!_initialized)
		{
			var are = new AutoResetEvent(false);
			Exception? error = null;
			HookEvents.HookDelegate<global::Terraria.Main,HookEvents.Terraria.Main. DedServEventArgs> cb = (instance, args) =>
			{
				instance.Initialize();
				are.Set();
				_initialized = true;
				Console.WriteLine($"Server init process successful for architecture {RuntimeInformation.ProcessArchitecture}");
			};
			HookEvents.Terraria.Main.DedServ += cb;

			global::TerrariaApi.Server.Program.Main(new string[] { });

			_initialized = are.WaitOne(TimeSpan.FromSeconds(30));

			HookEvents.Terraria.Main.DedServ -= cb;

			Assert.That(_initialized, Is.True);
			Assert.That(error, Is.Null);
		}
	}
}
