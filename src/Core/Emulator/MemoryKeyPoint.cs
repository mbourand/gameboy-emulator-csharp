namespace GBMU.Core;

public class MemoryKeyPoint
{
	public uint addr { get; set; }
	public uint size { get; set; }

	public MemoryKeyPoint(uint addr, uint size)
	{
		this.addr = addr;
		this.size = size;
	}

	public uint EndAddr => addr + size - 1;

	public bool IsInRange(uint addr) => addr >= this.addr && addr <= EndAddr;
	public uint GetOffset(uint addr) => addr - this.addr;
}