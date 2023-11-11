namespace GBMU.Core;

public class TMAHook : IMemoryHook {
	private readonly Timers _timers;

	public TMAHook(Timers timers) {
		_timers = timers;
	}

	public byte? OnReadByte(byte[] memory, ushort address) {
		return null;
	}

	public bool OnWriteByte(byte[] memory, ushort address, byte value) {
		if (address != Memory.TMA.Address)
			return false;

		if (!_timers.TimaWasResetThisCycle)
			return false;

		MemoryUtils.WriteByte(memory, address, value);
		MemoryUtils.WriteByte(memory, Memory.TIMA.Address, value);
		return true;
	}

	public ushort? OnReadWord(byte[] memory, ushort address) {
		return null;
	}

	public bool OnWriteWord(byte[] memory, ushort address, ushort value) {
		return false;
	}
}