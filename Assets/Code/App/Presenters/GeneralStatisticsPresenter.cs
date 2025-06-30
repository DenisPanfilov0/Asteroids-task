using System;
using Code.App.Models;
using Code.App.View;
using R3;
using UnityEngine;
using Zenject;

namespace Code.App.Presenters
{
    public class GeneralStatisticsPresenter : IInitializable, IDisposable
    {
        private readonly GeneralStatisticsView _view;
        private readonly IPlayerShipModel _model;
        private readonly CompositeDisposable _disposables = new();

        public GeneralStatisticsPresenter(
            GeneralStatisticsView view,
            IPlayerShipModel model)
        {
            _view = view;
            _model = model;
            
            _model.Position.Subscribe(position => _view.SetPosition(position)).AddTo(_disposables);
            _model.Rotation.Subscribe(rotation => _view.SetRotationAngle(Mathf.Abs(rotation))).AddTo(_disposables);
            _model.Speed.Subscribe(speed => _view.SetSpeed(speed)).AddTo(_disposables);
            _model.LaserCharge.Subscribe(laser => _view.SetLaserInfo(laser.charges, laser.progress)).AddTo(_disposables);
        }

        public void Initialize()
        {
            _view.SetPosition(_model.Position.Value);
            _view.SetRotationAngle(Mathf.Abs(_model.Rotation.Value));
            _view.SetSpeed(_model.Speed.Value);
            _view.SetLaserInfo(_model.LaserCharge.Value.charges, _model.LaserCharge.Value.progress);
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}