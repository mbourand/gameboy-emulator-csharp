using GBMU.Core;

namespace GBMU;

public class Gameboy
{
	private Cartridge _cartridge;
	private Memory _memory;
	public CPU cpu { get; private set; }

	public Gameboy(System.IO.Stream romStream)
	{
		_cartridge = new Cartridge(romStream);
		_memory = new Memory(_cartridge);
		cpu = new CPU(_memory);
	}

	public void Update(float deltaTime)
	{
		cpu.Cycle();
	}
}