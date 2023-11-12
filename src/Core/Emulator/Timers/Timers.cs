using System;

namespace GBMU.Core;

public class Timers {
	private readonly CPU _cpu;
	private readonly Memory _memory;

	private double _timaOverflowElapsedTime;
	private double _internalTimerElapsedTime;
	private ushort _internalTimer;

	private bool _wasTimerBitToCheckOne;

	private int? _timaOverflowDelay;

	private int _timaWasResetThisCycleCounter;
	public bool TimaWasResetThisCycle => _timaWasResetThisCycleCounter > 0;

	public void ResetInternalTimer() {
		_internalTimer = 0;
		_timaOverflowDelay = null;
		_timaWasResetThisCycleCounter = 0;
	}

	public Timers(CPU cpu, Memory memory) {
		_cpu = cpu;
		_memory = memory;
		_internalTimerElapsedTime = 0;
		_timaOverflowElapsedTime = 0;
		_internalTimer = InternalTimerDefaultValue;
		_wasTimerBitToCheckOne = false;
		_timaWasResetThisCycleCounter = 0;
	}

	public void Update(double elapsedTime) {
		HandleTimaOverflow(elapsedTime);
		HandleInternalTimerIncrement(elapsedTime);
		HandleTIMAIncrement();
	}

	public void CancelTimaOverflow() {
		_timaOverflowDelay = null;
	}

	private void HandleTimaOverflow(double elapsedTime) {
		_timaOverflowElapsedTime += elapsedTime;
		if (_timaOverflowElapsedTime < CPU.CycleDuration)
			return;

		_timaOverflowElapsedTime -= CPU.CycleDuration;
		_timaWasResetThisCycleCounter = Math.Max(0, _timaWasResetThisCycleCounter - 1);

		if (!_timaOverflowDelay.HasValue)
			return;

		_timaOverflowDelay -= 1;
		if (_timaOverflowDelay > 0)
			return;

		_timaOverflowDelay = null;
		_timaWasResetThisCycleCounter = 4;
		_memory.InternalWriteByte(Memory.TIMA.Address, _memory.InternalReadByte(Memory.TMA.Address));
		_memory.RequestInterrupt(Interrupt.Timer);
	}

	private void HandleInternalTimerIncrement(double elapsedTime) {
		if (_cpu.Stopped)
			return;

		_internalTimerElapsedTime += elapsedTime;
		if (_internalTimerElapsedTime < InternalTimerCycleDuration)
			return;

		_internalTimerElapsedTime -= InternalTimerCycleDuration;
		_internalTimer++;
		_memory.InternalWriteByte(Memory.DIV.Address, ByteUtils.HighByte(_internalTimer));
	}

	private void HandleTIMAIncrement() {
		TAC tac = new(_memory.InternalReadByte(Memory.TAC.Address));

		// Shitty gameboy circuitry implementation
		uint timerBitToCheckForOverflow = (_internalTimer & tac.ClockSpeedWatchBit) != 0 ? 1u : 0u;
		if (!tac.IsTimerEnabled)
			return;

		if (_wasTimerBitToCheckOne && timerBitToCheckForOverflow == 0) {
			_wasTimerBitToCheckOne = false;
			IncrementTIMA();
		} else if (!_wasTimerBitToCheckOne && timerBitToCheckForOverflow == 1) {
			_wasTimerBitToCheckOne = true;
		}
	}

	private void IncrementTIMA() {
		byte tima = _memory.InternalReadByte(Memory.TIMA.Address);
		if (tima == 0xFF)
			_timaOverflowDelay = 4; // Why can't they do anything normally
		_memory.InternalWriteByte(Memory.TIMA.Address, (byte)(tima + 1));
	}

	public const double InternalTimerClockSpeed = CPU.ClockSpeed;
	public const double InternalTimerCycleDuration = 1.0f / InternalTimerClockSpeed;
	public const ushort InternalTimerDefaultValue = 0xABCC;
}