namespace GBMU.Core;

// Cette classe contient une logique générale permettant de charger et assigner des données via un registre 8 bits
public class DataTypeReg8 : OperationDataType {
	private readonly CPURegister _register;

	private ushort? _lastRegisteredValue;

	public DataTypeReg8(CPURegister register) {
		_register = register;
		_lastRegisteredValue = null;
	}

	public ushort GetSourceValue(CPU cpu, Memory memory) {
		_lastRegisteredValue = cpu.Get8BitRegister(_register);
		return cpu.Get8BitRegister(_register);
	}

	public void WriteToDestination(CPU cpu, Memory memory, ushort value) {
		_lastRegisteredValue = GetSourceValue(cpu, memory);
		cpu.Set8BitRegister(_register, (byte)(value & 0xFF));
	}

	public byte GetLength() => 0;

	public string GetMnemonic() {
		if (_lastRegisteredValue.HasValue) {
			return $"{_register} {{0x{_lastRegisteredValue.Value:X2}}}";
		}
		return $"{_register}";
	}
}