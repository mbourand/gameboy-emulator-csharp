namespace GBMU.Core;

public enum JoypadButton {
    AOrRight = 0b0000_0001,
    BOrLeft = 0b0000_0010,
    SelectOrUp = 0b0000_0100,
    StartOrDown = 0b0000_1000,
}

public class Joypad {
    private readonly Memory _memory;

    public Joypad(Memory memory) {
        _memory = memory;
    }

    public void RequireButtonPress(JoypadButton button, bool pressed) {
        byte p1 = _memory.InternalReadByte(Memory.P1.Address);

        // 0 = pressed, 1 = not pressed
        if (pressed) {
            var wasPressed = (p1 & (byte)button) == 0;

            p1 &= (byte)~button;
            if (!wasPressed) {
                _memory.RequestInterrupt(Interrupt.Joypad);
            }
        } else {
            p1 |= (byte)button;
        }

        _memory.InternalWriteByte(Memory.P1.Address, p1);
    }
}
