using Irrlicht;
using Irrlicht.Core;
using Irrlicht.Video;
using nook.io;
using nook.main;
using nook.scenes;

namespace nook.entities;

sealed class Player
{
    private static readonly log4net.ILog _logger =
        log4net.LogManager.GetLogger(typeof(Player));
    
    public static readonly List<Bullet> Bullets = new List<Bullet>();

    public static int score;

    private readonly Texture mainTexture;
    private readonly Texture emptyTexture;
    
    private Texture texture;
    public Vector2Di position { get; }
    public UInt16 scale { get; }

    private readonly IrrlichtDevice device;
    private VideoDriver driver;

    private float nextTimeToFire;
    private const UInt16 fireRate = 250;
    private Int32 speedMul;

    public static bool useKeyboard { get; set; }

    public Player(IrrlichtDevice device) 
    {
        this.device = device;
        driver = device.VideoDriver; 
        
        texture = driver.GetTexture("assets/textures/kutas.png");
        emptyTexture = driver.GetTexture("assets/textures/e.png");
        mainTexture = texture;
        
        scale = 64;
        position = new Vector2Di(100, (Game.winHeight / 2) - (scale / 2));

        speedMul = 600;
        useKeyboard = false;

        score = 0;
        
        Blink(8);
        
        _logger.Debug("player spawned");
    }

    private void HandleBullets()
    {
        PlayerBullet playerBullet = new PlayerBullet(ref driver);

        if (Input.IsLMBPressed() && device.Timer.Time >= nextTimeToFire)
        {
            nextTimeToFire = device.Timer.Time + fireRate;
            playerBullet.Create(new Vector2Di(position.X + (scale / 2), position.Y + (scale / 4)));
            Bullets.Add(playerBullet);
        }

        foreach (var bullet in Bullets.ToList())
        {
            if (bullet.position.X < Game.winWidth && bullet.isAlive)
            {
                bullet.position.X += (int)(bullet.deltaX * Game.frameDeltaTime);
                bullet.position.Y += (int)(bullet.deltaY * Game.frameDeltaTime);
            }
            else
            {
                Bullets.Remove(bullet);
            }
        }
    }

    private void MoveKeyboard()
    {
        speedMul = Input.IsKeyDown(KeyCode.LShift) ? 600 : 400;
        if (Input.IsKeyDown(KeyCode.KeyW))
        {
            position.Y -= (int) (speedMul * Game.frameDeltaTime);
        }
        else if (Input.IsKeyDown(KeyCode.KeyS))
        {
            position.Y += (int) (speedMul * Game.frameDeltaTime);
        }

        if (Input.IsKeyDown(KeyCode.KeyA))
        {
            position.X -= (int) (speedMul * Game.frameDeltaTime);
        }
        else if (Input.IsKeyDown(KeyCode.KeyD))
        {
            position.X += (int)(speedMul * Game.frameDeltaTime);
        }
    }

    private void MoveMouse()
    {
        Vector2Di centerizedPlayerPosition = new Vector2Di(
            position.X + (scale / 2),
            position.Y + (scale / 4)
        );
        centerizedPlayerPosition.Set(new Vector2Di().Interpolate(Input.mousePos, centerizedPlayerPosition, 0.075));
        position.Set(centerizedPlayerPosition.X - (scale / 2), centerizedPlayerPosition.Y - (scale / 4));
    }
    
    public void Update()
    {
        if (useKeyboard)
        {
            MoveKeyboard();
        }
        else
        {
            MoveMouse();
        }
        
        HandleBullets();
    }

    private void Blink(UInt16 t)
    {
        async Task Blinking()
        {
            await Task.Run(delegate
            {
                for (var i = 0; i <= t; i++)
                {
                    texture = emptyTexture;
                    Task.Delay(75).Wait();
                    texture = mainTexture;
                    Task.Delay(75).Wait();
                }
            });
        }

        if (Blinking().IsCompleted)
        {
            texture = mainTexture;
            Blinking().Dispose();
        }
    }

    public void Draw()
    {
        foreach (var bullet in Bullets)
        {
            driver.Draw2DImage(
                bullet.texture,
                bullet.position,
                new Recti(0, 0, bullet.scale, bullet.scale),
                null,
                Color.SolidWhite,
                true
            );
            
            if (!GameScene.showHitboxes) continue;
            
            driver.Draw2DRectangleOutline(
                new Recti(bullet.position, new Dimension2Di(bullet.scale)),
                Color.SolidMagenta
            );
        }

        if (Game.gameState == State.Dead) return;
        
        driver.Draw2DImage(
            texture,
            position,
            new Recti(0, 0, scale, scale),
            null,
            Color.SolidWhite,
            true
        );

        if (!GameScene.showHitboxes) return;
        
        driver.Draw2DRectangleOutline(
            new Recti(position, new Dimension2Di(scale)),
            Color.SolidBlue
        );
    }

    public void Cleanup()
    {
        foreach (var bullet in Bullets)
        {
            bullet.Cleanup();
        }
        
        texture.Drop();
        position.Dispose();
        mainTexture.Drop();
        emptyTexture.Drop();
    }
}