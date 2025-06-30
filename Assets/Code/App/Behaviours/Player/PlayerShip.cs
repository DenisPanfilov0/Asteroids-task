using Code.App.Extensions;
using Code.App.Models;
using Code.App.Services.Interfaces;
using Code.App.Services.Models;
using R3;
using UnityEngine;
using Zenject;

namespace Code.App.Behaviours.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerShip : MonoBehaviour
    {
        private const float MAX_SPEED = 100f;
        private const float ACCELERATION = 50f;
        private const float DECELERATION = 40f;
        private const float ROTATION_SPEED = 75f;
        
        [SerializeField] private GameObject _bulletPrefab;
        [SerializeField] private GameObject _laserPrefab;
        [SerializeField] private Transform _bulletSpawnPoint;

        private Rigidbody2D _rb;
        private IBulletService _bulletService;
        private IPlayerShipModel _model;
        private Camera _mainCamera;
        private Bounds _worldBounds;
        private Vector2 _startPosition;
        private CompositeDisposable _disposable = new();
        private bool _isPause;

        [Inject]
        public void Construct(
            IBulletService bulletService,
            IPlayerShipModel model,
            Camera mainCamera)
        {
            _bulletService = bulletService;
            _model = model;
            _mainCamera = mainCamera;
        }

        private void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
            _worldBounds = CameraExtensions.GetOrthographicBounds(_mainCamera);
            _startPosition = _worldBounds.center;
            transform.position = _startPosition;
            _rb.position = _startPosition;

            _bulletService.SetPlayerShip(_model);
            _bulletService.OnSpawnBullet += HandleSpawnBullet;
            _bulletService.OnSpawnLaser += HandleSpawnLaser;

            _model.SetPosition(_rb.position);
            _model.SetRotation(_rb.rotation);
            _model.SetSpeed(0f);
            
            _model.IsPaused.Subscribe(GamePause).AddTo(_disposable);
        }

        private void Update()
        {
            if (_isPause) return;
            HandleInput();
            WrapAroundScreen();
            NotifyModel();
        }

        private void GamePause(bool state)
        {
            _isPause = state;
        }

        private void OnDestroy()
        {
            _bulletService.OnSpawnBullet -= HandleSpawnBullet;
            _bulletService.OnSpawnLaser -= HandleSpawnLaser;

            _isPause = false;
            
            _disposable.Dispose();
        }

        private void HandleInput()
        {
            var deltaTime = Time.deltaTime;
            float rotationInput = 0f;
            
            if (Input.GetKey(KeyCode.LeftArrow))
                rotationInput = ROTATION_SPEED;
            else if (Input.GetKey(KeyCode.RightArrow))
                rotationInput = -ROTATION_SPEED;

            _rb.angularVelocity = rotationInput;

            float targetSpeed = Input.GetKey(KeyCode.UpArrow) ? MAX_SPEED : 0f;
            float currentSpeed = _rb.linearVelocity.magnitude;
            float acceleration = targetSpeed > currentSpeed ? ACCELERATION : DECELERATION;
            float newSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * deltaTime);

            Vector2 direction = transform.up;
            _rb.linearVelocity = direction * newSpeed;
        }

        private void WrapAroundScreen()
        {
            Vector2 pos = _rb.position;
            float minX = _worldBounds.min.x;
            float maxX = _worldBounds.max.x;
            float minY = _worldBounds.min.y;
            float maxY = _worldBounds.max.y;

            if (pos.x > maxX) pos.x -= _worldBounds.size.x;
            else if (pos.x < minX) pos.x += _worldBounds.size.x;

            if (pos.y > maxY) pos.y -= _worldBounds.size.y;
            else if (pos.y < minY) pos.y += _worldBounds.size.y;

            _rb.position = pos;
        }

        private void NotifyModel()
        {
            _model.SetPosition(_rb.position);
            _model.SetRotation(_rb.rotation);
            _model.SetSpeed(_rb.linearVelocity.magnitude);
        }

        private void HandleSpawnBullet(int id, BulletData bulletData)
        {
            Vector2 spawnPosition = _bulletSpawnPoint.position;
            bulletData.Position = spawnPosition;

            GameObject bulletObj = Instantiate(_bulletPrefab, spawnPosition, transform.rotation);
            Bullet bullet = bulletObj.GetComponent<Bullet>();
            bullet.Initialize(id, bulletData, _bulletService);
        }

        private void HandleSpawnLaser(int id, LaserData laserData)
        {
            Vector2 spawnPosition = _bulletSpawnPoint.position;
            laserData.Position = spawnPosition;

            GameObject laserObj = Instantiate(_laserPrefab, spawnPosition, transform.rotation, _bulletSpawnPoint);
            Laser laser = laserObj.GetComponent<Laser>();
            laser.Initialize(id, _bulletService);
        }
    }
}