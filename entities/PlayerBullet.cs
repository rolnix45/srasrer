using Irrlicht.Core;
using Irrlicht.Video;

namespace nook.entities;

sealed class PlayerBullet : Bullet
{
    public override Texture texture { get; protected init; }
    public override Vector2Di position { get; protected init; }
    
    public PlayerBullet(ref VideoDriver driver)
    {
        this.texture = driver.GetTexture("assets/textures/bullet.png");
        this.position = new Vector2Di();
        this.scale = 32;
        this.isAlive = true;
    }
    
    public void Create(Vector2Di pos)
    {
        position.Set(pos);
        deltaX = 1750;
        deltaY = 0;
    } 
}