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

    private Texture2D _texture;


    bool _exit = false;

    public GBMUWindow() {
        Window.Title = "GBMU";
        Window.AllowUserResizing = false;

        _graphics = new(this) {
            PreferredBackBufferWidth = WindowWidth,
            PreferredBackBufferHeight = WindowHeight
        };
        _graphics.SynchronizeWithVerticalRetrace = false;
        _graphics.ApplyChanges();

        IsFixedTimeStep = false;

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

        _texture = new(GraphicsDevice, (int)PPU.ScreenWidth, (int)PPU.ScreenHeight);

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

        JoypadState joypadState = new(_gameboy.Memory.InternalReadByte(Memory.P1.Address));

        if (joypadState.IsListeningToActionButtons)
            foreach (var (key, button) in ActionMapping)
                _gameboy.Joypad.RequireButtonPress(button, keyboardState.IsKeyDown(key));

        if (joypadState.IsListeningToDirectionButtons)
            foreach (var (key, button) in KeypadMapping)
                _gameboy.Joypad.RequireButtonPress(button, keyboardState.IsKeyDown(key));

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

        Color[] colors = new Color[PPU.ScreenWidth * PPU.ScreenHeight];
        var ppuPixels = _gameboy.PPU.GetDisplayScreen();
        for (int y = 0; y < PPU.ScreenHeight; y++)
            for (int x = 0; x < PPU.ScreenWidth; x++)
                colors[y * PPU.ScreenWidth + x] = new Color(ppuPixels[y * PPU.ScreenWidth + x]);

        _texture.SetData(colors);

        _spriteBatch.Begin();
        _spriteBatch.Draw(_texture, Vector2.Zero, new Rectangle(0, 0, _texture.Width, _texture.Height), Color.White, 0, Vector2.Zero, PPUScreenToWindowRatio, SpriteEffects.None, 0);
        _spriteBatch.End();

        base.Draw(gameTime);
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

    public const int PPUScreenToWindowRatio = 3;
    public const int WindowWidth = (int)(PPU.ScreenWidth * PPUScreenToWindowRatio);
    public const int WindowHeight = (int)(PPU.ScreenHeight * PPUScreenToWindowRatio);
}
