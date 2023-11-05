using System;

namespace GBMU.Core;

// Cette classe contient une logique générale permettant de charger et assigner des données via un registre 16 bits
public class DataTypeOffsettedReg16 : OperationDataType {
	private readonly CPURegister _register;
	private ushort? _lastRegisteredValue;

	public DataTypeOffsettedReg16(CPURegister register) {
		_register = register;
		_lastRegisteredValue = null;
	}

	public ushort GetSourceValue(CPU cpu, Memory memory) {
		var offset = (sbyte)memory.ReadByte((ushort)(cpu.PC + 1));
		var offsettedValue = (ushort)(cpu.Get16BitRegister(_register) + offset);
		_lastRegisteredValue = offsettedValue;
		return offsettedValue;
	}

	public void WriteToDestination(CPU cpu, Memory memory, ushort value) => throw new Exception("Cannot write to offsetted reg 16");

	public byte GetLength() => 1;

	public string GetMnemonic() {
		if (_lastRegisteredValue.HasValue) {
			return $"{_register} {{0x{_lastRegisteredValue.Value:X4}}}";
		}
		return $"{_register}";
	}
}