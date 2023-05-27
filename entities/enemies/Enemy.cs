using Irrlicht.Core;
using Irrlicht.Video;
using nook.main;

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

    public ushort health { get; set; }

    private ushort deathCause;

    private void checkCollision(Player plr, Bullet bullet)
    {
        if (
            plr.position.X < bullet.position.X + bullet.scale &&
            plr.position.X + plr.scale > bullet.position.X &&
            plr.position.Y < bullet.position.Y + bullet.scale &&
            plr.position.Y + plr.scale > bullet.position.Y
        )
        {
            Game.KillPlayer();
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
            Game.KillPlayer();
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
            if (enemy.health == 0)
            {
                deathCause = 0;
            }
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
}