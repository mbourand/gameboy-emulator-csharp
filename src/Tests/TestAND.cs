using System;
using System.Collections.Generic;
using GBMU;
using GBMU.Core;

namespace Tests.Instructions;

public class TestAND : Test
{
	public bool CheckFlags(Dictionary<CPUFlag, bool> expectedValues, CPU cpu)
	{
		foreach (var flag in expectedValues.Keys)
			if (expectedValues[flag] != cpu.GetFlag(flag))
				return false;
		return true;
	}

	public void RunOperation(Gameboy gameboy, OperationDataType source, OperationDataType destination, ushort sourceValue, ushort destinationValue, ushort expectedOutput, Dictionary<CPUFlag, bool> expectedFlags, FlagPermissionHandler flagPermissionHandler)
	{
		try
		{
			source.WriteToDestination(gameboy.cpu, gameboy.memory, sourceValue);
		}
		catch (Exception) { }
		destination.WriteToDestination(gameboy.cpu, gameboy.memory, destinationValue);


		var operation = new OperatorAND(source, destination, flagPermissionHandler);
		operation.Execute(gameboy.cpu, gameboy.memory, 0x00);

		var valueWasSet = expectedOutput == destination.GetSourceValue(gameboy.cpu, gameboy.memory);
		var flagsWereSet = CheckFlags(expectedFlags, gameboy.cpu);
		Expect(gameboy, operation, valueWasSet && flagsWereSet, sourceValue, destinationValue);
	}

	public void TestANDReg8Reg8(CPURegister source, CPURegister destination)
	{
		var tests = new (byte, byte, Dictionary<CPUFlag, bool>)[]
		{
			(0xFF, 0x00, new() { {CPUFlag.ZERO, true}, {CPUFlag.N_SUBTRACT, false}, {CPUFlag.HALF_CARRY, true}, {CPUFlag.CARRY, false} }),
			(0b01, 0b10, new() { {CPUFlag.ZERO, true}, {CPUFlag.N_SUBTRACT, false}, {CPUFlag.HALF_CARRY, true}, {CPUFlag.CARRY, false} }),
			(0x42, 0x69, new() { {CPUFlag.ZERO, false}, {CPUFlag.N_SUBTRACT, false}, {CPUFlag.HALF_CARRY, true}, {CPUFlag.CARRY, false} }),
			(0xE9, 0x07, new() { {CPUFlag.ZERO, false}, {CPUFlag.N_SUBTRACT, false}, {CPUFlag.HALF_CARRY, true}, {CPUFlag.CARRY, false} }),
			(0xFF, 0xFF, new() { {CPUFlag.ZERO, false}, {CPUFlag.N_SUBTRACT, false}, {CPUFlag.HALF_CARRY, true}, {CPUFlag.CARRY, false} }),
		};

		foreach (var test in tests)
		{
			var gameboy = TestUtils.InitGameboy();
			RunOperation(gameboy, new DataTypeReg8(source), new DataTypeReg8(destination), test.Item1, test.Item2, (ushort)(test.Item1 & test.Item2), test.Item3, new FlagPermissionHandler("Z010"));
		}
	}

