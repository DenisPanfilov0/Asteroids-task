using System;
using UnityEngine;

namespace Code.App.Models.Interfaces
{
    public interface IGameStateModel
    {
        event Action<int> OnScoreChanged;
        event Action<bool> OnPauseChanged;
        event Action<Vector2> OnPositionChanged;
        event Action<float> OnRotationChanged;
        event Action<float> OnSpeedChanged;
        event Action<int, float> OnLaserChargeChanged;

        int GetScore();
        Vector2 GetPosition();
        float GetRotation();
        float GetSpeed();
        int GetLaserCharges();
        float GetLaserChargeProgress();

        void SetPosition(Vector2 position);
        void SetRotation(float rotation);
        void SetSpeed(float speed);
        void SetLaserCharge(int charges, float progress);
        void CleanUp();
    }
}