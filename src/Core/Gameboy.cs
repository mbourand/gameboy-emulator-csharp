using GBMU.Core;

namespace GBMU;

public class Gameboy
{
	public Cartridge cartridge { get; private set; }
	public Memory memory { get; private set; }
	public CPU cpu { get; private set; }

	public Gameboy(System.IO.Stream romStream)
	{
		cartridge = new Cartridge(romStream);
		memory = new Memory(cartridge);
		cpu = new CPU(memory);
	}

	public void Update(float deltaTime)
	{
		cpu.Cycle();
	}
}