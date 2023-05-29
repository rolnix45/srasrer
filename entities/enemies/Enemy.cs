using Irrlicht.Core;
using Irrlicht.Video;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using nook.audio;
using nook.scenes;

namespace nook.entities.enemies;

/* DEATH CAUSES
 * 0 = BULLET
 * 1 = COLLISION WITH PLAYER
 */

abstract class Enemy
{
    protected static readonly log4net.ILog _logger =
        log4net.LogManager.GetLogger(typeof(Enemy));
    
    public abstract Texture texture { get; }
    public abstract Vector2Di position { get; }
    public abstract UInt16 scale { get; }

    private readonly CachedSound _soundHit = new ("assets/sounds/enemyHit.wav");
    private readonly CachedSound _soundHitProt = new ("assets/sounds/enemyHitProt.wav");

    public ushort health { get; set; }
    public bool hasProtection;

    //private ushort deathCause;

    private void checkCollision(Player plr, Bullet bullet)
    {
        if (
            plr.position.X < bullet.position.X + bullet.scale &&
            plr.position.X + plr.scale > bullet.position.X &&
            plr.position.Y < bullet.position.Y + bullet.scale &&
            plr.position.Y + plr.scale > bullet.position.Y
        )
        {
            GameScene.KillPlayer();
            bullet.isAlive = false;
        }
    }
    
    private void checkCollision(Player plr, Enemy enemy)
    {
        if (
            plr.position.X < enemy.position.X + enemy.scale &&
            plr.position.X + plr.scale > enemy.position.X &&
            plr.position.Y < enemy.position.Y + enemy.scale &&
            plr.position.Y + plr.scale > enemy.position.Y
        )
        {
            GameScene.KillPlayer();
            enemy.health--;
        }
    }

    private void checkCollision(Bullet bullet, Enemy enemy)
    {
        if (
            bullet.position.X < enemy.position.X + enemy.scale &&
            bullet.position.X + bullet.scale > enemy.position.X &&
            bullet.position.Y < enemy.position.Y + enemy.scale &&
            bullet.position.Y + bullet.scale > enemy.position.Y
        ) 
        {
            bullet.isAlive = false;
            enemy.health--;
            AudioEngine.Instance.PlaySound(!hasProtection ? _soundHit : _soundHitProt);
        }
    }

    public virtual void Update(Player plr, Enemy enemy)
    {
        checkCollision(plr, enemy);

        foreach (var bullet in Player.Bullets)
        {
            checkCollision(bullet, enemy);
        }

        foreach (var bullet in EnemiesHandler.EnemyBullets.ToList())
        {
            checkCollision(plr, bullet);
        }
    }

    public void Cleanup()
    {
        texture.Drop();
        position.Dispose();
    }
}