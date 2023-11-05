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

	public bool IsTimerEnabled => (_value & (byte)TACFlags.TimerEnabled) != 0;
	public float ClockSpeed => GetClockSpeed((TACClockSpeed)(_value & (byte)TACFlags.ClockSpeed));
	public float CycleDuration => 1 / ClockSpeed;

	private static float GetClockSpeed(TACClockSpeed clockSpeed) {
		return clockSpeed switch {
			TACClockSpeed.Hz4096 => CPU.ClockSpeed / 1024f,
			TACClockSpeed.Hz262144 => CPU.ClockSpeed / 16f,
			TACClockSpeed.Hz65536 => CPU.ClockSpeed / 64f,
			TACClockSpeed.Hz16384 => CPU.ClockSpeed / 256f,
			_ => throw new System.NotImplementedException()
		};
	}

	public TAC(byte value) {
		_value = value;
	}
}