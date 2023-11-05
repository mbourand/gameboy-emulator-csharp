using System;

namespace GBMU.Core;

// Maybe one day I'll find a better way to handle this instruction
public class OperatorLD_F8 : CPUOperator
{
	protected DataTypeOffsettedReg16 _sourceDataType;
	protected DataTypeReg16 _destinationDataType;
	public OperatorLD_F8() : base("LD", 1)
	{
		_sourceDataType = new DataTypeOffsettedReg16(CPURegister.SP);
		_destinationDataType = new DataTypeReg16(CPURegister.HL);

		length += (byte)(_sourceDataType.GetLength() + _destinationDataType.GetLength());
	}

	public override void Execute(CPU cpu, Memory memory, int opcode)
	{
		var source = _sourceDataType.GetSourceValue(cpu, memory);

		var offset = memory.ReadByte((ushort)(cpu.PC + 1));

		var carryMask = (int)CarryBit.Bit8 - 1;
		var halfCarryMask = (int)HalfCarryBit.Bit4 - 1;

		cpu.SetFlag(CPUFlag.NSubtract, false);
		cpu.SetFlag(CPUFlag.Zero, false);
		cpu.SetFlag(CPUFlag.Carry, (cpu.SP & carryMask) + (offset & carryMask) >= (int)CarryBit.Bit8);
		cpu.SetFlag(CPUFlag.HalfCarry, (cpu.SP & halfCarryMask) + (offset & halfCarryMask) >= (int)HalfCarryBit.Bit4);

		_destinationDataType.WriteToDestination(cpu, memory, source);
		base.Execute(cpu, memory, opcode);
	}

	public override string ToString(CPU cpu, Memory memory, int opcode, ushort addr)
	{
		return base.ToString(cpu, memory, opcode, addr) + $" {_destinationDataType.GetMnemonic()}, {_sourceDataType.GetMnemonic()}";
	}
}