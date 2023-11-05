using System;

namespace GBMU.Core;

public class Timers {
	private const float DividerClockSpeed = 16384f;
	private const float DividerCycleDuration = 1.0f / DividerClockSpeed;

	private readonly CPU _cpu;
	private readonly Memory _memory;

	private double _dividerElapsedTime;
	private double _timaElapsedTime;

	public Timers(CPU cpu, Memory memory) {
		_cpu = cpu;
		_memory = memory;
	}

	public void Update(double elapsedTime) {
		if (!_cpu.Stopped) {
			_dividerElapsedTime += elapsedTime;
			if (_dividerElapsedTime >= DividerCycleDuration) {
				_dividerElapsedTime -= DividerCycleDuration;
				var currentDiv = _memory.ReadByte(Memory.DIV.Address);
				_memory.WriteByte(Memory.DIV.Address, (byte)(currentDiv + 1));
			}
		}

		TAC tac = new(_memory.ReadByte(Memory.TAC.Address));
		if (tac.IsTimerEnabled) {
			_timaElapsedTime += elapsedTime;
			if (_timaElapsedTime >= tac.CycleDuration) {
				_timaElapsedTime -= tac.CycleDuration;

				var currentTima = _memory.ReadByte(Memory.TIMA.Address);
				if (currentTima == 0xFF) {
					_memory.WriteByte(Memory.TIMA.Address, _memory.ReadByte(Memory.TMA.Address));
					_memory.RequestInterrupt(Interrupt.Timer);
					return;
				}
				_memory.WriteByte(Memory.TIMA.Address, (byte)(currentTima + 1));
			}
		}
	}
}