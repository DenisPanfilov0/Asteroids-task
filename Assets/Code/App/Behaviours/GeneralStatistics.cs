using Code.App.Services;
using Code.App.Services.PlayerShip;
using TMPro;
using UnityEngine;
using Zenject;

namespace Code.App.Behaviours
{
    public class GeneralStatistics : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _currentPosition;
        [SerializeField] private TextMeshProUGUI _rotationAngle;
        [SerializeField] private TextMeshProUGUI _instantaneousSpeed;
        [SerializeField] private TextMeshProUGUI _laserCharges;
        [SerializeField] private TextMeshProUGUI _laserChargeProgress;

        private IShipPositionService _shipPositionService;
        private IBulletService _bulletService;

        [Inject]
        public void Construct(IShipPositionService shipPositionService, IBulletService bulletService)
        {
            _shipPositionService = shipPositionService;
            _bulletService = bulletService;
        }

        private void Start()
        {
            var shipDataModel = _shipPositionService.GetDataModel();
            _currentPosition.text = $"x:{shipDataModel.ShipPosition.x} y:{shipDataModel.ShipPosition.y}";
            _rotationAngle.text = $"{shipDataModel.RotationAngle.z}";
            _instantaneousSpeed.text = $"speed: {shipDataModel.Speed}";
            UpdateLaserUI(_bulletService.GetLaserCharges(), _bulletService.GetLaserChargeProgress());

            _shipPositionService.OnChangeCurrentPosition += UpdateUIPosition;
            _shipPositionService.OnChangeRotationAngle += UpdateUIRotationAngle;
            _shipPositionService.OnChangeInstantaneousSpeed += UpdateUIInstantaneousSpeed;
            _bulletService.OnLaserChargeChanged += UpdateLaserUI;
        }

        private void OnDestroy()
        {
            _shipPositionService.OnChangeCurrentPosition -= UpdateUIPosition;
            _shipPositionService.OnChangeRotationAngle -= UpdateUIRotationAngle;
            _shipPositionService.OnChangeInstantaneousSpeed -= UpdateUIInstantaneousSpeed;
            _bulletService.OnLaserChargeChanged -= UpdateLaserUI;
        }

        private void UpdateUIPosition(Vector2 value)
        {
            _currentPosition.text = $"x:{value.x} y:{value.y}";
        }

        private void UpdateUIRotationAngle(Vector3 value)
        {
            float angle = value.z;
            
            if (angle < 0)
            {
                angle *= -1;
            }
            _rotationAngle.text = $"{angle}";
        }

        private void UpdateUIInstantaneousSpeed(float value)
        {
            _instantaneousSpeed.text = $"speed: {value}";
        }

        private void UpdateLaserUI(int charges, float progress)
        {
            _laserCharges.text = $"{charges}/3";
            _laserChargeProgress.text = charges >= 3 ? "MAX" : $"{Mathf.RoundToInt(progress)}%";
        }
    }
}