namespace GBMU.Core;

public class JOYPHook : IMemoryHook {
	public byte? OnReadByte(byte[] memory, ushort address) {
		return null;
	}

	public bool OnWriteByte(byte[] memory, ushort address, byte value) {
		if (address != Memory.P1.Address)
			return false;

		byte currentP1 = MemoryUtils.ReadByte(memory, Memory.P1.Address);
		byte newP1 = (byte)((value & 0xF0) | (currentP1 & 0x0F)); // Only the 4 upper bits of P1 are writable
		MemoryUtils.WriteByte(memory, Memory.P1.Address, newP1);

		return true;
	}

	public ushort? OnReadWord(byte[] memory, ushort address) {
		return null;
	}

	public bool OnWriteWord(byte[] memory, ushort address, ushort value) {
		return false;
	}
}
