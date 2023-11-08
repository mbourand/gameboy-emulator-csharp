namespace GBMU.Core;

public enum TACClockSpeed {
	Hz4096 = 0b00,
	Hz262144 = 0b01,
	Hz65536 = 0b10,
	Hz16384 = 0b11
}

public enum TACFlags {
	TimerEnabled = 0b100,
	ClockSpeed = 0b11
}

public class TAC {
	private readonly byte _value;
	public static readonly ushort[] ClockSpeedDividers = new ushort[] { 1024, 16, 64, 256 };

	public bool IsTimerEnabled => (_value & (byte)TACFlags.TimerEnabled) != 0;
	public ushort ClockSpeedWatchBit => (ushort)(ClockSpeedDividers[_value & (byte)TACFlags.ClockSpeed] >> 1);

	public TAC(byte value) {
		_value = value;
	}
}