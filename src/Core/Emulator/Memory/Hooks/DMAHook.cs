using System;

namespace GBMU.Core;

public class DMAHook : IMemoryHook {
	private int _oamDmaTransferCyclesLeft;
	private double _elapsedTime;
	private readonly Memory _memory;

	public DMAHook(Memory memory) {
		_memory = memory;
		_oamDmaTransferCyclesLeft = 0;
		_elapsedTime = 0;
	}

	public byte? OnReadByte(byte[] memory, ushort address) {
		if (address == Memory.DMA.Address || Memory.HRAM.IsInRange(address))
			return null;

		return _oamDmaTransferCyclesLeft > 0 ? (byte?)0xFF : null;
	}

	public bool OnWriteByte(byte[] memory, ushort address, byte value) {
		if (address == Memory.DMA.Address) {
			MemoryKeyPoint sourceRange = new(ByteUtils.MakeWord(value, 0), 0xA0);
			for (int i = 0; i < sourceRange.Size; i++)
				_memory.WriteByte((ushort)(Memory.OAM.Address + i), _memory.ReadByte((ushort)(sourceRange.Address + i)));
			MemoryUtils.WriteByte(memory, Memory.DMA.Address, value);
			_oamDmaTransferCyclesLeft = 160 * 4;
			return true;
		}

		return _oamDmaTransferCyclesLeft > 0 && !Memory.HRAM.IsInRange(address) && address != Memory.DMA.Address;
	}

	public ushort? OnReadWord(byte[] memory, ushort address) {
		return null;
	}

	public bool OnWriteWord(byte[] memory, ushort address, ushort value) {
		return false;
	}

	public void Update(double elapsedTime) {
		_elapsedTime += elapsedTime;
		if (_elapsedTime < CPU.CycleDuration)
			return;
		_elapsedTime -= CPU.CycleDuration;

		_oamDmaTransferCyclesLeft = Math.Max(0, _oamDmaTransferCyclesLeft - 1);
	}
}