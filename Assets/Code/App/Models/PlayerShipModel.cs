using System;
using Code.App.Services.Interfaces;
using Code.Infrastructure.WindowsService;
using Code.Infrastructure.WindowsService.MVP;
using R3;
using UnityEngine;

namespace Code.App.Models
{
    public class PlayerShipModel : IPlayerShipModel, IDisposable
    {
        public ReactiveProperty<Vector2> Position { get; set; }
        public ReactiveProperty<float> Rotation { get; set; }
        public ReactiveProperty<float> Speed { get; set; }
        public ReactiveProperty<(int charges, float progress)> LaserCharge { get; set; }
        public ReactiveProperty<int> Score { get; set; }
        public ReactiveProperty<bool> IsPaused { get; set; }

        private readonly WindowCreator _windowCreator;

        public PlayerShipModel(WindowCreator windowCreator, IBulletService bulletService)
        {
            _windowCreator = windowCreator;

            Setup(bulletService);
        }

        public Vector2 GetPosition() => 
            Position.Value;

        public float GetRotation() => 
            Rotation.Value;

        public void SetPosition(Vector2 position) => 
            Position.Value = position;

        public void SetRotation(float rotation) => 
            Rotation.Value = rotation;

        public void SetSpeed(float speed) => 
            Speed.Value = speed;

        public void SetLaserCharge(int charges, float progress) => 
            LaserCharge.Value = (charges, progress);

        public void HandlePlayerCollision()
        {
            if (IsPaused.Value)
                return;
            
            IsPaused.Value = true;
            _windowCreator.WindowOpen(WindowId.GameLoseWindow);
        }

        public void HandleEnemyHit()
        {
            if (IsPaused.Value) 
                return;
            
            Score.Value++;
        }

        public void Dispose()
        {
            Position.Value = Vector2.zero;
            Rotation.Value = 0f;
            Speed.Value = 0f;
            Score.Value = 0;
            IsPaused.Value = false;
            LaserCharge.Value = (0, 0f);
        }

        private void Setup(IBulletService bulletService)
        {
            Position = new ReactiveProperty<Vector2>(Vector2.zero);
            Rotation = new ReactiveProperty<float>(0f);
            Speed = new ReactiveProperty<float>(0f);
            LaserCharge = new ReactiveProperty<(int, float)>((bulletService.GetLaserCharges(), bulletService.GetLaserChargeProgress()));
            Score = new ReactiveProperty<int>(0);
            IsPaused = new ReactiveProperty<bool>(false);
        }
    }
}