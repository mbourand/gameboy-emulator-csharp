namespace GBMU.Core;

public class MemoryKeyPoint {
	public ushort Address;
	public ushort Size;

	public MemoryKeyPoint(ushort addr, ushort size) {
		Address = addr;
		Size = size;
	}

	public ushort EndAddress => (ushort)(Address + Size - 1);

	public bool IsInRange(ushort address) => address >= Address && address <= EndAddress;
	public ushort GetOffset(ushort address) => (ushort)(address - Address);
}