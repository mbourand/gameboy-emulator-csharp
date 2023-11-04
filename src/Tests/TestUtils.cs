using System.IO;
using GBMU;
using GBMU.Core;

namespace Tests;

public static class TestUtils {
	public static Gameboy InitGameboy() {
		var stream = File.OpenRead("./resources/roms/Tetris.gb");
		var gameboy = new Gameboy(stream);
		stream.Close();

		gameboy.CPU.SetFlag(CPUFlag.Zero, false);
		gameboy.CPU.SetFlag(CPUFlag.NSubtract, false);
		gameboy.CPU.SetFlag(CPUFlag.HalfCarry, false);
		gameboy.CPU.SetFlag(CPUFlag.Carry, false);

		return gameboy;
	}
}