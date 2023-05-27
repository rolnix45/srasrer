using Irrlicht;
using Irrlicht.Core;
using Irrlicht.Video;
using nook.main;

namespace nook.entities.enemies;

sealed class Imposter : Enemy
{
    private readonly IrrlichtDevice _device;
    
    private const UInt16 speed = 300;
    public override Texture texture { get; }
    public override Vector2Di position { get; }
    public override UInt16 scale { get; }

    public bool canShoot { get; set; }
    
    // TODO: private Particle deathParticles;

    private Imposter(Texture texture, IrrlichtDevice device)
    {
        _device = device;
        this.texture = texture;
        scale = 48;
        position = new Vector2Di();
        health = 1;
    }
    
    public static void SpawnImposter(IrrlichtDevice device)
    {
        Random rnd = new Random();

        var imposter = new Imposter(device.VideoDriver.GetTexture("assets/textures/enemy1.png"), device)
        {
            position =
            {
                X = Game.winWidth + 25
            }
        };

        imposter.position.Y = rnd.Next(imposter.scale, Game.winHeight - imposter.scale);
        
        EnemiesHandler.Enemies.Add(imposter);
    }

    private const ushort firerate = 1250;
    private float nextTimeToFire;
    private void Shoot()
    {
        EnemyBullet enemyBullet = new EnemyBullet(_device.VideoDriver.GetTexture("assets/textures/enemy1Bullet.png"), "imposter");

        if (_device.Timer.Time >= nextTimeToFire)
        {
            nextTimeToFire = _device.Timer.Time + firerate;
            enemyBullet.Create(new Vector2Di(position.X + (scale / 2), position.Y + (scale / 4)), -450);
            EnemiesHandler.EnemyBullets.Add(enemyBullet);
        }
    }
    
    public override void Update(Player player, Enemy enemy)
    {
        base.Update(player, enemy);
        if (position.X < 0 || health == 0)
        {
            EnemiesHandler.Enemies.Remove(this);
            
            if (health != 0) return;
            
            // TODO: DEATH PARTICLES
            Player.score++;
            
            return;
        }

        if (canShoot)
            Shoot();
        
        position.X -= (int) (speed * Game.frameDeltaTime);
    }
}