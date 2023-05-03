using System;
using System.IO;
using GBMU;
using GBMU.Core;

namespace Tests.Instructions;

public class TestLD : Test
{
	public Gameboy Init()
	{
		var stream = File.OpenRead("./resources/roms/Tetris.gb");
		var gameboy = new Gameboy(stream);
		stream.Close();

		return gameboy;
	}

	public void TestLDReg8Reg8(CPURegister source, CPURegister destination)
	{
		var gameboy = Init();
		var saveSource = gameboy.cpu.Get8BitRegister(source);
		var operation = new OperatorLD(new DataTypeReg8(source), new DataTypeReg8(destination));

		operation.Execute(gameboy.cpu, gameboy.memory, 0x00);

		Expect(gameboy, operation, saveSource == gameboy.cpu.Get8BitRegister(destination));
	}

	public void TestLDReg16AddressReg8(CPURegister sourceAddressRegister, CPURegister destinationRegister)
	{
		var gameboy = Init();
		var saveSource = gameboy.memory.ReadByte(gameboy.cpu.Get16BitRegister(sourceAddressRegister));
		var operation = new OperatorLD(new DataTypeReg16Address(sourceAddressRegister), new DataTypeReg8(destinationRegister));

		operation.Execute(gameboy.cpu, gameboy.memory, 0x00);

		Expect(gameboy, operation, saveSource == gameboy.cpu.Get8BitRegister(destinationRegister));
	}

	public void TestLDReg8Reg16Address(CPURegister sourceRegister, CPURegister destinationAddressRegister)
	{
		var gameboy = Init();
		// Set the register to a workable memory address
		gameboy.cpu.Set16BitRegister(destinationAddressRegister, Memory.WorkRAMBank0.addr);
		var saveSource = gameboy.cpu.Get8BitRegister(sourceRegister);
		var operation = new OperatorLD(new DataTypeReg8(sourceRegister), new DataTypeReg16Address(destinationAddressRegister));

		operation.Execute(gameboy.cpu, gameboy.memory, 0x00);

		Expect(gameboy, operation, gameboy.memory.ReadByte(gameboy.cpu.Get16BitRegister(destinationAddressRegister)) == saveSource);
	}

	public void TestLDReg8Reg16Address(byte sourceValue, CPURegister destinationRegister)
	{
		var gameboy = Init();
		gameboy.cartridge.WriteByte((ushort)(gameboy.cpu.PC + 1), sourceValue);
		var operation = new OperatorLD(new DataTypeU8(), new DataTypeReg8(destinationRegister));

		operation.Execute(gameboy.cpu, gameboy.memory, 0x00);

		Expect(gameboy, operation, sourceValue == gameboy.cpu.Get8BitRegister(destinationRegister));
	}

	public override void Run()
	{
		var registers = new CPURegister[] { CPURegister.B, CPURegister.C, CPURegister.D, CPURegister.E, CPURegister.H, CPURegister.L, CPURegister.A };

		foreach (var destinationRegister in registers)
			foreach (var sourceRegister in registers)
				TestLDReg8Reg8(sourceRegister, destinationRegister);

		TestLDReg16AddressReg8(CPURegister.BC, CPURegister.A);
		TestLDReg16AddressReg8(CPURegister.DE, CPURegister.A);
		TestLDReg16AddressReg8(CPURegister.HL, CPURegister.A);
		TestLDReg16AddressReg8(CPURegister.HL, CPURegister.B);
		TestLDReg16AddressReg8(CPURegister.HL, CPURegister.C);
		TestLDReg16AddressReg8(CPURegister.HL, CPURegister.D);
		TestLDReg16AddressReg8(CPURegister.HL, CPURegister.E);
		TestLDReg16AddressReg8(CPURegister.HL, CPURegister.H);
		TestLDReg16AddressReg8(CPURegister.HL, CPURegister.L);

		TestLDReg8Reg16Address(CPURegister.A, CPURegister.BC);
		TestLDReg8Reg16Address(CPURegister.A, CPURegister.DE);
		TestLDReg8Reg16Address(CPURegister.A, CPURegister.HL);
		TestLDReg8Reg16Address(CPURegister.B, CPURegister.HL);
		TestLDReg8Reg16Address(CPURegister.C, CPURegister.HL);
		TestLDReg8Reg16Address(CPURegister.D, CPURegister.HL);
		TestLDReg8Reg16Address(CPURegister.E, CPURegister.HL);
		TestLDReg8Reg16Address(CPURegister.H, CPURegister.HL);
		TestLDReg8Reg16Address(CPURegister.L, CPURegister.HL);

		foreach (var destinationRegister in registers)
			TestLDReg8Reg16Address(0x42, destinationRegister);

		Console.WriteLine("All tests passed");
	}

	public void Expect(Gameboy gameboy, CPUOperator operation, bool condition)
	{
		if (!condition)
			throw new System.Exception(operation.ToString(gameboy.cpu, gameboy.memory, 0x00, 0x00) + " failed");
		Console.WriteLine(operation.ToString(gameboy.cpu, gameboy.memory, 0x00, 0x00) + " succeeded");
	}
}