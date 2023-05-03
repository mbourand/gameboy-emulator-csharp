namespace GBMU.Core;

public class MemoryKeyPoint
{
	public ushort addr { get; set; }
	public ushort size { get; set; }

	public MemoryKeyPoint(ushort addr, ushort size)
	{
		this.addr = addr;
		this.size = size;
	}

	public ushort EndAddr => (ushort)(addr + size - 1);

	public bool IsInRange(ushort addr) => addr >= this.addr && addr <= EndAddr;
	public ushort GetOffset(ushort addr) => (ushort)(addr - this.addr);
}