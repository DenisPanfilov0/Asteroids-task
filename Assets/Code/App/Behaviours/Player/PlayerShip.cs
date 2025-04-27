using System;
using Code.App.Extensions;
using Code.App.Services.Interfaces;
using Code.App.Services.Models;
using UnityEngine;
using Zenject;

namespace Code.App.Behaviours.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerShip : MonoBehaviour
    {
        [SerializeField] private GameObject _bulletPrefab;
        [SerializeField] private GameObject _laserPrefab;
        [SerializeField] private Transform _bulletContainer;
        [SerializeField] private Transform _bulletSpawnPoint;

        private Rigidbody2D _rb;
        private IBulletService _bulletService;
        private float _screenWidth;
        private float _screenHeight;
        
        private Bounds _worldBounds;
        private Vector2 _startPosition;

        private const float MaxSpeed = 100f;
        private const float Acceleration = 50f;
        private const float Deceleration = 40f;
        private const float RotationSpeed = 75f;

        public event Action<Vector2> OnPositionChanged;
        public event Action<float> OnRotationChanged;
        public event Action<float> OnSpeedChanged;

        [Inject]
        public void Construct(IBulletService bulletService)
        {
            _bulletService = bulletService;
        }

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _rb.gravityScale = 0f;
            _rb.linearDamping = 0f;
            _rb.angularDamping = 0f;
            _rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }

        private void Start()
        {
            _worldBounds = Camera.main.GetOrthographicBounds();
            _startPosition = _worldBounds.center;
            transform.position = _startPosition;
            _rb.position = _startPosition;

            _bulletService.OnSpawnBullet += HandleSpawnBullet;
            _bulletService.OnSpawnLaser += HandleSpawnLaser;

            OnPositionChanged?.Invoke(transform.position);
            OnRotationChanged?.Invoke(transform.eulerAngles.z);
            OnSpeedChanged?.Invoke(0f);
        }

        private void OnDestroy()
        {
            _bulletService.OnSpawnBullet -= HandleSpawnBullet;
            _bulletService.OnSpawnLaser -= HandleSpawnLaser;
        }

        private void Update()
        {
            HandleInput();
            WrapAroundScreen();
            NotifyChanges();
        }

        private void HandleInput()
        {
            var deltaTime = Time.deltaTime;
            float rotationInput = 0f;
            if (Input.GetKey(KeyCode.LeftArrow))
                rotationInput = RotationSpeed;
            else if (Input.GetKey(KeyCode.RightArrow))
                rotationInput = -RotationSpeed;

            _rb.angularVelocity = rotationInput;

            float targetSpeed = Input.GetKey(KeyCode.UpArrow) ? MaxSpeed : 0f;
            float currentSpeed = _rb.linearVelocity.magnitude;
            float acceleration = targetSpeed > currentSpeed ? Acceleration : Deceleration;
            float newSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * deltaTime);

            Vector2 direction = transform.up;
            _rb.linearVelocity = direction * newSpeed;

            if (Input.GetKey(KeyCode.Q))
                _bulletService.Tick();
            if (Input.GetKeyDown(KeyCode.E))
                _bulletService.Tick();
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

        private void NotifyChanges()
        {
            OnPositionChanged?.Invoke(_rb.position);
            OnRotationChanged?.Invoke(_rb.rotation);
            OnSpeedChanged?.Invoke(_rb.linearVelocity.magnitude);
        }

        private void HandleSpawnBullet(int id, BulletData bulletData)
        {
            Vector2 spawnPosition = _bulletSpawnPoint.position;
            bulletData.Position = spawnPosition;

            GameObject bulletObj = Instantiate(_bulletPrefab, spawnPosition, transform.rotation, _bulletContainer);
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

        public void Reset()
        {
            _rb.position = _startPosition;
            _rb.rotation = 0f;
            _rb.linearVelocity = Vector2.zero;
            _rb.angularVelocity = 0f;
            transform.position = _startPosition;
            transform.rotation = Quaternion.identity;

            OnPositionChanged?.Invoke(_rb.position);
            OnRotationChanged?.Invoke(_rb.rotation);
            OnSpeedChanged?.Invoke(0f);
        }

        public Vector2 GetPosition() => _rb != null ? _rb.position : Vector2.zero;
        public float GetRotation() => _rb != null ? _rb.rotation : 0f;
        public float GetSpeed() => _rb != null ? _rb.linearVelocity.magnitude : 0f;
    }
}