using System.IO;
using GBMU;

namespace Tests;

public static class TestUtils
{
	public static Gameboy InitGameboy()
	{
		var stream = File.OpenRead("./resources/roms/Tetris.gb");
		var gameboy = new Gameboy(stream);
		stream.Close();

		return gameboy;
	}
}