	public void TestANDU8Reg8(CPURegister destination)
	{
		var tests = new (byte, byte, Dictionary<CPUFlag, bool>)[]
		{
			(0xFF, 0x00, new() { {CPUFlag.ZERO, true}, {CPUFlag.N_SUBTRACT, false}, {CPUFlag.HALF_CARRY, true}, {CPUFlag.CARRY, false} }),
			(0b01, 0b10, new() { {CPUFlag.ZERO, true}, {CPUFlag.N_SUBTRACT, false}, {CPUFlag.HALF_CARRY, true}, {CPUFlag.CARRY, false} }),
			(0x42, 0x69, new() { {CPUFlag.ZERO, false}, {CPUFlag.N_SUBTRACT, false}, {CPUFlag.HALF_CARRY, true}, {CPUFlag.CARRY, false} }),
			(0xE9, 0x07, new() { {CPUFlag.ZERO, false}, {CPUFlag.N_SUBTRACT, false}, {CPUFlag.HALF_CARRY, true}, {CPUFlag.CARRY, false} }),
			(0xFF, 0xFF, new() { {CPUFlag.ZERO, false}, {CPUFlag.N_SUBTRACT, false}, {CPUFlag.HALF_CARRY, true}, {CPUFlag.CARRY, false} }),
		};

		foreach (var test in tests)
		{
			var gameboy = TestUtils.InitGameboy();
			gameboy.cartridge.WriteByte((ushort)(gameboy.cpu.PC + 1), test.Item1);
			RunOperation(gameboy, new DataTypeU8(), new DataTypeReg8(destination), test.Item1, test.Item2, (ushort)(test.Item1 & test.Item2), test.Item3, new FlagPermissionHandler("Z010"));
		}
	}

	public void TestANDReg16AddressReg8(CPURegister source, CPURegister destination, ushort sourceAddress)
	{
		var tests = new (byte, byte, Dictionary<CPUFlag, bool>)[]
		{
			(0xFF, 0x00, new() { {CPUFlag.ZERO, true}, {CPUFlag.N_SUBTRACT, false}, {CPUFlag.HALF_CARRY, true}, {CPUFlag.CARRY, false} }),
			(0b01, 0b10, new() { {CPUFlag.ZERO, true}, {CPUFlag.N_SUBTRACT, false}, {CPUFlag.HALF_CARRY, true}, {CPUFlag.CARRY, false} }),
			(0x42, 0x69, new() { {CPUFlag.ZERO, false}, {CPUFlag.N_SUBTRACT, false}, {CPUFlag.HALF_CARRY, true}, {CPUFlag.CARRY, false} }),
			(0xE9, 0x07, new() { {CPUFlag.ZERO, false}, {CPUFlag.N_SUBTRACT, false}, {CPUFlag.HALF_CARRY, true}, {CPUFlag.CARRY, false} }),
			(0xFF, 0xFF, new() { {CPUFlag.ZERO, false}, {CPUFlag.N_SUBTRACT, false}, {CPUFlag.HALF_CARRY, true}, {CPUFlag.CARRY, false} }),
		};

		foreach (var test in tests)
		{
			var gameboy = TestUtils.InitGameboy();
			gameboy.cpu.Set16BitRegister(source, sourceAddress);
			RunOperation(gameboy, new DataTypeReg16Address(source), new DataTypeReg8(destination), test.Item1, test.Item2, (ushort)(test.Item1 & test.Item2), test.Item3, new FlagPermissionHandler("Z010"));
		}
	}

	public override void Run()
	{
		TestANDReg8Reg8(CPURegister.B, CPURegister.A);
		TestANDReg8Reg8(CPURegister.C, CPURegister.A);
		TestANDReg8Reg8(CPURegister.D, CPURegister.A);
		TestANDReg8Reg8(CPURegister.E, CPURegister.A);
		TestANDReg8Reg8(CPURegister.H, CPURegister.A);
		TestANDReg8Reg8(CPURegister.L, CPURegister.A);

		TestANDReg16AddressReg8(CPURegister.HL, CPURegister.A, Memory.WorkRAMBank0.addr);

		TestANDU8Reg8(CPURegister.A);

		Console.WriteLine("All AND tests passed");
	}

	public void Expect(Gameboy gameboy, CPUOperator operation, bool condition, ushort sourceValue, ushort destinationValue)
	{
		if (!condition)
			throw new System.Exception(operation.ToString(gameboy.cpu, gameboy.memory, 0x00, 0x00) + $" failed for {destinationValue} & {sourceValue}");
		Console.WriteLine(operation.ToString(gameboy.cpu, gameboy.memory, 0x00, 0x00) + $" succeeded for {destinationValue} & {sourceValue}");
	}
}