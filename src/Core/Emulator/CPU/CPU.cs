using System;

namespace GBMU.Core;

public partial class CPU {
    public byte A, B, C, D, E, H, L;
    private byte _f;
    private readonly Memory _memory;

    public bool CBPrefix;
    public bool Halted;
    public bool Stopped;

    public bool InterruptMasterEnable {
        get; private set;
    }

    private bool _enableInterruptMasterNextCycle;
    private int _cycleWaitTime;

    public void SetInterruptMasterEnable(bool value) {
        if (value) {
            _enableInterruptMasterNextCycle = true;
        } else {
            _enableInterruptMasterNextCycle = false;
            InterruptMasterEnable = false;
        }
    }

    private double _elapsedTime;

    public CPU(Memory memory) {
        InitializeRegisters();
        _memory = memory;
        _elapsedTime = 0;
        _enableInterruptMasterNextCycle = false;
        _cycleWaitTime = 0;
        Halted = false;
        Stopped = false;
        CBPrefix = false;
    }

    private void InitializeRegisters() {
        AF = AFBaseValue;
        BC = BCBaseValue;
        DE = DEBaseValue;
        HL = HLBaseValue;
        SP = SPBaseValue;
        PC = PCBaseValue;
        InterruptMasterEnable = IMEBaseValue;
    }

    public ushort AF {
        get => ByteUtils.MakeWord(A, _f);
        set => Set16BitRegister(ref A, ref _f, value);
    }

    public ushort BC {
        get => ByteUtils.MakeWord(B, C);
        set => Set16BitRegister(ref B, ref C, value);
    }

    public ushort DE {
        get => ByteUtils.MakeWord(D, E);
        set => Set16BitRegister(ref D, ref E, value);
    }

    public ushort HL {
        get => ByteUtils.MakeWord(H, L);
        set => Set16BitRegister(ref H, ref L, value);
    }

    public ushort SP;
    public ushort PC;

    // Handle the 1 cycle delay of EI instruction
    public void HandleInterruptMasterEnableDelay() {
        if (_enableInterruptMasterNextCycle == true)
            InterruptMasterEnable = true;
        _enableInterruptMasterNextCycle = false;
    }

    public void Cycle() {
        _cycleWaitTime = Math.Max(_cycleWaitTime - 1, 0);
        if (_cycleWaitTime > 0)
            return;

        HandleInterruptMasterEnableDelay();

        if (!Halted && !Stopped) {
            // Fetch
            // var fetchStart = DateTime.Now;
            byte opcode = _memory.ReadByte(PC);
            // _fetchTime += (DateTime.Now - fetchStart).TotalMilliseconds;

            // Decode
            var tmpCBPrefix = CBPrefix;
            CPUOperator instruction = CBPrefix ? InstructionSet.GetCBInstruction(opcode) : InstructionSet.GetInstruction(opcode);
            CBPrefix = false;

            // Execute
            // var tmpPC = PC;
            // var executeStart = DateTime.Now;
            instruction.Execute(this, _memory, opcode);
            instruction.ShiftPC(this, _memory);
            _cycleWaitTime = instruction.GetCycles(tmpCBPrefix, opcode);
            // _executeTime += (DateTime.Now - executeStart).TotalMilliseconds;
            // File.AppendAllText("instructions.txt", $"0x{tmpPC:X4}: " + instruction.ToString(this, _memory, opcode, 0x00) + "\n");
        }

        // var interruptsStart = DateTime.Now;
        HandleInterrupts();
        // _interruptsTime += (DateTime.Now - interruptsStart).TotalMilliseconds;
    }

    public void HandleInterrupts() {
        if (CBPrefix)
            return;

        byte enabledInterrupts = _memory.ReadByte(Memory.InterruptEnableRegister.Address);
        byte triggeredInterrupts = _memory.ReadByte(Memory.InterruptFlagRegister.Address);

        InterruptByte interruptsToRun = new((byte)(enabledInterrupts & triggeredInterrupts));
        if (!interruptsToRun.IsEmpty) {
            Halted = false;
            Stopped = false;
        }

        if (!InterruptMasterEnable)
            return;
        var highestPriorityInterrupt = InterruptUtils.GetHighestPriorityInterrupt(interruptsToRun);

        if (highestPriorityInterrupt.HasValue) {
            InterruptMasterEnable = false;
            byte newInterruptFlag = (byte)(triggeredInterrupts & ~(byte)highestPriorityInterrupt.Value);
            _memory.WriteByte(Memory.InterruptFlagRegister.Address, newInterruptFlag);

            PushToStack(PC);
            PC = InterruptUtils.GetVectorFromInterrupt(highestPriorityInterrupt.Value).Address;
        }
    }

    public void Update(double deltaTime) {
        _elapsedTime += deltaTime;
        if (_elapsedTime >= CycleDuration) {
            Cycle();
            _elapsedTime -= CycleDuration;
        }
    }

    private static void Set16BitRegister(ref byte high, ref byte low, ushort value) {
        high = ByteUtils.HighByte(value);
        low = ByteUtils.LowByte(value);
    }

    public void ResetFlags() {
        _f = 0;
    }

    public void SetFlag(CPUFlag flag, bool value) {
        if (value)
            _f |= (byte)flag;
        else
            _f &= (byte)~flag;
    }

    public bool GetFlag(CPUFlag flag) => (_f & (byte)flag) != 0;

    public void PushToStack(ushort value) {
        SP -= 2;
        _memory.WriteWord(SP, value);
    }

    public ushort PopFromStack() {
        ushort value = _memory.ReadWord(SP);
        SP += 2;
        return value;
    }

    public string DebugString() => $"AF: {AF:X4} BC: {BC:X4} DE: {DE:X4} HL: {HL:X4} SP: {SP:X4} PC: {PC:X4} F: {Convert.ToString(_f, 2).PadLeft(8, '0')}";

    public const ushort AFBaseValue = 0x01B0;
    public const ushort BCBaseValue = 0x0013;
    public const ushort DEBaseValue = 0x00D8;
    public const ushort HLBaseValue = 0x014D;
    public const ushort SPBaseValue = 0xFFFE;
    public const ushort PCBaseValue = 0x0100;

    public const bool IMEBaseValue = false;

    public const float ClockSpeed = 4194304f;
    public const float CycleDuration = 1f / ClockSpeed;
}
