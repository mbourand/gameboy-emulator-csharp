using System;
using GBMU.Core;

namespace GBMU.Core;

public class LCDStatus {
	public enum LCDStatusInterrupts {
		LycLyCoincidenceInt = 0b01000000,
		OAMSearchInt = 0b00100000,
		VBlankInt = 0b00010000,
		HBlankInt = 0b00001000
	}

	public enum LCDStatusModes {
		HBlankMode = 0,
		VBlankMode = 1,
		OAMSearchMode = 2,
		PixelTransferMode = 3,
	}

	public byte Value;

	public bool IsHBlankMode => (Value & (byte)LCDStatusModes.HBlankMode) != 0;
	public bool IsVBlankMode => (Value & (byte)LCDStatusModes.VBlankMode) != 0;
	public bool IsOAMSearchMode => (Value & (byte)LCDStatusModes.OAMSearchMode) != 0;
	public bool IsPixelTransferMode => (Value & (byte)LCDStatusModes.PixelTransferMode) != 0;
	public bool HasLycLyCoincidenceInt => (Value & (byte)LCDStatusInterrupts.LycLyCoincidenceInt) != 0;
	public bool HasOAMSearchInt => (Value & (byte)LCDStatusInterrupts.OAMSearchInt) != 0;
	public bool HasVBlankInt => (Value & (byte)LCDStatusInterrupts.VBlankInt) != 0;
	public bool HasHBlankInt => (Value & (byte)LCDStatusInterrupts.HBlankInt) != 0;

	public void SetMode(LCDStatusModes state) {
		byte modeMask = (byte)(LCDStatusModes.HBlankMode | LCDStatusModes.VBlankMode | LCDStatusModes.OAMSearchMode | LCDStatusModes.PixelTransferMode);
		Value &= (byte)~modeMask;
		Value |= (byte)state;
	}

	public void SetInterrupt(LCDStatusInterrupts interrupt, bool value) {
		if (value)
			Value |= (byte)interrupt;
		else
			Value &= (byte)~interrupt;
	}

	public LCDStatus(byte value) {
		Value = value;
	}

	public static LCDStatusModes GetModeFromPPUState(PPU.State state)
		=> state switch {
			PPU.State.HBlank => LCDStatusModes.HBlankMode,
			PPU.State.VBlank => LCDStatusModes.VBlankMode,
			PPU.State.OAMSearch => LCDStatusModes.OAMSearchMode,
			PPU.State.PixelTransfer => LCDStatusModes.PixelTransferMode,
			_ => throw new Exception("Invalid PPU state")
		};
}