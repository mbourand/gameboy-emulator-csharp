using System.Runtime.CompilerServices;

namespace GBMU.Core;

public class PPUScreens {
    private readonly uint[] _screen1;
    private readonly uint[] _screen2;

    private readonly uint _width;
    private readonly uint _height;

    private int _screenIndex = 0;

    public PPUScreens(uint width, uint height) {
        _width = width;
        _height = height;

        _screen1 = new uint[_width * _height];
        _screen2 = new uint[_width * _height];

        for (int i = 0; i < _screen1.Length; i++) {
            _screen1[i] = PPU.White;
            _screen2[i] = PPU.White;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Swap() {
        _screenIndex = (_screenIndex + 1) % 2;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint[] GetDisplayScreen() {
        return _screenIndex == 0 ? _screen1 : _screen2;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetPixel(uint x, uint y, uint color) {
        var currentScreen = GetDisplayScreen();
        currentScreen[y * _width + x] = color;
    }
}
