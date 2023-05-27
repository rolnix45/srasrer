using Irrlicht.Core;
using Irrlicht.Video;

namespace nook.entities;

abstract class Bullet
{
    public abstract Texture texture { get; protected init; }
    public abstract Vector2Di position { get; protected init; }
    public UInt16 scale { get; protected init; }
    public bool isAlive { get; set; }

    public double deltaX { get; protected set; }
    public double deltaY { get; protected set; }

    protected double CalculateAngle(int x1, int y1, int x2, int y2)
    {
        var y = y1 - y2;
        var x = x1 - x2;
        return Math.Atan2(y, x);
    }
}