using TMPro;
using UnityEngine;

namespace Code.App.View
{
    public class GeneralStatisticsView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _currentPosition;
        [SerializeField] private TextMeshProUGUI _rotationAngle;
        [SerializeField] private TextMeshProUGUI _instantaneousSpeed;
        [SerializeField] private TextMeshProUGUI _laserCharges;
        [SerializeField] private TextMeshProUGUI _laserChargeProgress;

        public void SetPosition(Vector2 position)
        {
            _currentPosition.text = $"x:{position.x:F1} y:{position.y:F1}";
        }

        public void SetRotationAngle(float angle)
        {
            _rotationAngle.text = $"{angle:F1}";
        }

        public void SetSpeed(float speed)
        {
            _instantaneousSpeed.text = $"speed: {speed:F1}";
        }

        public void SetLaserInfo(int charges, float progress)
        {
            _laserCharges.text = $"{charges}/3";
            _laserChargeProgress.text = charges >= 3 ? "MAX" : $"{Mathf.RoundToInt(progress)}%";
        }
    }
}