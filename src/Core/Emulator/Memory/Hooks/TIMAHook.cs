using System;

namespace GBMU.Core;

public class TIMAHook : IMemoryHook {
	private readonly Timers _timers;

	public TIMAHook(Timers timers) {
		_timers = timers;
	}

	public byte? OnReadByte(byte[] memory, ushort address) {
		return null;
	}

	public bool OnWriteByte(byte[] memory, ushort address, byte value) {
		if (address != Memory.TIMA.Address)
			return false;

		// TIMA can't be written during the delay between the overflow and the reset to TMA
		if (!_timers.TimaWasResetThisCycle) {
			MemoryUtils.WriteByte(memory, address, value);
			_timers.CancelTimaOverflow();
		}

		return true;
	}

	public ushort? OnReadWord(byte[] memory, ushort address) {
		return null;
	}

	public bool OnWriteWord(byte[] memory, ushort address, ushort value) {
		return false;
	}
}