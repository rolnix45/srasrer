using Irrlicht.Core;
using Irrlicht.Video;
using nook.main;

namespace nook.entities.enemies;

sealed class EnemiesHandler
{
    private static readonly log4net.ILog _logger =
        log4net.LogManager.GetLogger(typeof(EnemiesHandler));
    
    public static readonly List<Enemy> Enemies = new List<Enemy>();
    public static readonly List<EnemyBullet> EnemyBullets = new List<EnemyBullet>();

    private readonly VideoDriver driver;
    
    public EnemiesHandler(VideoDriver driver)
    {
        this.driver = driver;
    }

    public void KillAllEnemiesAndBullets()
    {
        foreach (var enemy in Enemies)
        {
            enemy.health = 0;
        }

        foreach (var bullet in Player.Bullets)
        {
            bullet.isAlive = false;
        }

        foreach (var bullet in EnemyBullets)
        {
            bullet.isAlive = false;
        }
    }
    
    public void Draw()
    {
        foreach (var bullet in EnemyBullets)
        {
            driver.Draw2DImage(
                bullet.texture,
                bullet.position,
                new Recti(0, 0, bullet.scale, bullet.scale),
                null,
                Color.SolidWhite,
                true
            );
            
            if (!Game.showHitboxes) continue;
            
            driver.Draw2DRectangleOutline(
                new Recti(bullet.position, new Dimension2Di(bullet.scale)),
                Color.SolidRed
            );
        }
        
        foreach (var enemy in Enemies)
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
    }
}