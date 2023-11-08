namespace GBMU.Core;

public class DIVHook : IMemoryHook {
	private readonly Timers _timers;

	public DIVHook(Timers timers) {
		_timers = timers;
	}

	public byte? OnReadByte(byte[] memory, ushort address) {
		return null;
	}

	public bool OnWriteByte(byte[] memory, ushort address, byte value) {
		if (address != Memory.DIV.Address) {
			return false;
		}

		_timers.ResetInternalTimer();
		MemoryUtils.WriteByte(memory, address, 0);
		return true;
	}

	public ushort? OnReadWord(byte[] memory, ushort address) {
		return null;
	}

	public bool OnWriteWord(byte[] memory, ushort address, ushort value) {
		return false;
	}
}