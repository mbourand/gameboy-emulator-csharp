using GBMU.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GBMU.UI;

public class GBMUWindow : Game {
	private readonly GraphicsDeviceManager _graphics;
	private SpriteBatch _spriteBatch;

	private Gameboy _gameboy;

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
	}

	protected override void LoadContent() {
		_spriteBatch = new SpriteBatch(GraphicsDevice);
	}

	protected override void Update(GameTime gameTime) {
		if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
			Exit();

		_gameboy.Update(gameTime.ElapsedGameTime.TotalSeconds);
		base.Update(gameTime);
	}

	protected override void Draw(GameTime gameTime) {
		GraphicsDevice.Clear(Color.Black);

		Color[] colors = new Color[WindowWidth * WindowHeight];
		var ppuPixels = _gameboy.PPU.GetScreen();
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
