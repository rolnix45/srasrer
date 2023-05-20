using Irrlicht;
using Irrlicht.Core;
using Irrlicht.Video;
using nook.io;
using nook.main;

namespace nook.entities;

sealed class Player
{
    private static readonly log4net.ILog _logger =
        log4net.LogManager.GetLogger(typeof(Player));
    
    public static readonly List<Bullet> Bullets = new List<Bullet>();
    
    private readonly Texture texture;
    public Vector2Di position { get; }
    public UInt16 scale { get; }

    private readonly IrrlichtDevice device;
    private VideoDriver driver;

    private float nextTimeToFire;
    private const UInt16 fireRate = 250;
    private Int32 speedMul;

    public Player(IrrlichtDevice device) 
    {
        this.device = device;
        driver = device.VideoDriver; 
        
        texture = driver.GetTexture(Game.debugPath + "assets/textures/kutas.png");
        scale = 64;
        position = new Vector2Di(100, (Game.winHeight / 2) - (scale / 2));

        speedMul = 600;
        Game.isPlayerAlive = true;
        
        _logger.Debug("player spawned");
    }

    
    private void HandleBullets()
    {
        Bullet _bullet = new Bullet(ref driver);

        if (Input.keys[(int)KeyCode.LControl] && device.Timer.Time >= nextTimeToFire)
        {
            nextTimeToFire = device.Timer.Time + fireRate;
            _bullet.Create(new Vector2Di(position.X + (scale / 2), position.Y + (scale / 4)));
            Bullets.Add(_bullet);
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
    
    public void Update()
    {
        speedMul = Input.keys[(int)KeyCode.LShift] ? 600 : 400;
        
        if (Input.keys[(int)KeyCode.KeyW])
        {
            position.Y -= (int) (speedMul * Game.frameDeltaTime);
        }
        else if (Input.keys[(int)KeyCode.KeyS])
        {
            position.Y += (int) (speedMul * Game.frameDeltaTime);
        }

        if (Input.keys[(int)KeyCode.KeyA])
        {
            position.X -= (int) (speedMul * Game.frameDeltaTime);
        }
        else if (Input.keys[(int)KeyCode.KeyD])
        {
            position.X += (int) (speedMul * Game.frameDeltaTime);
        }
        
        HandleBullets();
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
        }

        if (Game.isPlayerAlive)
        {
            driver.Draw2DImage(
                texture,
                position,
                new Recti(0, 0, scale, scale),
                null,
                Color.SolidWhite,
                true
            );
        }
    }
}