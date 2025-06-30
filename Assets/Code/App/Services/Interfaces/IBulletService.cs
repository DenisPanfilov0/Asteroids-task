using System;
using Code.App.Models;
using Code.App.Services.Models;

namespace Code.App.Services.Interfaces
{
    public interface IBulletService
    {
        void RemoveBullet(int id);
        void RemoveLaser(int id);
        event Action<int, BulletData> OnSpawnBullet;
        event Action<int, LaserData> OnSpawnLaser;
        int GetLaserCharges();
        float GetLaserChargeProgress();
        void SetPlayerShip(IPlayerShipModel playerShipModel);
    }
}