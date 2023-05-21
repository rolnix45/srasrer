using Irrlicht.Core;
using Irrlicht.Video;
using nook.main;

namespace nook.entities.enemies;

abstract class Enemy
{
    private static readonly log4net.ILog _logger =
        log4net.LogManager.GetLogger(typeof(Enemy));
    
    public Texture texture { get; }
    public abstract Vector2Di position { get; }
    public abstract UInt16 scale { get; protected init; }

    public bool isAlive { get; set; }

    protected Enemy(Texture texture)
    {
        this.texture = texture;
        isAlive = true;
    }

    private void KillPlayer()
    {
        Game.isPlayerAlive = false;
        _logger.Debug("player dead");
    }

    private void checkCollision(ref Player plr, Enemy enemy)
    {
        if (
            plr.position.X < enemy.position.X + enemy.scale &&
            plr.position.X + plr.scale > enemy.position.X &&
            plr.position.Y < enemy.position.Y + enemy.scale &&
            plr.position.Y + plr.scale > enemy.position.Y
        ) 
        {
            KillPlayer();
            enemy.isAlive = false;
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
            enemy.isAlive = false;
        }
    }

    public virtual void Update(ref Player plr, Enemy enemy)
    {
        checkCollision(ref plr, enemy);

        foreach (var bullet in Player.Bullets)
        {
            checkCollision(bullet, enemy);
        }
    }
}