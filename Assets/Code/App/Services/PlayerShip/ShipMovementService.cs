using System;
using UnityEngine;

namespace Code.App.Services.PlayerShip
{
    public interface IShipMovementService
    {
        void UpdateInput();
        event Action<Vector3> OnChangeRotationAngle;
        event Action<float> OnChangeRotationSpeed;
        void CleanUp();
    }

    public class ShipMovementService : IShipMovementService
    {
        public event Action<Vector3> OnChangeRotationAngle;
        public event Action<float> OnChangeRotationSpeed;

        private readonly IShipPositionService _shipPositionService;
        private float _rotationSpeed;
        private Vector3 _rotationAngle;
        private float _targetSpeed;
        private const float RotationSpeedIncrement = 75f;
        private const float SpeedAcceleration = 50f;
        private const float SpeedDeceleration = 40f;
        private const float MaxSpeed = 100f;

        public ShipMovementService(IShipPositionService shipPositionService)
        {
            _shipPositionService = shipPositionService;
            _rotationSpeed = 0f;
            _rotationAngle = Vector3.zero;
            _targetSpeed = 0f;
        }

        public void UpdateInput()
        {
            float deltaTime = Time.deltaTime;
            bool hasInput = false;

            if (Input.GetKey(KeyCode.LeftArrow))
            {
                _rotationSpeed = RotationSpeedIncrement;
                _rotationAngle.z += _rotationSpeed * deltaTime;
                hasInput = true;
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                _rotationSpeed = -RotationSpeedIncrement;
                _rotationAngle.z += _rotationSpeed * deltaTime;
                hasInput = true;
            }
            else
            {
                _rotationSpeed = 0f;
            }

            if (hasInput)
            {
                _rotationAngle.z = NormalizeAngle(_rotationAngle.z);
                _shipPositionService.ChangeAngle(_rotationAngle);
                OnChangeRotationAngle?.Invoke(_rotationAngle);
                OnChangeRotationSpeed?.Invoke(_rotationSpeed);
            }

            if (Input.GetKey(KeyCode.UpArrow))
            {
                _targetSpeed = MaxSpeed;
            }
            else
            {
                _targetSpeed = 0f;
            }

            float currentSpeed = _shipPositionService.GetDataModel().Speed;
            float acceleration = _targetSpeed > currentSpeed ? SpeedAcceleration : SpeedDeceleration;
            currentSpeed = Mathf.MoveTowards(currentSpeed, _targetSpeed, acceleration * deltaTime);
            _shipPositionService.ChangeSpeed(currentSpeed);
        }

        public void CleanUp()
        {
            _rotationSpeed = 0f;
            _rotationAngle = Vector3.zero;
            _targetSpeed = 0f;
        }

        private float NormalizeAngle(float angle)
        {
            while (angle > 180f) angle -= 360f;
            while (angle <= -180f) angle += 360f;
            return angle;
        }
    }
}