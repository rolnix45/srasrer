using Irrlicht.Core;
using Irrlicht.Video;
using nook.main;

namespace nook.entities;

sealed class Bullet
{
    public Texture texture { get; }
    public Vector2Di position { get; }
    public UInt16 scale { get; }

    public double deltaX { get; private set; }
    public double deltaY { get; private set; }
    
    public bool isAlive { get; set; }
    
    public Bullet(ref VideoDriver driver)
    {
        texture = driver.GetTexture(Game.debugPath + "assets/textures/bullet.png");
        position = new Vector2Di();
        scale = 32;
        isAlive = true;
    }
    
    public void Create(Vector2Di pos)
    {
        position.Set(pos);
        deltaX = 1750;
        deltaY = 0;
    }
}