using System;
using System.Numerics;

namespace GBMU.Core;

public enum BankingSelectMode {
	SIMPLE = 0,
	ADVANCED = 1
}

public class MBC1 : IMemoryHook {
	private readonly byte[][] _romBanks;
	private readonly byte[][] _ramBanks;
	private int _selectedRomBankIndex;
	private int _selectedRamBankIndex;
	private bool _isRamEnabled;
	private BankingSelectMode _bankingMode;

	public MBC1(Cartridge cartridge) {
		_ramBanks = new byte[RamBankCount][];
		for (int i = 0; i < _ramBanks.Length; i++)
			_ramBanks[i] = new byte[RamBankSize];

		_romBanks = new byte[cartridge.RomBankCount][];
		for (int i = 0; i < cartridge.RomBankCount; i++) {
			_romBanks[i] = new byte[RomBankSize];
			Array.Copy(cartridge.Rom, i * RomBankSize, _romBanks[i], 0, RomBankSize);
		}

		_bankingMode = BankingSelectMode.SIMPLE;
		_isRamEnabled = false;
		_selectedRamBankIndex = 0;
		_selectedRomBankIndex = 1;
	}

	public byte? OnReadByte(byte[] memory, ushort address) {
		byte usedRomBankIndex = (byte)(_selectedRomBankIndex | (_bankingMode == BankingSelectMode.ADVANCED ? _selectedRamBankIndex << 5 : 0x00));
		if (usedRomBankIndex >= _romBanks.Length) {
			byte maxIndex = (byte)(_romBanks.Length - 1);
			byte mostSignificantPosition = (byte)(63 - BitOperations.LeadingZeroCount(maxIndex));
			byte mostSignificantBit = (byte)(1 << mostSignificantPosition);
			byte mask = (byte)(mostSignificantBit - 1 | mostSignificantBit);
			usedRomBankIndex = (byte)(usedRomBankIndex & mask);
		}
		byte usedRamBankIndex = (byte)(_bankingMode == BankingSelectMode.ADVANCED ? _selectedRamBankIndex : 0x00);

		if (Memory.ROMBank0.IsInRange(address))
			return MemoryUtils.ReadByte(memory, address);
		if (Memory.SwitchableROMBank.IsInRange(address))
			return MemoryUtils.RedirectedReadByte(_romBanks[usedRomBankIndex], address, Memory.SwitchableROMBank.Address, 0x0000);
		if (Memory.ExternalRAM.IsInRange(address))
			return _isRamEnabled ? MemoryUtils.RedirectedReadByte(_ramBanks[usedRamBankIndex], address, Memory.ExternalRAM.Address, 0x0000) : (byte)0xFF;
		return null;
	}

	public bool OnWriteByte(byte[] memory, ushort address, byte value) {
		byte usedRamBankIndex = (byte)(_bankingMode == BankingSelectMode.ADVANCED ? _selectedRamBankIndex : 0x00);

		if (Memory.RAMEnable.IsInRange(address)) {
			_isRamEnabled = ByteUtils.LowNibble(value) == 0x0A;
			return true;
		}

		if (Memory.ROMBankNumber.IsInRange(address)) {
			byte maskedValue = (byte)(value & 0b0001_1111);

			if (maskedValue == 0x20 || maskedValue == 0x40 || maskedValue == 0x60)
				_selectedRomBankIndex = maskedValue + 1;
			else
				_selectedRomBankIndex = Math.Max(maskedValue, (byte)1);

			return true;
		}

		if (Memory.ExternalRAM.IsInRange(address)) {
			if (_isRamEnabled)
				MemoryUtils.RedirectedWriteByte(_ramBanks[usedRamBankIndex], address, Memory.ExternalRAM.Address, 0x0000, value);
			return true;
		}

		if (Memory.RAMBankNumber.IsInRange(address)) {
			_selectedRamBankIndex = value & 0b0000_0011;
			return true;
		}

		if (Memory.BankingModeSelect.IsInRange(address)) {
			_bankingMode = (BankingSelectMode)(value & 0b0000_0001);
			return true;
		}


		return false;
	}

	public ushort? OnReadWord(byte[] memory, ushort address) {
		return null;
	}

	public bool OnWriteWord(byte[] memory, ushort address, ushort value) {
		return false;
	}

	public const int RomBankSize = 0x4000;
	public const int RamBankSize = 0x2000;
	public const int RamBankCount = 4;
}