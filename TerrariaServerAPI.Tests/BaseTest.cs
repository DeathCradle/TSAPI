using NUnit.Framework;
using System.Runtime.InteropServices;

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
			bool invoked = false;
			HookEvents.HookDelegate<global::Terraria.Main, HookEvents.Terraria.Main.DedServEventArgs> cb = (instance, args) =>
			{
				invoked = true;
				// DedServ typically requires input, so no need to continue execution
				args.ContinueExecution = false;
				// DedServ calls the following method, which is needed for subsequent tests
				instance.Initialize();
			};
			HookEvents.Terraria.Main.DedServ += cb;

			global::TerrariaApi.Server.Program.Main([]);

			HookEvents.Terraria.Main.DedServ -= cb;

			Assert.That(invoked, Is.True);
		}
	}
}
