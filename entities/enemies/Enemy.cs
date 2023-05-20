using Irrlicht.Core;
using Irrlicht.Video;
using nook.main;

namespace nook.entities.enemies;

abstract class Enemy
{
    private static readonly log4net.ILog _logger =
        log4net.LogManager.GetLogger(typeof(Player));
    
    public Texture texture { get; }
    public Vector2Di position { get; }
    public UInt16 scale { get; protected init; }

    public bool isAlive { get; set; }
    
    protected Enemy(Texture texture)
    {
        this.texture = texture;
        position = new Vector2Di();
        isAlive = true;
    }

    private void KillPlayer()
    {
        Game.isPlayerAlive = false;
        _logger.Debug("player dead");
    }

    private void checkCollision(ref Player plr)
    {
        if (
            plr.position.X < position.X + scale &&
            plr.position.X + plr.scale > position.X &&
            plr.position.Y < position.Y + scale &&
            plr.position.Y + plr.scale > position.Y
        ) 
        {
            KillPlayer();
            isAlive = false;
        }
    }

    private void checkCollision(Bullet bullet)
    {
        if (
            bullet.position.X < position.X + scale &&
            bullet.position.X + bullet.scale > position.X &&
            bullet.position.Y < position.Y + scale &&
            bullet.position.Y + bullet.scale > position.Y
        ) 
        {
            bullet.isAlive = false;
            isAlive = false;
        }
    }

    public virtual void Update(ref Player plr)
    {
        checkCollision(ref plr);
        foreach (var bullet in Player.Bullets)
        {
            checkCollision(bullet);
        }
    }
}