using Irrlicht;
using Irrlicht.Core;
using Irrlicht.GUI;
using Irrlicht.Scene;
using Irrlicht.Video;
using log4net.Config;
using nook.entities;
using nook.io;
using nook.scenes;

namespace nook.main;

sealed class Game
{
    private static readonly log4net.ILog _logger =
        log4net.LogManager.GetLogger(typeof(Game));
    
    internal static readonly ushort winWidth = 1280;
    internal static readonly ushort winHeight = 960;
    internal static double frameDeltaTime = 0.0166;
    
    private readonly IrrlichtDevice _device;

    private readonly VideoDriver _driver;
    private readonly SceneManager _sceneManager;
    private readonly GUIEnvironment _guiEnv;

    private readonly TitleScreen _titleScreen;
    private readonly GameScene _scene;
    
    public static State gameState { get; internal set; }
    
    public Game(string[] args)
    {
        XmlConfigurator.Configure(File.OpenRead("log4net.config"));
        _logger.Info("Starting...");

        IrrlichtCreationParameters parameters = new IrrlichtCreationParameters
        {
            DriverType = DriverType.Direct3D9,
            AntiAliasing = 8,
            VSync = false,
            WindowSize = new Dimension2Di(winWidth, winHeight),
        };

        ParseCliArguments(args);
        
        _device = IrrlichtDevice.CreateDevice(parameters);
        _device.CursorControl.Visible = false;

        // ReSharper disable once ObjectCreationAsStatement
        new Input(_device);

        _device.SetWindowCaption("sraczka");

        _driver = _device.VideoDriver;
        _sceneManager = _device.SceneManager;
        _guiEnv = _device.GUIEnvironment;

        _scene = new GameScene(_device);

        _titleScreen = new TitleScreen(_device);
        
        _logger.Debug("DEBUG");
        _logger.Info("INFO");
        _logger.Warn("WARNING");
        _logger.Error("ERROR");
        _logger.Fatal("FATAL");
        
        _logger.Info("Initialized!");
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write("Debug Controlls: \n" +
                      "F1 = Kill Player\n" +
                      "F2 = nothing\n" +
                      "F3 = Debug Info\n" + 
                      "F4 = Hitboxes\n" +
                      "hold some keys for while\n");
        Console.ForegroundColor = ConsoleColor.White;
        gameState = State.Title;
    }

    private void ParseCliArguments(string[] args)
    {
        _logger.Debug("arguments count " + args.Length);
        foreach (var arg in args)
        {
            _logger.Debug(arg);
                
            Player.useKeyboard = arg == "--keyboard";

            /*
            (arg == "--fastmode" ? 0.0332 : 
                (arg == "--slowmo" ? 0.0083 : 0.0166));
            */
        }
    }

    private void Draw()
    {
        switch (gameState)
        {
            case State.Running:
                _scene.CommonDraw();
                break;
            case State.Dead:
                _scene.CommonDraw();
                _scene.DeadDraw();
                break;
            case State.Pause:
                _scene.CommonDraw();
                _scene.PausedDraw();
                break;
            case State.Title:
                _titleScreen.Draw();
                break;
        }
        
        _sceneManager.DrawAll();
        _guiEnv.DrawAll();
    }
    
    private void Update()
    {
        switch (gameState)
        {
            case State.Running:
                _scene.Update();
                break;
            case State.Dead:
                _scene.PausedEvents();
                break;
            case State.Pause:
                _scene.PausedEvents();
                break;
            case State.Title:
                _titleScreen.Update();
                break;
        }
    }

    /*
     * DALEJ NIE PATRZEC
     * BURDEL W CHUJ
     */
    private const double limitFPS = 1000.0 / 60.0;
    public void Run()
    {
        _sceneManager.AddCameraSceneNode(null, new Vector3Df(0, 0, -1), new Vector3Df());

        double lastTime = _device.Timer.Time;
        double timer = 0.0;

        double then = _device.Timer.Time;
        while (_device.Run())
        {
            var nowTime = _device.Timer.Time;
            timer += (nowTime - lastTime) / limitFPS;
            lastTime = nowTime;
            
            while (timer >= 1.0)
            {
                var now = _device.Timer.Time;
                frameDeltaTime = (now - then) / 1000.0;
                then = now;
                
                Update();
                
                _driver.BeginScene(ClearBufferFlag.All, new Color(69, 34, 35));
                Draw();
                _driver.EndScene();

                timer--;
            }
        }
        
        _device.Drop();
        _titleScreen.Cleanup();
        _scene.Cleanup();
        _logger.Info("Closing...");
    }
}

static class Program
{
    public static void Main(string[] args)
    {
        Game game = new(args);
        game.Run();
    }
}