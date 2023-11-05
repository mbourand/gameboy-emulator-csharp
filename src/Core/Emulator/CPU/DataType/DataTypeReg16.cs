namespace GBMU.Core;

// Cette classe contient une logique générale permettant de charger et assigner des données via un registre 16 bits
public class DataTypeReg16 : OperationDataType {
	private readonly CPURegister _register;
	private ushort? _lastRegisteredValue;

	public DataTypeReg16(CPURegister register) {
		_register = register;
		_lastRegisteredValue = null;
	}

	public ushort GetSourceValue(CPU cpu, Memory memory) {
		_lastRegisteredValue = cpu.Get16BitRegister(_register);
		return cpu.Get16BitRegister(_register);
	}
	public void WriteToDestination(CPU cpu, Memory memory, ushort value) {
		_lastRegisteredValue = GetSourceValue(cpu, memory);
		cpu.Set16BitRegister(_register, value);
	}

	public byte GetLength() => 0;

	public string GetMnemonic() {
		if (_lastRegisteredValue.HasValue) {
			return $"{_register} {{0x{_lastRegisteredValue.Value:X4}}}";
		}
		return $"{_register}";
	}
}