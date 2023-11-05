namespace GBMU.Core;

public class OAMFlags {
	private readonly byte _value;

	public bool IsBehindBackground => (_value & 0b10000000) != 0;
	public bool IsFlippedY => (_value & 0b01000000) != 0;
	public bool IsFlippedX => (_value & 0b00100000) != 0;
	public MemoryKeyPoint Palette => (_value & 0b00010000) == 0 ? Memory.OBP0 : Memory.OBP1;

	public OAMFlags(byte value) {
		_value = value;
	}
}