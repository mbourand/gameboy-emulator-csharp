namespace GBMU.Core;

public class PPUScreens {
	private readonly uint[][] _screen1;
	private readonly uint[][] _screen2;

	private int _screenIndex = 0;

	public PPUScreens(uint width, uint height) {
		_screen1 = new uint[height][];
		_screen2 = new uint[height][];
		for (int i = 0; i < height; i++) {
			_screen1[i] = new uint[width];
			_screen2[i] = new uint[width];
		}

		for (int i = 0; i < height; i++) {
			for (int j = 0; j < width; j++) {
				_screen1[i][j] = PPU.White;
				_screen2[i][j] = PPU.White;
			}
		}
	}

	public void Swap() {
		_screenIndex = (_screenIndex + 1) % 2;
	}

	public uint[][] GetDisplayScreen() {
		return _screenIndex == 0 ? _screen1 : _screen2;
	}

	public void SetPixel(uint x, uint y, uint color) {
		if (_screenIndex == 0) {
			_screen2[y][x] = color;
		} else {
			_screen1[y][x] = color;
		}
	}

}