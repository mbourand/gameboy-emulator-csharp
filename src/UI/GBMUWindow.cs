using System;
using System.Collections.Generic;
using System.Threading;
using GBMU.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GBMU.UI;

public class GBMUWindow : Game {
	private readonly GraphicsDeviceManager _graphics;
	private SpriteBatch _spriteBatch;

	private Gameboy _gameboy;

	private Thread _gameboyThread;

	bool _exit = false;

	public GBMUWindow() {
		Window.Title = "GBMU";
		Window.AllowUserResizing = false;

		_graphics = new GraphicsDeviceManager(this) {
			PreferredBackBufferWidth = 160,
			PreferredBackBufferHeight = 144
		};
		_graphics.ApplyChanges();

		Content.RootDirectory = "Content";
		IsMouseVisible = true;
	}

	protected override void Initialize() {
		base.Initialize();
		InitializeGameboy();
	}

	private void InitializeGameboy() {
		var romStream = System.IO.File.OpenRead(System.Environment.GetCommandLineArgs()[1]);
		_gameboy = new Gameboy(romStream);

		_gameboyThread = new Thread(() => {
			var lastFrame = DateTime.Now;
			while (!_exit) {
				var now = DateTime.Now;
				_gameboy.Update((now - lastFrame).TotalSeconds);
				lastFrame = now;
			}
		});

		_gameboyThread.Start();
	}

	protected override void LoadContent() {
		_spriteBatch = new SpriteBatch(GraphicsDevice);
	}

	protected override void Update(GameTime gameTime) {
		var keyboardState = Keyboard.GetState();

		if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || keyboardState.IsKeyDown(Keys.Escape)) {
			Exit();
		}

		Dictionary<Keys, JoypadButton> keyMapping = new() {
			{ Keys.A, JoypadButton.A },
			{ Keys.S, JoypadButton.B },
			{ Keys.Enter, JoypadButton.Start },
			{ Keys.Space, JoypadButton.Select }
		};

		foreach (var (key, button) in keyMapping) {
			_gameboy.Joypad.RequireButtonPress(button, keyboardState.IsKeyDown(key));
		}

		base.Update(gameTime);
	}

	protected override void OnExiting(object sender, EventArgs args) {
		_exit = true;
		_gameboyThread.Join();
		base.OnExiting(sender, args);
	}

	protected override void UnloadContent() {
		base.UnloadContent();
	}

	protected override void Draw(GameTime gameTime) {
		GraphicsDevice.Clear(Color.Black);

		Color[] colors = new Color[WindowWidth * WindowHeight];
		var ppuPixels = _gameboy.PPU.GetDisplayScreen();
		for (int y = 0; y < PPU.ScreenHeight; y++)
			for (int x = 0; x < PPU.ScreenWidth; x++)
				colors[y * PPU.ScreenWidth + x] = new Color(ppuPixels[y][x]);

		Texture2D texture = new(GraphicsDevice, WindowWidth, WindowHeight);
		texture.SetData(colors);

		_spriteBatch.Begin();
		_spriteBatch.Draw(texture, new Rectangle(0, 0, texture.Width, texture.Height), Color.White);
		_spriteBatch.End();

		base.Draw(gameTime);
	}

	public const uint PPUScreenToWindowRatio = 1; // TODO: Finish implementing this
	public const int WindowWidth = (int)(PPU.ScreenWidth * PPUScreenToWindowRatio);
	public const int WindowHeight = (int)(PPU.ScreenHeight * PPUScreenToWindowRatio);
}
