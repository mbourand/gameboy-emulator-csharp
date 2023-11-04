using System;
using GBMU;

namespace GBMU;

public static class MemoryUtils {
	public static byte ReadByte(this byte[] memory, uint addr) => memory[addr];
	public static ushort ReadWord(this byte[] memory, uint addr) => ByteUtils.FlipEndian(BitConverter.ToUInt16(memory, (int)addr));
	public static void WriteByte(this byte[] memory, uint addr, byte value) => memory[addr] = value;
	public static void WriteWord(this byte[] memory, uint addr, ushort value) {
		var flipped = ByteUtils.FlipEndian(value);
		memory[addr] = ByteUtils.HighByte(flipped);
		memory[addr + 1] = ByteUtils.LowByte(flipped);
	}

	public static byte RedirectedReadByte(this byte[] memory, uint addr, uint fromAddr, uint toAddr) => ReadByte(memory, Redirect(addr, fromAddr, toAddr));
	public static ushort RedirectedReadWord(this byte[] memory, uint addr, uint fromAddr, uint toAddr) => ReadWord(memory, Redirect(addr, fromAddr, toAddr));
	public static void RedirectedWriteByte(this byte[] memory, uint addr, uint fromAddr, uint toAddr, byte value) => WriteByte(memory, Redirect(addr, fromAddr, toAddr), value);
	public static void RedirectedWriteWord(this byte[] memory, uint addr, uint fromAddr, uint toAddr, ushort value) => WriteWord(memory, Redirect(addr, fromAddr, toAddr), value);

	private static uint Redirect(uint addr, uint fromAddr, uint toAddr) => toAddr + (addr - fromAddr);
}