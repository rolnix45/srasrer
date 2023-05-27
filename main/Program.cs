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
    
    internal static readonly ushort winWidth = 1280;
    internal static readonly ushort winHeight = 960;
    internal static readonly double frameDeltaTime = 0.0166;
    
    private readonly IrrlichtDevice device;

    private readonly VideoDriver driver;
    private readonly SceneManager smgr;
    private readonly GUIEnvironment gui;
    private readonly EnemiesHandler enmHandler;

    private readonly GUIFont font;
    private readonly GUIFont bigFont;
    private readonly Texture background;

    private Player _player;
    private Boss1 _boss1;
    
    public static bool isPlayerAlive = true;
    private bool showDebugInfo = true;
    public static bool showHitboxes { get; private set; }
    
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
        
        device = IrrlichtDevice.CreateDevice(parameters);
        device.CursorControl.Visible = false;

        // ReSharper disable once ObjectCreationAsStatement
        new Input(device);

        device.SetWindowCaption("sraczka");

        driver = device.VideoDriver;
        smgr = device.SceneManager;
        gui = device.GUIEnvironment;

        font = gui.GetFont("assets/fonts/font.png");
        bigFont = gui.GetFont("assets/fonts/bigFont.png");
        background = driver.GetTexture("assets/textures/background.png");

        _player = new Player(device);
        _boss1 = new Boss1(device, _player);

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
        font.Draw("EBU " + EnemiesHandler.EnemyBullets.Count, new Vector2Di(5, 50), Color.SolidWhite);
        font.Draw("ENM " + EnemiesHandler.Enemies.Count, new Vector2Di(5, 65), Color.SolidWhite);
        font.Draw("POS " + _player.position.X + " " + _player.position.Y, new Vector2Di(5, 80), Color.SolidWhite);
        font.Draw("TIM " + device.Timer.Time, new Vector2Di(5, 95), Color.SolidWhite);
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
        font.Draw("SCORE: " + Player.score, new Vector2Di(5, winHeight - 25), Color.SolidWhite);
        
        enmHandler.Draw();
        
        smgr.DrawAll();
        gui.DrawAll();
        
        if (!isPlayerAlive)
        {
            bigFont.Draw("DEAD", new Vector2Di((winWidth / 2) - 32, (winHeight / 2) - 16), Color.SolidWhite);
        }
    }

    public static void KillPlayer()
    {
        isPlayerAlive = false;
        _logger.Debug("player dead");
    }
    
    private void UpdateEnemies()
    {
        foreach (var enemy in EnemiesHandler.Enemies.ToList())
        {
            switch (enemy)
            {
                case Imposter b:
                    b.canShoot = _boss1.health == 0;
                    b.Update(_player, enemy);
                    break;
                case Boss1 b:
                    b.Update(_player, enemy);
                    break;
            }
        }

        foreach (var bullet in EnemiesHandler.EnemyBullets.ToList())
        {
            switch (bullet.tag)
            {
                case "boss1" or "imposter":
                    if (bullet.position.X > 0 && bullet.isAlive)
                    {
                        bullet.position.X += (int)(bullet.deltaX * Game.frameDeltaTime);
                        bullet.position.Y += (int)(bullet.deltaY * Game.frameDeltaTime);
                    }
                    else
                    {
                        EnemiesHandler.EnemyBullets.Remove(bullet);
                    }
                    break;
            }
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
            _boss1 = new Boss1(device, _player);
            boss1Spawned = false;
            UpdateEnemies();
            Player.score = 0;
        }

        if (Input.keys[(int)KeyCode.F1])
        {
            KillPlayer();
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

    private bool boss1Spawned;
    private void Spawn()
    {
        if (device.Timer.Time >= timeToSpawn)
        {
            timeToSpawn = device.Timer.Time + spawnRate;
            Imposter.SpawnImposter(device);
        }

        if (Player.score == 20 && !boss1Spawned)
        {
            _boss1.Spawn();
            boss1Spawned = true;
        }
    }
    
    private void Update()
    {
        Events();
        
        if (!isPlayerAlive) return;
        
        UpdateEnemies();
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