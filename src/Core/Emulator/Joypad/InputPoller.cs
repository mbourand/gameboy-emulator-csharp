

using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace GBMU.Core {
    public class InputPoller {
        private readonly Joypad _joypad;
        private readonly Memory _memory;

        private double _elapsedTime;

        public InputPoller(Memory memory, Joypad joypad) {
            _memory = memory;
            _joypad = joypad;
            _elapsedTime = 0;
        }

        public void Update(double deltaTime) {
            _elapsedTime += deltaTime;
            if (_elapsedTime >= PollingRateDuration) {
                Poll();
                _elapsedTime = 0;
            }
        }

        public void Poll() {
            var keyboardState = Keyboard.GetState();
            JoypadState joypadState = new(_memory.InternalReadByte(Memory.P1.Address));

            if (joypadState.IsListeningToDirectionButtons)
                foreach (var (key, button) in KeypadMapping)
                    _joypad.RequireButtonPress(button, keyboardState.IsKeyDown(key));

            if (joypadState.IsListeningToActionButtons)
                foreach (var (key, button) in ActionMapping)
                    _joypad.RequireButtonPress(button, keyboardState.IsKeyDown(key));
        }

        public readonly Dictionary<Keys, JoypadButton> ActionMapping = new() {
            { Keys.A, JoypadButton.AOrRight },
            { Keys.S, JoypadButton.BOrLeft },
            { Keys.Enter, JoypadButton.StartOrDown },
            { Keys.Space, JoypadButton.SelectOrUp },
        };

        public readonly Dictionary<Keys, JoypadButton> KeypadMapping = new() {
            { Keys.Right, JoypadButton.AOrRight },
            { Keys.Left, JoypadButton.BOrLeft },
            { Keys.Up, JoypadButton.SelectOrUp },
            { Keys.Down, JoypadButton.StartOrDown }
        };

        public const float PollingRate = 1000f;
        public const double PollingRateDuration = 1.0 / PollingRate;
    }
}
