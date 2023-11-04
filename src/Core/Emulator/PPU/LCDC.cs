namespace GBMU.Core;

public class LCDC {
	public enum LCDCFlags {
		BackgroundAndWindowEnable = 0b0000_0001,
		ObjectEnable = 0b0000_0010,
		ObjectSize = 0b0000_0100,
		BackgroundTileMap = 0b0000_1000,
		BackgroundTileSet = 0b0001_0000,
		WindowEnable = 0b0010_0000,
		WindowTileMap = 0b0100_0000,
		DisplayEnable = 0b1000_0000
	}

	private readonly byte _value;

	public bool IsBackgroundAndWindowEnable => (_value & (byte)LCDCFlags.BackgroundAndWindowEnable) != 0;
	public bool IsObjectEnabled => (_value & (byte)LCDCFlags.ObjectEnable) != 0;
	public bool IsDisplayEnabled => (_value & (byte)LCDCFlags.DisplayEnable) != 0;
	public bool IsWindowEnabled => (_value & (byte)LCDCFlags.WindowEnable) != 0;

	public byte ObjectSize => (byte)((_value & (byte)LCDCFlags.ObjectSize) == 0 ? 8 : 16);
	public ushort BackgroundTileMap => (ushort)((_value & (byte)LCDCFlags.BackgroundTileMap) == 0 ? 0x9800 : 0x9C00);
	public ushort BackgroundTileSet => (ushort)((_value & (byte)LCDCFlags.BackgroundTileSet) == 0 ? 0x9000 : 0x8000);
	public ushort WindowTileMap => (ushort)((_value & (byte)LCDCFlags.WindowTileMap) == 0 ? 0x9800 : 0x9C00);

	public LCDC(byte value) {
		_value = value;
	}
}