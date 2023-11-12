namespace GBMU.Core;

public class ECHOHook : IMemoryHook {
	public byte? OnReadByte(byte[] memory, ushort address) {
		if (Memory.Echo.IsInRange(address))
			return MemoryUtils.RedirectedReadByte(memory, address, Memory.Echo.Address, Memory.WorkRAMBank0.Address);
		return null;
	}

	public bool OnWriteByte(byte[] memory, ushort address, byte value) {
		if (Memory.Echo.IsInRange(address)) {
			MemoryUtils.RedirectedWriteByte(memory, address, Memory.Echo.Address, Memory.WorkRAMBank0.Address, value);
			MemoryUtils.WriteByte(memory, address, value);
			return true;
		}
		return false;
	}

	public ushort? OnReadWord(byte[] memory, ushort address) {
		return null;
	}

	public bool OnWriteWord(byte[] memory, ushort address, ushort value) {
		return false;
	}
}