using System;

namespace GBMU.Core;

public static class InterruptUtils {
	public static Interrupt? GetHighestPriorityInterrupt(InterruptByte interruptByte) {
		if (interruptByte.HasVBlank)
			return Interrupt.VBlank;
		if (interruptByte.HasLCDStat)
			return Interrupt.LCDStat;
		if (interruptByte.HasTimer)
			return Interrupt.Timer;
		if (interruptByte.HasSerial)
			return Interrupt.Serial;
		if (interruptByte.HasJoypad)
			return Interrupt.Joypad;
		return null;
	}

	public static MemoryKeyPoint GetVectorFromInterrupt(Interrupt interrupt) => interrupt switch {
		Interrupt.VBlank => Memory.InterruptVectorVBlank,
		Interrupt.LCDStat => Memory.InterruptVectorLCDStat,
		Interrupt.Timer => Memory.InterruptVectorTimer,
		Interrupt.Serial => Memory.InterruptVectorSerial,
		Interrupt.Joypad => Memory.InterruptVectorJoypad,
		_ => throw new Exception("Invalid interrupt")
	};
}