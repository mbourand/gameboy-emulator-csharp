namespace GBMU.Core;

public enum JoypadButton {
	A = 0b0000_0001,
	B = 0b0000_0010,
	Select = 0b0000_0100,
	Start = 0b0000_1000,
	Right = 0b0000_0001,
	Left = 0b0000_0010,
	Up = 0b0000_0100,
	Down = 0b0000_1000,
}

public class Joypad {
	private readonly Memory _memory;

	public Joypad(Memory memory) {
		_memory = memory;
	}

	public void RequireButtonPress(JoypadButton button, bool isKeypad, bool pressed) {
		byte p1 = _memory.InternalReadByte(Memory.P1.Address);
		JoypadState joypadState = new(p1);

		if (!joypadState.IsListeningToActionButtons && !isKeypad)
			return;
		if (!joypadState.IsListeningToDirectionButtons && isKeypad)
			return;

		// 0 = pressed, 1 = not pressed
		if (pressed) {
			p1 &= (byte)~button;
			_memory.RequestInterrupt(Interrupt.Joypad);
		} else {
			p1 |= (byte)button;
		}

		_memory.InternalWriteByte(Memory.P1.Address, p1);
	}
}
