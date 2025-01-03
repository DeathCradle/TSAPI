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
		TestContext.Out.WriteLine($"Test architecture {RuntimeInformation.ProcessArchitecture}");

		if (!_initialized)
		{
			AutoResetEvent are = new(false);
			Exception? error = null;
			HookEvents.HookDelegate<global::Terraria.Main, HookEvents.Terraria.Main.DedServEventArgs> cb = (instance, args) =>
			{
				instance.Initialize();
				are.Set();
				_initialized = true;
			};
			HookEvents.Terraria.Main.DedServ += cb;

			global::TerrariaApi.Server.Program.Main([]);

			_initialized = are.WaitOne(TimeSpan.FromSeconds(30));

			HookEvents.Terraria.Main.DedServ -= cb;

			Assert.That(_initialized, Is.True);
			Assert.That(error, Is.Null);
		}
	}
}
