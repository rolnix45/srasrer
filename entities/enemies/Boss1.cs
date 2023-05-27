using Irrlicht;
using Irrlicht.Core;
using Irrlicht.Video;
using nook.main;
#pragma warning disable CS4014

namespace nook.entities.enemies;

sealed class Boss1 : Enemy
{
    public override Texture texture { get; }
    public override Vector2Di position { get; }
    public override ushort scale { get; }

    private const ushort speed = 350;
    private const ushort firerate = 1000;
    private float nextTimeToFire;

    private readonly IrrlichtDevice _device;
    private readonly Player _player;

    private readonly Texture bulletTexure;

    public Boss1(IrrlichtDevice device, Player player)
    {
        _player = player;
        _device = device;
        texture = device.VideoDriver.GetTexture("assets/textures/boss1.png");
        bulletTexure = device.VideoDriver.GetTexture("assets/textures/boss1Bullet.png");
        scale = 256;
        position = new Vector2Di();
    }

    public void Spawn()
    {
        health = 16;
        position.Set(Game.winWidth + 25, (Game.winHeight / 2) - scale / 2);
        EnemiesHandler.Enemies.Add(this);
    }

    private void Attack()
    {
        if (_device.Timer.Time < nextTimeToFire) return;
        
        nextTimeToFire = _device.Timer.Time + firerate;
        
        Random rnd = new Random();
        var attackType = rnd.Next(0, 3);
        EnemyBullet enBullet;
        switch (attackType)
        {
            case 0: // TRIPLE SHOT
                for (var i = -20; i < 40; i += 10)
                {
                    enBullet = new EnemyBullet(bulletTexure, "boss1");
                    enBullet.Create(new Vector2Di(position.X, position.Y + (scale / 2)), _player, i);
                    EnemiesHandler.EnemyBullets.Add(enBullet);
                }
                break;
            case 1: // LOT OF SHIT
                async Task Shoot()
                {
                    for (var i = 0; i < 16; i++)
                    {
                        enBullet = new EnemyBullet(bulletTexure, "boss1");
                        enBullet.Create(new Vector2Di(position.X, position.Y + (scale / 2)), _player);
                        EnemiesHandler.EnemyBullets.Add(enBullet);
                        await Task.Delay(50);
                    }
                }

                Shoot();
                break;
            case 2: // a circle
                for (var i = 0; i < 360; i += 20)
                {
                    enBullet = new EnemyBullet(bulletTexure, "boss1");
                    enBullet.Create(new Vector2Di(position.X, (position.Y + (scale / 2))), _player, i);
                    EnemiesHandler.EnemyBullets.Add(enBullet);
                }
                break;
        }

    }
    
    public override void Update(Player plr, Enemy enemy)
    {
        base.Update(plr, enemy);
        if (health == 0)
        {
            Player.score += 100;
            EnemiesHandler.Enemies.Remove(this);
            return;
        }
        
        if (position.X > Game.winWidth / 1.5)
        {
            health = 16;
            position.X -= (int)(speed * Game.frameDeltaTime);
            return;
        }
        
        Attack();
    }
}