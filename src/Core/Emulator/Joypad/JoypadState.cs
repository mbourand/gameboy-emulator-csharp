namespace GBMU.Core;

public enum JoypadListeningBits {
	KeyPad = 0b0001_0000,
	Actions = 0b0010_0000
}

public class JoypadState {
	private readonly byte _value;

	public bool IsListeningToActionButtons => (_value & (byte)JoypadListeningBits.Actions) == 0;
	public bool IsListeningToDirectionButtons => (_value & (byte)JoypadListeningBits.KeyPad) == 0;

	public JoypadState(byte value) {
		_value = value;
	}
}