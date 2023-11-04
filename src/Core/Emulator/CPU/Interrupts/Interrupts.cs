namespace GBMU.Core;

public enum Interrupt {
	VBlank = 0b00000001,
	LCDStat = 0b00000010,
	Timer = 0b00000100,
	Serial = 0b00001000,
	Joypad = 0b00010000
}