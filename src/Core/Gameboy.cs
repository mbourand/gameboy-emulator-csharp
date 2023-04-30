using GBMU.Core;

namespace GBMU;

public class Gameboy
{
	private Cartridge _cartridge;
	private Memory _memory;
	private CPU _cpu;

	public Gameboy(System.IO.Stream romStream)
	{
		_cartridge = new Cartridge(romStream);
		_memory = new Memory(_cartridge);
		_cpu = new CPU();
	}

	public void Update(float deltaTime)
	{
	}
}