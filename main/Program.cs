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
    internal static float frameDeltaTime;
    internal static string debugPath = "../../../";
    
    private IrrlichtDevice device;

    private readonly VideoDriver driver;
    private readonly SceneManager smgr;
    private readonly GUIEnvironment gui;
    private readonly EnemiesHandler enmHandler;

    private readonly GUIFont font;

    private Player _player;
    private EnTest enTest;
    
    public static bool isPlayerAlive = true;

    public Game()
    {
        XmlConfigurator.Configure(File.OpenRead(debugPath + "log4net.config"));

        IrrlichtCreationParameters parameters = new IrrlichtCreationParameters
        {
            DriverType = DriverType.Direct3D9,
            AntiAliasing = 8,
            VSync = true,
            WindowSize = new Dimension2Di(winWidth, winHeight), 
        };
        
        device = IrrlichtDevice.CreateDevice(parameters);

        // ReSharper disable once ObjectCreationAsStatement
        new Input(device);

        device.SetWindowCaption("sraczka");

        driver = device.VideoDriver;
        smgr = device.SceneManager;
        gui = device.GUIEnvironment;

        font = gui.GetFont(debugPath + "assets/fonts/font.png");

        _player = new Player(device);

        enTest = new EnTest(
            driver.GetTexture(debugPath + "assets/textures/enemy1.png"),
            device);

        enmHandler = new EnemiesHandler(driver);

        _logger.Info("Initialized!");
    }

    private void DebugInfo()
    {
        font.Draw("FPS " + driver.FPS, new Vector2Di(5, 5), Color.SolidWhite);
        font.Draw("DLT " + frameDeltaTime, new Vector2Di(5, 20), Color.SolidWhite);
        font.Draw("FTI " + frameDeltaTime * 1000, new Vector2Di(5, 35), Color.SolidWhite);
        font.Draw("PBU " + Player.Bullets.Count, new Vector2Di(5, 50), Color.SolidWhite);
        font.Draw("ENM " + EnemiesHandler.Enemies.Count, new Vector2Di(5, 65), Color.SolidWhite);
        font.Draw("POS " + _player.position.X + " " + _player.position.Y, new Vector2Di(5, 80), Color.SolidWhite);
        if (!isPlayerAlive)
        {
            font.Draw("DEAD", new Vector2Di(winWidth / 2, winHeight / 2), Color.SolidWhite);
        }
    }
    
    private void Draw()
    {
        DebugInfo();
        
        _player.Draw();
        enmHandler.Draw();
        
        smgr.DrawAll();
        gui.DrawAll();
    }


    private float timeToSpawn;
    private UInt16 spawnRate = 500;
    private void Update()
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

        if (Input.keys[(int)KeyCode.F2] && device.Timer.Time >= timeToSpawn)
        {
            timeToSpawn = device.Timer.Time + spawnRate;
            enTest.Spawn();
        }
        
        if (!isPlayerAlive) return;

        foreach (var enemy in EnemiesHandler.Enemies.ToList())
        {
            switch (enemy)
            {
                case EnTest b:
                    b.Update(ref _player);
                    break;
            }
        }
        
        _player.Update();
    }
    
    public void Run()
    {
        smgr.AddCameraSceneNode(null, new Vector3Df(0, 0, -1), new Vector3Df());

        UInt32 then = device.Timer.Time;
        while (device.Run())
        {
            UInt32 now = device.Timer.Time;
            frameDeltaTime = (now - then) / 1000f;
            then = now;
            Update();
            
            driver.BeginScene(ClearBufferFlag.All, Color.SolidBlack);
            Draw();
            driver.EndScene();
        }

        device.Drop();
        _logger.Info("Closing...");
    }
}

static class Program
{
    public static void Main()
    {
        Game game = new();
        game.Run();
    }
}