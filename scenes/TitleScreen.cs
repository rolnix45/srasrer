using Irrlicht;
using Irrlicht.Core;
using Irrlicht.GUI;
using Irrlicht.Video;
using nook.audio;
using nook.io;
using nook.main;

namespace nook.scenes;

sealed class TitleScreen
{
    private readonly IrrlichtDevice _device;
    private readonly VideoDriver _driver;

    private readonly Texture _background;
    private readonly GUIFont _bigFont;

    public TitleScreen(IrrlichtDevice device)
    {
        this._device = device;
        _driver = _device.VideoDriver;
        var guiEnv = _device.GUIEnvironment;

        _background = _driver.GetTexture("assets/textures/menuBackground.png");
        _bigFont = guiEnv.GetFont("assets/fonts/bigFont.png");
    }

    public void Update()
    {
        if (Input.IsKeyDown(KeyCode.Esc))
        {
            _device.Close();
        }

        if (Input.IsKeyDown(KeyCode.Return))
        {
            AudioEngine.Instance.PlaySound(new CachedSound("assets/sounds/select.wav"));
            Game.gameState = State.Running;
            _device.CursorControl.Position = new Vector2Di(100, (Game.winHeight / 2) - 16);
        }
    }

    public void Draw()
    {
        _driver.Draw2DImage(
            _background,
            new Vector2Di(0, 0)
        );

        var textXPosition = (Math.PI / 180.0) * (Math.Sin(_device.Timer.Time / 200f)) * 6500;
        _bigFont.Draw(
            "NACISNIJ  ENTER",
            new Vector2Di((int)textXPosition + (Game.winWidth / 2 - 190), (Game.winHeight / 2) + 200),
            Color.SolidCyan
        );
    }

    public void Cleanup()
    {
        _background.Drop();
        _bigFont.Drop();
    }
}