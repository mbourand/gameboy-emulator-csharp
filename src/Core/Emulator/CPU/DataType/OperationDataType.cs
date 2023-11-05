namespace GBMU.Core;

// Cette interface permet de définir les types de données que l'on peut charger et assigner, comme les registres ou les adresses
public interface OperationDataType {
	public ushort GetSourceValue(CPU cpu, Memory memory);
	public void WriteToDestination(CPU cpu, Memory memory, ushort value);
	public byte GetLength();
	public string GetMnemonic();
}