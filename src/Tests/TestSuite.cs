using Tests.Instructions;

namespace Tests;

public static class TestSuite
{
	public static Test[] TestList = new Test[] {
		new TestLD()
	};

	public static void Run()
	{
		foreach (var test in TestList)
			test.Run();
	}
}