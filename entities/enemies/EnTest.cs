using Irrlicht;
using Irrlicht.Core;
using Irrlicht.Video;
using nook.main;

namespace nook.entities.enemies;

sealed class EnTest : Enemy
{
    private static readonly log4net.ILog _logger =
        log4net.LogManager.GetLogger(typeof(Enemy));

    private const UInt16 speed = 300;
    public override Vector2Di position { get; }
    public override UInt16 scale { get; protected init; }

    private readonly IrrlichtDevice device;

    private EnTest(Texture texture, IrrlichtDevice device) : base(texture)
    {
        scale = 48;
        position = new Vector2Di();
        isAlive = true;
        
        this.device = device;
    }

    public static void SpawnEnTest(IrrlichtDevice device)
    {
        Random rnd = new Random();

        var enTest = new EnTest(
            device.VideoDriver.GetTexture(Game.debugPath + "assets/textures/enemy1.png"),
            device)
        {
            position =
            {
                X = Game.winWidth + 25
            }
        };

        enTest.position.Y = rnd.Next(enTest.scale, Game.winHeight - enTest.scale);
        
        EnemiesHandler.Enemies.Add(enTest);
    }
    
    public override void Update(ref Player player, Enemy enemy)
    {
        base.Update(ref player, enemy);
        if (position.X < 0 || !isAlive)
        {
            EnemiesHandler.Enemies.Remove(this);
            return;
        }
        
        position.X -= (int) (speed * Game.frameDeltaTime);
    }
}