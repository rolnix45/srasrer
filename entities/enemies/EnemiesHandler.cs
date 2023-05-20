using Irrlicht;
using Irrlicht.Core;
using Irrlicht.Video;
using nook.io;

namespace nook.entities.enemies;

class EnemiesHandler
{
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
            }
            else
            {
                Enemies.Remove(enemy);
            }
        }
    }
}