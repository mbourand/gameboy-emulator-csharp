using System;
using System.Runtime.CompilerServices;
using GBMU;

namespace GBMU;

public static class MemoryUtils {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte ReadByte(this byte[] memory, uint addr) => memory[addr];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ushort ReadWord(this byte[] memory, uint addr) => ByteUtils.FlipEndian(BitConverter.ToUInt16(memory, (int)addr));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteByte(this byte[] memory, uint addr, byte value) => memory[addr] = value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteWord(this byte[] memory, uint addr, ushort value) {
        var flipped = ByteUtils.FlipEndian(value);
        memory[addr] = ByteUtils.HighByte(flipped);
        memory[addr + 1] = ByteUtils.LowByte(flipped);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte RedirectedReadByte(this byte[] memory, uint addr, uint fromAddr, uint toAddr) => ReadByte(memory, Redirect(addr, fromAddr, toAddr));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ushort RedirectedReadWord(this byte[] memory, uint addr, uint fromAddr, uint toAddr) => ReadWord(memory, Redirect(addr, fromAddr, toAddr));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RedirectedWriteByte(this byte[] memory, uint addr, uint fromAddr, uint toAddr, byte value) => WriteByte(memory, Redirect(addr, fromAddr, toAddr), value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RedirectedWriteWord(this byte[] memory, uint addr, uint fromAddr, uint toAddr, ushort value) => WriteWord(memory, Redirect(addr, fromAddr, toAddr), value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint Redirect(uint addr, uint fromAddr, uint toAddr) => toAddr + (addr - fromAddr);
}
