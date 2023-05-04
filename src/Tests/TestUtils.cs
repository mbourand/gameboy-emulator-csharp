using System.IO;
using GBMU;
using GBMU.Core;

namespace Tests;

public static class TestUtils
{
	public static Gameboy InitGameboy()
	{
		var stream = File.OpenRead("./resources/roms/Tetris.gb");
		var gameboy = new Gameboy(stream);
		stream.Close();

		gameboy.cpu.SetFlag(CPUFlag.ZERO, false);
		gameboy.cpu.SetFlag(CPUFlag.N_SUBTRACT, false);
		gameboy.cpu.SetFlag(CPUFlag.HALF_CARRY, false);
		gameboy.cpu.SetFlag(CPUFlag.CARRY, false);

		return gameboy;
	}
}