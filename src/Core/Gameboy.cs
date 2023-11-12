using System.IO;
using GBMU.Core;

namespace GBMU;

public class Gameboy {
	public Cartridge Cartridge {
		get; private set;
	}
	public Memory Memory {
		get; private set;
	}
	public CPU CPU {
		get; private set;
	}
	public PPU PPU {
		get; private set;
	}

	public Timers Timers {
		get; private set;
	}

	public Joypad Joypad {
		get; private set;
	}

	public Gameboy(Stream romStream) {
		Cartridge = new Cartridge(romStream);
		Memory = new Memory(Cartridge);
		CPU = new CPU(Memory);
		PPU = new PPU(Memory);
		Timers = new Timers(CPU, Memory);
		Joypad = new Joypad(Memory);

		Memory.RegisterHook(new DIVHook(Timers));
		Memory.RegisterHook(new TIMAHook(Timers));
		Memory.RegisterHook(new TMAHook(Timers));
		Memory.RegisterHook(new JOYPHook());

		IMemoryHook mbc = Cartridge.GetMemoryBankController();
		if (mbc != null)
			Memory.RegisterHook(mbc);
	}

	public void Update(double deltaTime) {
		Timers.Update(deltaTime);
		CPU.Update(deltaTime);
		PPU.Update(deltaTime);
	}
}