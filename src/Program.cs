using GBMU;
using GBMU.UI;

// using var game = new GMBUWindow();
// game.Run();

var gameboy = new Gameboy(System.IO.File.OpenRead("resources/roms/Tetris.gb"));
gameboy.Update(0);
gameboy.cpu.DebugString();