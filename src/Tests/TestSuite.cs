using Tests.Instructions;

namespace Tests;

public static class TestSuite {
	public static readonly Test[] TestList = new Test[] {
		new TestLD(), new TestLDShift(), new TestINC(), new TestDEC(), new TestAND(), new TestOR(), new TestXOR(), new TestCP(), new TestADD(), new TestADC(), new TestSUB()
	};

	public static void Run() {
		foreach (var test in TestList)
			test.Run();
	}
}