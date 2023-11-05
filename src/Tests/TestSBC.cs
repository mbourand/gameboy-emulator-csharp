using System;
using System.Collections.Generic;
using GBMU;
using GBMU.Core;

namespace Tests.Instructions;

public class TestSBC : Test {
	private static readonly (byte, byte, bool, Dictionary<CPUFlag, bool>)[] tests = new (byte, byte, bool, Dictionary<CPUFlag, bool>)[]
	{
		(0xFF, 0x00, false, new() { {CPUFlag.Zero, false}, {CPUFlag.NSubtract, true}, {CPUFlag.HalfCarry, true}, {CPUFlag.Carry, true} }),
		(0x01, 0x10, false, new() { {CPUFlag.Zero, false}, {CPUFlag.NSubtract, true}, {CPUFlag.HalfCarry, true}, {CPUFlag.Carry, false} }),
		(0x42, 0x69, false, new() { {CPUFlag.Zero, false}, {CPUFlag.NSubtract, true}, {CPUFlag.HalfCarry, false}, {CPUFlag.Carry, false} }),
		(0xE9, 0x07, false, new() { {CPUFlag.Zero, false}, {CPUFlag.NSubtract, true}, {CPUFlag.HalfCarry, true}, {CPUFlag.Carry, true} }),
		(0xFF, 0xFF, false, new() { {CPUFlag.Zero, true}, {CPUFlag.NSubtract, true}, {CPUFlag.HalfCarry, false}, {CPUFlag.Carry, false} }),
		(0x00, 0x00, false, new() { {CPUFlag.Zero, true}, {CPUFlag.NSubtract, true}, {CPUFlag.HalfCarry, false}, {CPUFlag.Carry, false} }),
		(0xFF, 0x00, true, new() { {CPUFlag.Zero, true}, {CPUFlag.NSubtract, true}, {CPUFlag.HalfCarry, true}, {CPUFlag.Carry, true} }),
		(0x01, 0x10, true, new() { {CPUFlag.Zero, false}, {CPUFlag.NSubtract, true}, {CPUFlag.HalfCarry, true}, {CPUFlag.Carry, false} }),
		(0x42, 0x69, true, new() { {CPUFlag.Zero, false}, {CPUFlag.NSubtract, true}, {CPUFlag.HalfCarry, false}, {CPUFlag.Carry, false} }),
		(0xE9, 0x07, true, new() { {CPUFlag.Zero, false}, {CPUFlag.NSubtract, true}, {CPUFlag.HalfCarry, true}, {CPUFlag.Carry, true} }),
		(0xFF, 0xFF, true, new() { {CPUFlag.Zero, false}, {CPUFlag.NSubtract, true}, {CPUFlag.HalfCarry, true}, {CPUFlag.Carry, true} }),
		(0x00, 0x00, true, new() { {CPUFlag.Zero, false}, {CPUFlag.NSubtract, true}, {CPUFlag.HalfCarry, true}, {CPUFlag.Carry, true} }),
		(0x01, 0x02, true, new() { {CPUFlag.Zero, true}, {CPUFlag.NSubtract, true}, {CPUFlag.HalfCarry, false}, {CPUFlag.Carry, false} }),
	};

	public static bool CheckFlags(Dictionary<CPUFlag, bool> expectedValues, CPU cpu) {
		foreach (var flag in expectedValues.Keys)
			if (expectedValues[flag] != cpu.GetFlag(flag))
				return false;
		return true;
	}

	public static void RunOperation(
		Gameboy gameboy,
		OperationDataType source,
		OperationDataType destination,
		ushort sourceValue,
		ushort destinationValue,
		bool carryValue,
		ushort expectedOutput,
		Dictionary<CPUFlag, bool> expectedFlags,
		FlagPermissionHandler flagPermissionHandler
	) {
		gameboy.CPU.SetFlag(CPUFlag.Carry, carryValue);
		try {
			source.WriteToDestination(gameboy.CPU, gameboy.Memory, sourceValue);
		} catch (Exception) { }
		destination.WriteToDestination(gameboy.CPU, gameboy.Memory, destinationValue);

		var operation = new OperatorSBC(source, destination, flagPermissionHandler);
		operation.Execute(gameboy.CPU, gameboy.Memory, 0x00);

		var valueWasSet = expectedOutput == destination.GetSourceValue(gameboy.CPU, gameboy.Memory);
		var flagsWereSet = CheckFlags(expectedFlags, gameboy.CPU);
		Expect(gameboy, operation, valueWasSet && flagsWereSet, sourceValue, destinationValue);
	}

	public static void TestSBCReg8Reg8(CPURegister source, CPURegister destination) {
		foreach (var test in tests) {
			var gameboy = TestUtils.InitGameboy();
			RunOperation(gameboy,
				new DataTypeReg8(source),
				new DataTypeReg8(destination),
				test.Item1,
				test.Item2,
				test.Item3,
				(byte)(test.Item2 - test.Item1 - (test.Item3 ? 1 : 0)),
				test.Item4,
				new FlagPermissionHandler("Z1HC")
			);
		}
	}

	public static void TestSBCU8Reg8(CPURegister destination) {
		foreach (var test in tests) {
			var gameboy = TestUtils.InitGameboy();
			gameboy.Memory.WriteByte((ushort)(gameboy.CPU.PC + 1), test.Item1);
			RunOperation(gameboy,
				new DataTypeU8(),
				new DataTypeReg8(destination),
				test.Item1,
				test.Item2,
				test.Item3,
				(byte)(test.Item2 - test.Item1 - (test.Item3 ? 1 : 0)),
				test.Item4,
				new FlagPermissionHandler("Z1HC")
			);
		}
	}

	public static void TestSBCReg16AddressReg8(CPURegister source, CPURegister destination, ushort sourceAddress) {
		foreach (var test in tests) {
			var gameboy = TestUtils.InitGameboy();
			gameboy.CPU.Set16BitRegister(source, sourceAddress);
			RunOperation(gameboy,
				new DataTypeReg16Address(source),
				new DataTypeReg8(destination),
				test.Item1,
				test.Item2,
				test.Item3,
				(byte)(test.Item2 - test.Item1 - (test.Item3 ? 1 : 0)),
				test.Item4,
				new FlagPermissionHandler("Z1HC")
			);
		}
	}

	public override void Run() {
		TestSBCReg8Reg8(CPURegister.B, CPURegister.A);
		TestSBCReg8Reg8(CPURegister.C, CPURegister.A);
		TestSBCReg8Reg8(CPURegister.D, CPURegister.A);
		TestSBCReg8Reg8(CPURegister.E, CPURegister.A);
		TestSBCReg8Reg8(CPURegister.H, CPURegister.A);
		TestSBCReg8Reg8(CPURegister.L, CPURegister.A);

		TestSBCReg16AddressReg8(CPURegister.HL, CPURegister.A, Memory.WorkRAMBank0.Address);

		TestSBCU8Reg8(CPURegister.A);

		Console.WriteLine("All SBC tests passed");
	}

	public static void Expect(Gameboy gameboy, CPUOperator operation, bool condition, ushort sourceValue, ushort destinationValue) {
		if (!condition)
			throw new Exception(operation.ToString(gameboy.CPU, gameboy.Memory, 0x00, 0x00) + $" failed for {destinationValue} - {sourceValue}");
		Console.WriteLine(operation.ToString(gameboy.CPU, gameboy.Memory, 0x00, 0x00) + $" succeeded for {destinationValue} - {sourceValue}");
	}
}