using Code.App.Behaviours;
using Code.App.Behaviours.Player;
using Code.App.Models.Interfaces;
using Code.App.Services.Interfaces;
using Code.App.View;
using UnityEngine;
using Zenject;

namespace Code.App.Presenters
{
    public class GeneralStatisticsPresenter : IInitializable
    {
        private readonly IGameStateModel _gameStateModel;
        private readonly GeneralStatisticsView _view;
        private readonly PlayerShip _playerShip;
        private readonly IBulletService _bulletService;

        public GeneralStatisticsPresenter(
            IGameStateModel gameStateModel,
            GeneralStatisticsView view,
            PlayerShip playerShip,
            IBulletService bulletService)
        {
            _gameStateModel = gameStateModel;
            _view = view;
            _playerShip = playerShip;
            _bulletService = bulletService;
        }

        public void Initialize()
        {
            _view.SetPosition(_gameStateModel.GetPosition());
            _view.SetRotationAngle(Mathf.Abs(_gameStateModel.GetRotation()));
            _view.SetSpeed(_gameStateModel.GetSpeed());
            _view.SetLaserInfo(_gameStateModel.GetLaserCharges(), _gameStateModel.GetLaserChargeProgress());

            _playerShip.OnPositionChanged += UpdateModelPosition;
            _playerShip.OnRotationChanged += UpdateModelRotation;
            _playerShip.OnSpeedChanged += UpdateModelSpeed;
            _bulletService.OnLaserChargeChanged += UpdateModelLaserCharge;

            _gameStateModel.OnPositionChanged += UpdateViewPosition;
            _gameStateModel.OnRotationChanged += UpdateViewRotationAngle;
            _gameStateModel.OnSpeedChanged += UpdateViewSpeed;
            _gameStateModel.OnLaserChargeChanged += UpdateViewLaserInfo;
        }

        public void Cleanup()
        {
            _playerShip.OnPositionChanged -= UpdateModelPosition;
            _playerShip.OnRotationChanged -= UpdateModelRotation;
            _playerShip.OnSpeedChanged -= UpdateModelSpeed;
            _bulletService.OnLaserChargeChanged -= UpdateModelLaserCharge;

            _gameStateModel.OnPositionChanged -= UpdateViewPosition;
            _gameStateModel.OnRotationChanged -= UpdateViewRotationAngle;
            _gameStateModel.OnSpeedChanged -= UpdateViewSpeed;
            _gameStateModel.OnLaserChargeChanged -= UpdateViewLaserInfo;
        }

        private void UpdateModelPosition(Vector2 position)
        {
            _gameStateModel.SetPosition(position);
        }

        private void UpdateModelRotation(float rotation)
        {
            _gameStateModel.SetRotation(rotation);
        }

        private void UpdateModelSpeed(float speed)
        {
            _gameStateModel.SetSpeed(speed);
        }

        private void UpdateModelLaserCharge(int charges, float progress)
        {
            _gameStateModel.SetLaserCharge(charges, progress);
        }

        private void UpdateViewPosition(Vector2 position)
        {
            _view.SetPosition(position);
        }

        private void UpdateViewRotationAngle(float rotation)
        {
            _view.SetRotationAngle(Mathf.Abs(rotation));
        }

        private void UpdateViewSpeed(float speed)
        {
            _view.SetSpeed(speed);
        }

        private void UpdateViewLaserInfo(int charges, float progress)
        {
            _view.SetLaserInfo(charges, progress);
        }
    }
}