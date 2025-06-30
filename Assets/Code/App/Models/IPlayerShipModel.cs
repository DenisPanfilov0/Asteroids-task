using R3;
using UnityEngine;

namespace Code.App.Models
{
    public interface IPlayerShipModel
    {
        ReactiveProperty<Vector2> Position { get; }
        ReactiveProperty<float> Rotation { get; }
        ReactiveProperty<float> Speed { get; }
        ReactiveProperty<(int charges, float progress)> LaserCharge { get; }
        ReactiveProperty<int> Score { get; }
        ReactiveProperty<bool> IsPaused { get; }

        Vector2 GetPosition();
        float GetRotation();
        void SetPosition(Vector2 position);
        void SetRotation(float rotation);
        void SetSpeed(float speed);
        void SetLaserCharge(int charges, float progress);
        void HandlePlayerCollision();
        void HandleEnemyHit();
    }
}