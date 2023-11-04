namespace GBMU.Core;

public class InterruptByte {
	private readonly byte _value;

	public bool HasVBlank => (_value & (byte)Interrupt.VBlank) != 0;
	public bool HasLCDStat => (_value & (byte)Interrupt.LCDStat) != 0;
	public bool HasTimer => (_value & (byte)Interrupt.Timer) != 0;
	public bool HasSerial => (_value & (byte)Interrupt.Serial) != 0;
	public bool HasJoypad => (_value & (byte)Interrupt.Joypad) != 0;

	public bool IsEmpty => _value == 0;

	public InterruptByte(byte value) {
		_value = value;
	}
}