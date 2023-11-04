namespace GBMU.Core;

// Cette classe contient une logique générale permettant de charger et assigner des données via un registre 8 bits
public class DataTypeReg8 : OperationDataType {
	private readonly CPURegister _register;

	public DataTypeReg8(CPURegister register) => _register = register;

	public ushort GetSourceValue(CPU cpu, Memory memory) => cpu.Get8BitRegister(_register);
	public void WriteToDestination(CPU cpu, Memory memory, ushort value) => cpu.Set8BitRegister(_register, (byte)(value & 0xFF));

	public byte GetLength() => 0;
	public string GetMnemonic() => _register.ToString();
}