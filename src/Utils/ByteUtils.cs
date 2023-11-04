namespace GBMU;

public static class ByteUtils {
	public static ushort FlipEndian(ushort value) => (ushort)((value & 0xFF) << 8 | (value & 0xFF00) >> 8);
	public static byte HighByte(ushort value) => (byte)((value & 0xFF00) >> 8);
	public static byte LowByte(ushort value) => (byte)(value & 0xFF);
	public static byte LowNibble(byte value) => (byte)(value & 0x0F);
	public static byte HighNibble(byte value) => (byte)((value & 0xF0) >> 4);
	public static ushort MakeWord(byte high, byte low) => (ushort)((high << 8) | low);
}