using Irrlicht.Core;
using Irrlicht.Video;
using nook.main;
using Timer = System.Timers.Timer;

namespace nook.graph;

class Particle
{
    private Texture texture;
    private Vector2Di position;
    private int lifeTime;
    private double angle;
    private int speed;

    public Particle(Texture texture, Vector2Di position, int lifeTime, double angle, int speed)
    {
        this.texture = texture;
        this.position = position;
        this.lifeTime = lifeTime;
        this.angle = angle;
        this.speed = speed;
    }

    public void Update()
    {
        Timer lifeTimer = new Timer(lifeTime);
        lifeTimer.AutoReset = false;
        lifeTimer.Elapsed += (_, _) =>
        {
            ParticleSystem.Particles.Remove(this);
        };

        position.Set(position + (int)(speed * Game.frameDeltaTime));
    }
}