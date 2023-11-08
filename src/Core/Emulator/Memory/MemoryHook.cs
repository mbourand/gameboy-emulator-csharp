namespace GBMU.Core;

public interface IMemoryHook {
	public byte? OnReadByte(byte[] memory, ushort address);
	public bool OnWriteByte(byte[] memory, ushort address, byte value);
	public ushort? OnReadWord(byte[] memory, ushort address);
	public bool OnWriteWord(byte[] memory, ushort address, ushort value);
}