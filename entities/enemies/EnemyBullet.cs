using Irrlicht.Core;
using Irrlicht.Video;
using nook.main;

namespace nook.entities.enemies;

sealed class EnemyBullet : Bullet
{
    public override Texture texture { get; protected init; }
    public override Vector2Di position { get; protected init; }

    public readonly string tag;
    private const double bulletVelocity = 800;
    
    public EnemyBullet(Texture texture, string tag)
    {
        this.texture = texture;
        this.position = new Vector2Di();
        this.scale = 16;
        this.isAlive = true;
        this.tag = tag;
    }

    public void Create(Vector2Di pos, int speed)
    {
        position.Set(pos);
        deltaX = speed;
        deltaY = 0;
    }
    
    public void Create(Vector2Di pos, Player player)
    {
        var enemyPos = pos;
        var playerPos = player.position;

        position.Set(pos.X, pos.Y);
        var angle = CalculateAngle(
            playerPos.X + player.scale / 2,
            playerPos.Y + player.scale / 2,
            enemyPos.X,
            enemyPos.Y
        );

        deltaX = ((180 / Math.PI) * Math.Cos(angle)) * bulletVelocity * Game.frameDeltaTime;
        deltaY = ((180 / Math.PI) * Math.Sin(angle)) * bulletVelocity * Game.frameDeltaTime;
    }


    public void Create(Vector2Di pos, Player player, double ang)
    {
        var enemyPos = pos;
        var playerPos = player.position;

        position.Set(pos.X, pos.Y);
        var angle = CalculateAngle(
            playerPos.X + player.scale / 2,
            playerPos.Y + player.scale / 2,
            enemyPos.X,
            enemyPos.Y
        );

        var ang1 = (ang * Math.PI) / 180;
        
        deltaX = ((180 / Math.PI) * Math.Cos(angle + ang1))
                 * (bulletVelocity * Game.frameDeltaTime);

        deltaY = ((180 / Math.PI) * Math.Sin(angle + ang1))
                 * (bulletVelocity * Game.frameDeltaTime);
    }
}