using Irrlicht;
using Irrlicht.Core;
using Irrlicht.Video;
using nook.io;
using nook.main;

namespace nook.entities.enemies;

class EnemiesHandler
{
    private static readonly log4net.ILog _logger =
        log4net.LogManager.GetLogger(typeof(EnemiesHandler));
    
    public static readonly List<Enemy> Enemies = new List<Enemy>();

    private readonly VideoDriver driver;
    
    public EnemiesHandler(VideoDriver driver)
    {
        this.driver = driver;
    }

    public void KillAllEnemiesAndBullets()
    {
        foreach (var enemy in EnemiesHandler.Enemies)
        {
            enemy.isAlive = false;
        }

        foreach (var bullet in Player.Bullets)
        {
            bullet.isAlive = false;
        }
    }
    
    public void Draw()
    {
        foreach (var enemy in Enemies)
        {
            if (!Input.keys[(int)KeyCode.Delete])
            {
                driver.Draw2DImage(
                    enemy.texture,
                    enemy.position,
                    new Recti(0, 0, enemy.scale, enemy.scale),
                    null,
                    Color.SolidWhite,
                    true
                );
                
                if (!Game.showHitboxes) continue;
                
                driver.Draw2DRectangleOutline(
                    new Recti(enemy.position, new Dimension2Di(enemy.scale)),
                    Color.SolidRed
                );
            }
            else
            {
                Enemies.Remove(enemy);
            }
        }
    }
}