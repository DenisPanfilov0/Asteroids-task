using System;
using Code.App.Services.Models;
using Zenject;

namespace Code.App.Services.Interfaces
{
    public interface IBulletService : ITickable
    {
        void RemoveBullet(int id);
        void RemoveLaser(int id);
        event Action<int, BulletData> OnSpawnBullet;
        event Action<int, LaserData> OnSpawnLaser;
        event Action<int, float> OnLaserChargeChanged;
        int GetLaserCharges();
        float GetLaserChargeProgress();
        void CleanUp();
    }
}