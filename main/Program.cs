using Irrlicht;
using Irrlicht.Core;
using Irrlicht.GUI;
using Irrlicht.Scene;
using Irrlicht.Video;
using log4net.Config;
using nook.entities;
using nook.entities.enemies;
using nook.io;


namespace nook.main;

sealed class Game
{
    private static readonly log4net.ILog _logger =
        log4net.LogManager.GetLogger(typeof(Game));
    
    internal static readonly UInt16 winWidth = 1280;
    internal static readonly UInt16 winHeight = 960;
    internal static readonly double frameDeltaTime = 0.0166;
    internal static string debugPath = "../../../";
    
    private readonly IrrlichtDevice device;

    private readonly VideoDriver driver;
    private readonly SceneManager smgr;
    private readonly GUIEnvironment gui;
    private readonly EnemiesHandler enmHandler;

    private readonly GUIFont font;
    private readonly Texture background;

    private Player _player;
    
    public static bool isPlayerAlive = true;
    private bool showDebugInfo = true;
    public static bool showHitboxes { get; private set; }
    
    public Game(string[] args)
    {
        XmlConfigurator.Configure(File.OpenRead(debugPath + "log4net.config"));
        _logger.Info("Starting...");

        IrrlichtCreationParameters parameters = new IrrlichtCreationParameters
        {
            DriverType = DriverType.Direct3D9,
            AntiAliasing = 8,
            VSync = false,
            WindowSize = new Dimension2Di(winWidth, winHeight),
        };
        
        device = IrrlichtDevice.CreateDevice(parameters);
        device.CursorControl.Visible = false;

        // ReSharper disable once ObjectCreationAsStatement
        new Input(device);

        device.SetWindowCaption("sraczka");

        driver = device.VideoDriver;
        smgr = device.SceneManager;
        gui = device.GUIEnvironment;

        font = gui.GetFont(debugPath + "assets/fonts/font.png");
        background = driver.GetTexture(debugPath + "assets/textures/background.png");

        _player = new Player(device);

        enmHandler = new EnemiesHandler(driver);

        ParseCliArguments(args);
        
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
    }

    private void ParseCliArguments(string[] args)
    {
        _logger.Debug("arguments count " + args.Length);
        foreach (var arg in args)
        {
            _logger.Debug(arg);
            if (arg == "--kbd")
            {
                _player.useKeyboard = true;
            }
        }
    }
    
    private void DebugInfo()
    {
        font.Draw("FPS " + driver.FPS, new Vector2Di(5, 5), Color.SolidWhite);
        font.Draw("DLT " + frameDeltaTime, new Vector2Di(5, 20), Color.SolidWhite);
        font.Draw("PBU " + Player.Bullets.Count, new Vector2Di(5, 35), Color.SolidWhite);
        font.Draw("ENM " + EnemiesHandler.Enemies.Count, new Vector2Di(5, 50), Color.SolidWhite);
        font.Draw("POS " + _player.position.X + " " + _player.position.Y, new Vector2Di(5, 65), Color.SolidWhite);
        font.Draw("TIM " + device.Timer.Time + " " + device.Timer.Time / 1000.0, new Vector2Di(5, 80), Color.SolidWhite);
    }
    
    private void Draw()
    {
        driver.Draw2DImage(
            background,
            new Vector2Di(0, 0)
        );
        
        if (showDebugInfo)
            DebugInfo();
        
        _player.Draw();
        enmHandler.Draw();
        
        smgr.DrawAll();
        gui.DrawAll();
        
        if (!isPlayerAlive)
        {
            font.Draw("DEAD", new Vector2Di((winWidth / 2) - 32, (winHeight / 2) - 16), Color.SolidWhite);
        }
    }
    
    private float timeToSpawn;
    private const UInt16 spawnRate = 500;
    private void Events()
    {
        if (Input.keys[(int)KeyCode.Esc])
        {
            device.Close();
        }

        if (Input.keys[(int)KeyCode.Backspace] && !isPlayerAlive)
        {
            enmHandler.KillAllEnemiesAndBullets();
            _player = new Player(device);
        }

        if (Input.keys[(int)KeyCode.F1])
        {
            isPlayerAlive = false;
        }

        if (Input.keys[(int)KeyCode.F3] && device.Timer.Time >= timeToSpawn)
        {
            showDebugInfo = !showDebugInfo;
        }

        if (Input.keys[(int)KeyCode.F4] && device.Timer.Time >= timeToSpawn)
        {
            showHitboxes = !showHitboxes;
        }
    }
    
    
    private void Spawn()
    {
        if (device.Timer.Time >= timeToSpawn)
        {
            timeToSpawn = device.Timer.Time + spawnRate;
            EnTest.SpawnEnTest(device);
        }
    }
    
    private void Update()
    {
        Events();
        
        if (!isPlayerAlive) return;
        
        foreach (var enemy in EnemiesHandler.Enemies.ToList())
        {
            switch (enemy)
            {
                case EnTest b:
                    b.Update(ref _player, enemy);
                    break;
            }
        }

        Spawn();
        
        _player.Update();
    }


    private const double limitFPS = 1.0 / 60.0;
    public void Run()
    {
        smgr.AddCameraSceneNode(null, new Vector3Df(0, 0, -1), new Vector3Df());

        double lastTime = device.Timer.Time / 1000.0;
        double timer = 0.0;

        while (device.Run())
        {
            var nowTime = device.Timer.Time / 1000.0;
            timer += (nowTime - lastTime) / limitFPS;
            lastTime = nowTime;

            while (timer >= 1.0)
            {
                Update();
                
                driver.BeginScene(ClearBufferFlag.All, Color.SolidBlack);
                Draw();
                driver.EndScene();

                timer--;
            }
        }

        device.Drop();
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
