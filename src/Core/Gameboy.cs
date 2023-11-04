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

	public Gameboy(Stream romStream) {
		Cartridge = new Cartridge(romStream);
		Memory = new Memory(Cartridge);
		CPU = new CPU(Memory);
		PPU = new PPU(Memory);
	}

	public void Update(double deltaTime) {
		CPU.Update(deltaTime);
		PPU.Update(deltaTime);
	}

	public void ForceCycle() {
		CPU.Cycle();
		PPU.Cycle();
	}
}