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

	public void BaseTestLD(Gameboy gameboy, OperationDataType sourceDataType, OperationDataType destinationType, byte sourceValue)
	{
		try
		{
			sourceDataType.WriteToDestination(gameboy.cpu, gameboy.memory, sourceValue);
		}
		catch (Exception) { }

		var operation = new OperatorLD(sourceDataType, destinationType);
		operation.Execute(gameboy.cpu, gameboy.memory, 0x00);

		Expect(gameboy, operation, sourceValue == destinationType.GetSourceValue(gameboy.cpu, gameboy.memory));
	}

	public void TestLDReg8Reg8(CPURegister source, CPURegister destination, byte sourceValue)
	{
		var gameboy = Init();
		BaseTestLD(gameboy, new DataTypeReg8(source), new DataTypeReg8(destination), sourceValue);
	}

	public void TestLDReg16AddressReg8(CPURegister sourceAddressRegister, CPURegister destinationRegister, byte sourceValue)
	{
		var gameboy = Init();
		// Put the source register to a workable address
		gameboy.cpu.Set16BitRegister(sourceAddressRegister, Memory.WorkRAMBank0.addr);
		BaseTestLD(gameboy, new DataTypeReg16Address(sourceAddressRegister), new DataTypeReg8(destinationRegister), sourceValue);
	}

	public void TestLDReg8Reg16Address(CPURegister sourceRegister, CPURegister destinationAddressRegister, byte sourceValue)
	{
		var gameboy = Init();
		// Put the destination register to a workable address
		gameboy.cpu.Set16BitRegister(destinationAddressRegister, Memory.WorkRAMBank0.addr);
		BaseTestLD(gameboy, new DataTypeReg8(sourceRegister), new DataTypeReg16Address(destinationAddressRegister), sourceValue);
	}

	public void TestLDReg8U8(CPURegister destinationRegister, byte sourceValue)
	{
		var gameboy = Init();
		// Emulate the U8 value in the ROM
		gameboy.cartridge.WriteByte((ushort)(gameboy.cpu.PC + 1), sourceValue);
		BaseTestLD(gameboy, new DataTypeU8(), new DataTypeReg8(destinationRegister), sourceValue);
	}

	public void TestLDReg16AddressU8(CPURegister destinationRegister, ushort destinationAddress, byte sourceValue)
	{
		var gameboy = Init();
		gameboy.cpu.Set16BitRegister(destinationRegister, destinationAddress);
		gameboy.cartridge.WriteWord((ushort)(gameboy.cpu.PC + 1), sourceValue);
		BaseTestLD(gameboy, new DataTypeU8(), new DataTypeReg16Address(destinationRegister), sourceValue);
	}

	public void TestLDU16AddressReg8(CPURegister sourceRegister, ushort destinationAddress, byte sourceValue)
	{
		var gameboy = Init();
		// Emulate the U16 address in the ROM
		gameboy.cartridge.WriteWord((ushort)(gameboy.cpu.PC + 1), destinationAddress);
		BaseTestLD(gameboy, new DataTypeReg8(sourceRegister), new DataTypeU16Address(), sourceValue);
	}

	public void TestLDReg8U16Address(CPURegister sourceRegister, ushort sourceAddress, byte sourceValue)
	{
		var gameboy = Init();
		// Emulate the U16 address in the ROM
		gameboy.cartridge.WriteWord((ushort)(gameboy.cpu.PC + 1), sourceAddress);
		BaseTestLD(gameboy, new DataTypeU16Address(), new DataTypeReg8(sourceRegister), sourceValue);
	}

	public override void Run()
	{
		var registers = new CPURegister[] { CPURegister.B, CPURegister.C, CPURegister.D, CPURegister.E, CPURegister.H, CPURegister.L, CPURegister.A };

		foreach (var destinationRegister in registers)
			foreach (var sourceRegister in registers)
				TestLDReg8Reg8(sourceRegister, destinationRegister, 0x42);

		TestLDReg16AddressReg8(CPURegister.BC, CPURegister.A, 0x42);
		TestLDReg16AddressReg8(CPURegister.DE, CPURegister.A, 0x42);
		TestLDReg16AddressReg8(CPURegister.HL, CPURegister.A, 0x42);
		TestLDReg16AddressReg8(CPURegister.HL, CPURegister.B, 0x42);
		TestLDReg16AddressReg8(CPURegister.HL, CPURegister.C, 0x42);
		TestLDReg16AddressReg8(CPURegister.HL, CPURegister.D, 0x42);
		TestLDReg16AddressReg8(CPURegister.HL, CPURegister.E, 0x42);
		TestLDReg16AddressReg8(CPURegister.HL, CPURegister.H, 0x42);
		TestLDReg16AddressReg8(CPURegister.HL, CPURegister.L, 0x42);

		TestLDReg8Reg16Address(CPURegister.A, CPURegister.BC, 0x42);
		TestLDReg8Reg16Address(CPURegister.A, CPURegister.DE, 0x42);
		TestLDReg8Reg16Address(CPURegister.A, CPURegister.HL, 0x42);
		TestLDReg8Reg16Address(CPURegister.B, CPURegister.HL, 0x42);
		TestLDReg8Reg16Address(CPURegister.C, CPURegister.HL, 0x42);
		TestLDReg8Reg16Address(CPURegister.D, CPURegister.HL, 0x42);
		TestLDReg8Reg16Address(CPURegister.E, CPURegister.HL, 0x42);
		TestLDReg8Reg16Address(CPURegister.H, CPURegister.HL, 0x42);
		TestLDReg8Reg16Address(CPURegister.L, CPURegister.HL, 0x42);

		foreach (var destinationRegister in registers)
			TestLDReg8U8(destinationRegister, 0x42);
		TestLDReg16AddressU8(CPURegister.HL, Memory.WorkRAMBank0.addr, 0x42);

		TestLDReg8U16Address(CPURegister.A, Memory.WorkRAMBank0.addr, 0x42);
		TestLDU16AddressReg8(CPURegister.A, Memory.WorkRAMBank0.addr, 0x42);

		Console.WriteLine("All tests passed");
	}

	public void Expect(Gameboy gameboy, CPUOperator operation, bool condition)
	{
		if (!condition)
			throw new System.Exception(operation.ToString(gameboy.cpu, gameboy.memory, 0x00, 0x00) + " failed");
		Console.WriteLine(operation.ToString(gameboy.cpu, gameboy.memory, 0x00, 0x00) + " succeeded");
	}
}