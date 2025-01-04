using NUnit.Framework;
using System.Runtime.InteropServices;

namespace TerrariaServerAPI.Tests;

public class ServerInitTests : BaseTest
{
	[Test]
	public void EnsureBoots()
	{
		EnsureInitialised();
	}

	[Test]
	public void EnsureRuntimeDetours()
	{
		// Platform exclude doesnt support arm64, so manual check it is...
		if (RuntimeInformation.ProcessArchitecture == Architecture.Arm64)
			Assert.Ignore("Test is not supported on ARM64 architecture.");

		TestContext.Out.WriteLine($"Test architecture {RuntimeInformation.ProcessArchitecture}");

		bool invoked = false;

		On.Terraria.Main.hook_DedServ callback = (orig, self) =>
		{
			invoked = true;
			// DedServ typically requires input, so no need to continue execution
		};
		On.Terraria.Main.DedServ += callback;

		global::TerrariaApi.Server.Program.Main([]);

		On.Terraria.Main.DedServ -= callback;

		Assert.That(invoked, Is.True);
	}
}
