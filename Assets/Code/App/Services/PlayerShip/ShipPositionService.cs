using System;
using UnityEngine;

namespace Code.App.Services.PlayerShip
{
    public interface IShipPositionService
    {
        void Setup(Vector3 startPosition, int screenWidth, int screenHeight);
        event Action<Vector2> OnChangeCurrentPosition;
        event Action<Vector3> OnChangeRotationAngle;
        event Action<float> OnChangeInstantaneousSpeed;
        ShipDataModel GetDataModel();
        void ChangeSpeed(float newSpeed);
        void ChangeAngle(Vector3 newAngle);
        void UpdatePosition(float deltaTime);
        void CleanUp();
    }

    [Serializable]
    public class ShipDataModel
    {
        public Vector3 ShipPosition;
        public Vector3 RotationAngle;
        public float Speed;
        public int ScreenWidth;
        public int ScreenHeight;

        public ShipDataModel(Vector3 shipPosition, Vector3 rotationAngle, float speed, int screenWidth, int screenHeight)
        {
            ShipPosition = shipPosition;
            RotationAngle = rotationAngle;
            Speed = speed;
            ScreenWidth = screenWidth;
            ScreenHeight = screenHeight;
        }
    }

    public class ShipPositionService : IShipPositionService
    {
        public event Action<Vector2> OnChangeCurrentPosition;
        public event Action<Vector3> OnChangeRotationAngle;
        public event Action<float> OnChangeInstantaneousSpeed;

        private ShipDataModel _shipData;
        private Vector3 _startPosition;

        public void Setup(Vector3 startPosition, int screenWidth, int screenHeight)
        {
            _startPosition = startPosition;
            _shipData = new ShipDataModel(
                shipPosition: startPosition,
                rotationAngle: Vector3.zero,
                speed: 0f,
                screenWidth: screenWidth,
                screenHeight: screenHeight
            );

            OnChangeCurrentPosition?.Invoke(GetNormalizedPosition());
            OnChangeRotationAngle?.Invoke(_shipData.RotationAngle);
            OnChangeInstantaneousSpeed?.Invoke(_shipData.Speed);
        }

        public void ChangeSpeed(float newSpeed)
        {
            _shipData.Speed = newSpeed;
            OnChangeInstantaneousSpeed?.Invoke(_shipData.Speed);
        }

        public void ChangeAngle(Vector3 newAngle)
        {
            float normalizedZ = NormalizeAngle(newAngle.z);
            _shipData.RotationAngle = new Vector3(0f, 0f, normalizedZ);
            OnChangeRotationAngle?.Invoke(_shipData.RotationAngle);
            OnChangeCurrentPosition?.Invoke(GetNormalizedPosition());
        }

        public void UpdatePosition(float deltaTime)
        {
            float angleRad = _shipData.RotationAngle.z * Mathf.Deg2Rad;
            Vector2 direction = new Vector2(-Mathf.Sin(angleRad), Mathf.Cos(angleRad));
            _shipData.ShipPosition += (Vector3)(direction * _shipData.Speed * deltaTime);
            WrapAroundScreen();
            OnChangeCurrentPosition?.Invoke(GetNormalizedPosition());
        }

        public ShipDataModel GetDataModel()
        {
            return _shipData;
        }

        public void CleanUp()
        {
            _shipData.ShipPosition = _startPosition;
            _shipData.RotationAngle = Vector3.zero;
            _shipData.Speed = 0f;
            OnChangeCurrentPosition?.Invoke(GetNormalizedPosition());
            OnChangeRotationAngle?.Invoke(_shipData.RotationAngle);
            OnChangeInstantaneousSpeed?.Invoke(_shipData.Speed);
        }

        private Vector2 GetNormalizedPosition()
        {
            float normalizedX = _shipData.ShipPosition.x / _shipData.ScreenWidth;
            float normalizedY = _shipData.ShipPosition.y / _shipData.ScreenHeight;
            return new Vector2(normalizedX, normalizedY);
        }

        private float NormalizeAngle(float angle)
        {
            while (angle > 180f) angle -= 360f;
            while (angle <= -180f) angle += 360f;
            return angle;
        }

        private void WrapAroundScreen()
        {
            float x = _shipData.ShipPosition.x;
            float y = _shipData.ShipPosition.y;

            if (x > _shipData.ScreenWidth)
                x -= _shipData.ScreenWidth;
            else if (x < 0)
                x += _shipData.ScreenWidth;

            if (y > _shipData.ScreenHeight)
                y -= _shipData.ScreenHeight;
            else if (y < 0)
                y += _shipData.ScreenHeight;

            _shipData.ShipPosition = new Vector3(x, y, _shipData.ShipPosition.z);
        }
    }
}