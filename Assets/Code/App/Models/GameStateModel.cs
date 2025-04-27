using System;
using Code.App.Models.Interfaces;
using Code.App.Services;
using Code.App.Services.Interfaces;
using Code.Infrastructure.WindowsService;
using Code.Infrastructure.WindowsService.MVP;
using UnityEngine;

namespace Code.App.Models
{
    public class GameStateModel : IGameStateModel
    {
        public event Action<int> OnScoreChanged;
        public event Action<bool> OnPauseChanged;
        public event Action<Vector2> OnPositionChanged;
        public event Action<float> OnRotationChanged;
        public event Action<float> OnSpeedChanged;
        public event Action<int, float> OnLaserChargeChanged;

        private readonly ICollisionService _collisionService;
        private readonly WindowCreator _windowCreator;
        private readonly IBulletService _bulletService;
        private int _score;
        private bool _onPause;
        private Vector2 _position;
        private float _rotation;
        private float _speed;
        private int _laserCharges;
        private float _laserChargeProgress;

        public GameStateModel(
            ICollisionService collisionService,
            WindowCreator windowCreator,
            IBulletService bulletService)
        {
            _collisionService = collisionService;
            _windowCreator = windowCreator;
            _bulletService = bulletService;

            _onPause = false;
            _position = Vector2.zero;
            _rotation = 0f;
            _speed = 0f;
            _laserCharges = _bulletService.GetLaserCharges();
            _laserChargeProgress = _bulletService.GetLaserChargeProgress();

            SubscribeUpdates();
        }

        private void SubscribeUpdates()
        {
            _collisionService.OnPlayerHit += HandlePlayerCollision;
            _collisionService.OnEnemyHitByBullet += HandleEnemyHit;
            _collisionService.OnEnemyHitByLaser += HandleEnemyHit;
        }

        public int GetScore() => _score;

        public Vector2 GetPosition() => _position;

        public float GetRotation() => _rotation;

        public float GetSpeed() => _speed;

        public int GetLaserCharges() => _laserCharges;

        public float GetLaserChargeProgress() => _laserChargeProgress;

        public void SetPosition(Vector2 position)
        {
            _position = position;
            OnPositionChanged?.Invoke(_position);
        }

        public void SetRotation(float rotation)
        {
            _rotation = rotation;
            OnRotationChanged?.Invoke(_rotation);
        }

        public void SetSpeed(float speed)
        {
            _speed = speed;
            OnSpeedChanged?.Invoke(_speed);
        }

        public void SetLaserCharge(int charges, float progress)
        {
            _laserCharges = charges;
            _laserChargeProgress = progress;
            OnLaserChargeChanged?.Invoke(_laserCharges, _laserChargeProgress);
        }

        public void CleanUp()
        {
            _score = 0;
            _onPause = false;
            _position = Vector2.zero;
            _rotation = 0f;
            _speed = 0f;
            _laserCharges = 0;
            _laserChargeProgress = 0f;

            OnScoreChanged?.Invoke(_score);
            OnPauseChanged?.Invoke(_onPause);
            OnPositionChanged?.Invoke(_position);
            OnRotationChanged?.Invoke(_rotation);
            OnSpeedChanged?.Invoke(_speed);
            OnLaserChargeChanged?.Invoke(_laserCharges, _laserChargeProgress);

            _collisionService.OnPlayerHit -= HandlePlayerCollision;
            _collisionService.OnEnemyHitByBullet -= HandleEnemyHit;
            _collisionService.OnEnemyHitByLaser -= HandleEnemyHit;
        }

        private void HandlePlayerCollision(int value)
        {
            SetPauseState(true);
            _windowCreator.WindowOpen(WindowId.GameLoseWindow);
        }

        private void HandleEnemyHit(int id)
        {
            if (_onPause)
                return;

            _score++;
            OnScoreChanged?.Invoke(_score);
        }

        private void SetPauseState(bool isPaused)
        {
            _onPause = isPaused;
            OnPauseChanged?.Invoke(_onPause);
        }
    }
}