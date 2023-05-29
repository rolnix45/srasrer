using Irrlicht;
using Irrlicht.Core;
using Irrlicht.GUI;
using Irrlicht.Video;
using log4net;
using nook.audio;
using nook.entities;
using nook.entities.enemies;
using nook.io;
using nook.main;

namespace nook.scenes;

sealed class GameScene
{
    private static readonly ILog _logger =
        LogManager.GetLogger(typeof(GameScene));
    
    public static bool showHitboxes { get; private set; }
    private bool showDebugInfo = true;

    private readonly IrrlichtDevice _device;
    private readonly VideoDriver _driver;

    private Player _player { get; set; }
    private Boss1 _boss1 { get; set; }
    private readonly EnemiesHandler _enemiesHandler;
    
    private readonly Texture _background;
    private readonly GUIFont _font;
    private readonly GUIFont _bigFont;

    public GameScene(IrrlichtDevice device)
    {
        this._device = device;
        _driver = _device.VideoDriver;
        var guiEnv = _device.GUIEnvironment;
        
        _player = new Player(_device);
        _boss1 = new Boss1(_device, _player);
        _enemiesHandler = new EnemiesHandler(_driver);
        
        _background = _driver.GetTexture("assets/textures/background.png");
        _font = guiEnv.GetFont("assets/fonts/font.png");
        _bigFont = guiEnv.GetFont("assets/fonts/bigFont.png");
    }
    
    public static void KillPlayer()
    {
        Game.gameState = State.Dead;
        _logger.Debug("player dead");
    }
    
    private void RunningEvents()
    {
        if (Input.IsKeyDown(KeyCode.F1))
        {
            KillPlayer();
        }

        if (Input.IsKeyDown(KeyCode.F3) && _device.Timer.Time >= timeToSpawn)
        {
            showDebugInfo = !showDebugInfo;
        }

        if (Input.IsKeyDown(KeyCode.F4) && _device.Timer.Time >= timeToSpawn)
        {
            showHitboxes = !showHitboxes;
        }

        if (Input.IsKeyDown(KeyCode.Esc))
        {
            Game.gameState = State.Pause;
        }
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
    private bool boss1Spawned;
    private void Spawn()
    {
        if (_device.Timer.Time >= timeToSpawn)
        {
            timeToSpawn = _device.Timer.Time + spawnRate;
            Imposter.SpawnImposter(_device);
        }

        if (Player.score == 20 && !boss1Spawned)
        {
            _boss1.Spawn();
            boss1Spawned = true;
        }
    }
    
    public void PausedEvents()
    {
        if (Input.IsKeyDown(KeyCode.Backspace))
        {
            _enemiesHandler.KillAllEnemiesAndBullets();
            _player = new Player(_device);
            _boss1 = new Boss1(_device, _player);
            boss1Spawned = false;
            UpdateEnemies();
            Game.gameState = State.Running;
            Player.score = 0;
        }

        if (Input.IsKeyDown(KeyCode.Esc) && Game.gameState == State.Pause)
        {
            Game.gameState = State.Running;
        }

        if (Input.IsKeyDown(KeyCode.Esc) && Input.IsKeyDown(KeyCode.LShift))
        {
            _device.Close();
        }
    }
    
    public void Update()
    {
        RunningEvents();
        UpdateEnemies();
        Spawn();
        _player.Update();
    }

    private void DebugInfo()
    {
        _font.Draw(
            $"FPS {_driver.FPS}\n" +
            $"DLT {Game.frameDeltaTime}\n" +
            $"PBU {Player.Bullets.Count}\n" +
            $"EBU {EnemiesHandler.EnemyBullets.Count}\n" +
            $"ENM {EnemiesHandler.Enemies.Count}\n" +
            $"POS {_player.position.X} {_player.position.Y}\n" +
            $"TIM {_device.Timer.Time}",
            
            new Vector2Di(5, 5), 
            Color.SolidWhite);
    }

    public void CommonDraw()
    {
        _driver.Draw2DImage(
            _background,
            new Vector2Di(0, 0)
        );

        _font.Draw(
            "SCORE: " + Player.score,
            new Vector2Di(5, Game.winHeight - 25),
            Color.SolidWhite
        );
        
        _enemiesHandler.Draw();
        _player.Draw();

        if (showDebugInfo)
            DebugInfo();
    }

    public void DeadDraw()
    {
        _font.Draw(
            $"HOLD LSHIFT AND ESC TO CLOSE\n{"", 2}TO RESTART PRESS BACKSPACE",
            new Vector2Di(Game.winWidth - 412, 5),
            Color.SolidWhite
        );
        
        _bigFont.Draw(
            "DEAD", 
            new Vector2Di((Game.winWidth / 2) - 32, (Game.winHeight / 2) - 16), 
            Color.SolidWhite
        );
    }

    public void PausedDraw()
    {
        _font.Draw(
            $"{"", 22} RETURN WITH ESC\nHOLD LSHIFT AND ESC TO CLOSE\n{"", 2}TO RESTART PRESS BACKSPACE",
            new Vector2Di(Game.winWidth - 412, 5),
            Color.SolidWhite
        );
        
        _bigFont.Draw(
            "PAUSED",
            new Vector2Di((Game.winWidth / 2) - 32, (Game.winHeight / 2) - 16),
            Color.SolidWhite
        );
    }

    public void Cleanup()
    {
        _player.Cleanup();
        _enemiesHandler.Cleanup();
        _background.Drop();
        _font.Drop();
        _bigFont.Drop();
        AudioEngine.Instance.Dispose();
    }
}