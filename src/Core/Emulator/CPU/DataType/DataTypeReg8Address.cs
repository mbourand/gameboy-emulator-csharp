namespace GBMU.Core;

// Cette classe contient une logique générale permettant de charger et assigner des données via un registre 8 bits
public class DataTypeReg8Address : OperationDataType {
	private readonly CPURegister _register;
	private ushort? _lastRegisteredValue;

	public DataTypeReg8Address(CPURegister register) {
		_register = register;
		_lastRegisteredValue = null;
	}

	public ushort GetSourceValue(CPU cpu, Memory memory) {
		_lastRegisteredValue = memory.ReadByte((ushort)(0xFF00 + cpu.Get8BitRegister(_register)));
		return memory.ReadByte((ushort)(0xFF00 + cpu.Get8BitRegister(_register)));
	}

	public void WriteToDestination(CPU cpu, Memory memory, ushort value) {
		_lastRegisteredValue = (ushort)(0xFF00 + cpu.Get8BitRegister(_register));
		memory.WriteByte((ushort)(0xFF00 + cpu.Get8BitRegister(_register)), (byte)(value & 0xFF));
	}

	public byte GetLength() => 0;


	public string GetMnemonic() {
		if (_lastRegisteredValue.HasValue) {
			return $"(0xFF00 + {_register}) {{0x{_lastRegisteredValue.Value:X2}}}";
		}
		return $"(0xFF00 + {_register})";
	}
}