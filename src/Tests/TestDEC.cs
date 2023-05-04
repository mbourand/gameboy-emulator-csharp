using System;
using System.Collections.Generic;
using GBMU;
using GBMU.Core;

namespace Tests.Instructions;

public class TestDEC : Test
{
	public bool CheckFlags(Dictionary<CPUFlag, bool> expectedValues, CPU cpu)
	{
		foreach (var flag in expectedValues.Keys)
			if (expectedValues[flag] != cpu.GetFlag(flag))
				return false;
		return true;
	}

	public void RunOperation(Gameboy gameboy, OperationDataType dataType, ushort inputValue, ushort expectedOutput, Dictionary<CPUFlag, bool> expectedFlags, FlagPermissionHandler flagPermissionHandler)
	{
		dataType.WriteToDestination(gameboy.cpu, gameboy.memory, inputValue);

		var operation = new OperatorDEC(dataType, flagPermissionHandler);
		operation.Execute(gameboy.cpu, gameboy.memory, 0x00);

		var valueWasSet = expectedOutput == dataType.GetSourceValue(gameboy.cpu, gameboy.memory);
		var flagsWereSet = CheckFlags(expectedFlags, gameboy.cpu);
		Expect(gameboy, operation, valueWasSet && flagsWereSet, inputValue);
	}

	public void TestDEC8Bit(OperationDataType dataType)
	{
		var tests = new (byte, byte, Dictionary<CPUFlag, bool>)[] {
			(0x01, 0x00, new() { {CPUFlag.ZERO, true}, {CPUFlag.N_SUBTRACT, true}, {CPUFlag.HALF_CARRY, false}, {CPUFlag.CARRY, false} }),
			(0x10, 0x0F, new() { {CPUFlag.ZERO, false}, {CPUFlag.N_SUBTRACT, true}, {CPUFlag.HALF_CARRY, true}, {CPUFlag.CARRY, false} }),
			(0x43, 0x42, new() { {CPUFlag.ZERO, false}, {CPUFlag.N_SUBTRACT, true}, {CPUFlag.HALF_CARRY, false}, {CPUFlag.CARRY, false} }),
			(0x00, 0xFF, new() { {CPUFlag.ZERO, false}, {CPUFlag.N_SUBTRACT, true}, {CPUFlag.HALF_CARRY, true}, {CPUFlag.CARRY, false} }),
		};

		foreach (var test in tests)
		{
			var gameboy = TestUtils.InitGameboy();
			RunOperation(gameboy, dataType, test.Item1, test.Item2, test.Item3, new FlagPermissionHandler("Z1H-"));
		}
	}

	public void TestDEC16Bit(OperationDataType dataType)
	{
		var tests = new (ushort, ushort, Dictionary<CPUFlag, bool>)[]
		{
			(0x0001, 0x0000, new() { {CPUFlag.ZERO, false}, {CPUFlag.N_SUBTRACT, false}, {CPUFlag.HALF_CARRY, false}, {CPUFlag.CARRY, false} }),
			(0x0010, 0x000F, new() { {CPUFlag.ZERO, false}, {CPUFlag.N_SUBTRACT, false}, {CPUFlag.HALF_CARRY, false}, {CPUFlag.CARRY, false} }),
			(0x0043, 0x0042, new() { {CPUFlag.ZERO, false}, {CPUFlag.N_SUBTRACT, false}, {CPUFlag.HALF_CARRY, false}, {CPUFlag.CARRY, false} }),
			(0x0100, 0x00FF, new() { {CPUFlag.ZERO, false}, {CPUFlag.N_SUBTRACT, false}, {CPUFlag.HALF_CARRY, false}, {CPUFlag.CARRY, false} }),
			(0x0101, 0x0100, new() { {CPUFlag.ZERO, false}, {CPUFlag.N_SUBTRACT, false}, {CPUFlag.HALF_CARRY, false}, {CPUFlag.CARRY, false} }),
			(0x1000, 0x0FFF, new() { {CPUFlag.ZERO, false}, {CPUFlag.N_SUBTRACT, false}, {CPUFlag.HALF_CARRY, false}, {CPUFlag.CARRY, false} }),
			(0x426A, 0x4269, new() { {CPUFlag.ZERO, false}, {CPUFlag.N_SUBTRACT, false}, {CPUFlag.HALF_CARRY, false}, {CPUFlag.CARRY, false} }),
			(0x0000, 0xFFFF, new() { {CPUFlag.ZERO, false}, {CPUFlag.N_SUBTRACT, false}, {CPUFlag.HALF_CARRY, false}, {CPUFlag.CARRY, false} }),
		};

		foreach (var test in tests)
		{
			var gameboy = TestUtils.InitGameboy();
			RunOperation(gameboy, dataType, test.Item1, test.Item2, test.Item3, new FlagPermissionHandler("----"));
		}
	}

	public void TestDECReg16Address(CPURegister register, ushort registerAddress)
	{
		var tests = new (byte, byte, Dictionary<CPUFlag, bool>)[] {
			(0x01, 0x00, new() { {CPUFlag.ZERO, true}, {CPUFlag.N_SUBTRACT, true}, {CPUFlag.HALF_CARRY, false}, {CPUFlag.CARRY, false} }),
			(0x10, 0x0F, new() { {CPUFlag.ZERO, false}, {CPUFlag.N_SUBTRACT, true}, {CPUFlag.HALF_CARRY, true}, {CPUFlag.CARRY, false} }),
			(0x43, 0x42, new() { {CPUFlag.ZERO, false}, {CPUFlag.N_SUBTRACT, true}, {CPUFlag.HALF_CARRY, false}, {CPUFlag.CARRY, false} }),
			(0x00, 0xFF, new() { {CPUFlag.ZERO, false}, {CPUFlag.N_SUBTRACT, true}, {CPUFlag.HALF_CARRY, true}, {CPUFlag.CARRY, false} }),
		};

		var dataType = new DataTypeReg16Address(register);

		foreach (var test in tests)
		{
			var gameboy = TestUtils.InitGameboy();
			gameboy.cpu.Set16BitRegister(register, registerAddress);
			RunOperation(gameboy, dataType, test.Item1, test.Item2, test.Item3, new FlagPermissionHandler("Z1H-"));
		}
	}


	public override void Run()
	{
		var registers8Bits = new CPURegister[] { CPURegister.A, CPURegister.B, CPURegister.C, CPURegister.D, CPURegister.E, CPURegister.H, CPURegister.L };

		foreach (var register in registers8Bits)
			TestDEC8Bit(new DataTypeReg8(register));

		var registers16Bits = new CPURegister[] { CPURegister.BC, CPURegister.DE, CPURegister.HL, CPURegister.SP };
		foreach (var register in registers16Bits)
			TestDEC16Bit(new DataTypeReg16(register));

		TestDECReg16Address(CPURegister.HL, Memory.WorkRAMBank0.addr);

		Console.WriteLine("All DEC tests passed");
	}

	public void Expect(Gameboy gameboy, CPUOperator operation, bool condition, ushort inputValue)
	{
		if (!condition)
			throw new System.Exception(operation.ToString(gameboy.cpu, gameboy.memory, 0x00, 0x00) + $" failed with input value 0x{inputValue:X2}");
		Console.WriteLine(operation.ToString(gameboy.cpu, gameboy.memory, 0x00, 0x00) + $" succeeded with input value 0x{inputValue:X2}");
	}
}