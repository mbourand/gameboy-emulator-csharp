namespace GBMU.Core;

// Cette classe contient une logique générale permettant de charger et assigner des données via un registre 16 bits
public class DataTypeReg16 : OperationDataType {
	private readonly CPURegister _register;

	public DataTypeReg16(CPURegister register) => _register = register;

	public ushort GetSourceValue(CPU cpu, Memory memory) => cpu.Get16BitRegister(_register);
	public void WriteToDestination(CPU cpu, Memory memory, ushort value) => cpu.Set16BitRegister(_register, value);

	public byte GetLength() => 0;
	public string GetMnemonic() => _register.ToString();
}