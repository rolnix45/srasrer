using Irrlicht;
using Irrlicht.Video;
using nook.main;

namespace nook.entities.enemies;

sealed class EnTest : Enemy
{
    private readonly Random rnd;

    private const UInt16 speed = 300;


    private readonly IrrlichtDevice device;
    
    public EnTest(Texture texture, IrrlichtDevice device) : base(texture)
    {
        scale = 32;
        rnd = new Random();
        this.device = device;
    }

    public void Spawn()
    {
        position.X = Game.winWidth + 25;
        position.Y = rnd.Next(scale, Game.winHeight - scale);
        
        EnemiesHandler.Enemies.Add(this);
    }

    public override void Update(ref Player player)
    {
        base.Update(ref player);
        if (position.X < 0 || !isAlive)
        {
            EnemiesHandler.Enemies.Remove(this);
            return;
        }
        
        /*
        if (device.Timer.Time >= timeToSpawn)
        {
            timeToSpawn = device.Timer.Time + spawnRate;
            _logger.Debug("spawned");
            Spawn();
        }
        */
        
        position.X -= (int) (speed * Game.frameDeltaTime);
    }
}