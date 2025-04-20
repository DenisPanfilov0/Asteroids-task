using Code.App.Behaviours.Weapon;
using Code.App.Services;
using Code.App.Services.PlayerShip;
using UnityEngine;
using Zenject;

namespace Code.App.Behaviours
{
    public class PlayerShip : MonoBehaviour
    {
        private IShipPositionService _shipPositionService;
        private IShipMovementService _shipMovementService;
        private IBulletService _bulletService;

        [SerializeField] private GameObject _bulletPrefab;
        [SerializeField] private GameObject _laserPrefab;
        [SerializeField] private Transform _bulletContainer;
        [SerializeField] private Transform _bulletSpawnPoint;

        [Inject]
        public void Construct(
            IShipPositionService shipPositionService,
            IShipMovementService shipMovementService,
            IBulletService bulletService)
        {
            _shipPositionService = shipPositionService;
            _shipMovementService = shipMovementService;
            _bulletService = bulletService;
        }

        private void Start()
        {
            _shipPositionService.Setup(transform.position, Screen.width, Screen.height);

            _shipPositionService.OnChangeRotationAngle += HandleRotationChange;
            _shipPositionService.OnChangeCurrentPosition += HandlePositionChange;
            _bulletService.OnSpawnBullet += HandleSpawnBullet;
            _bulletService.OnSpawnLaser += HandleSpawnLaser;
        }

        private void Update()
        {
            _shipMovementService.UpdateInput();
            _shipPositionService.UpdatePosition(Time.deltaTime);
        }

        private void OnDestroy()
        {
            _shipPositionService.OnChangeRotationAngle -= HandleRotationChange;
            _shipPositionService.OnChangeCurrentPosition -= HandlePositionChange;
            _bulletService.OnSpawnBullet -= HandleSpawnBullet;
            _bulletService.OnSpawnLaser -= HandleSpawnLaser;
        }

        private void HandleRotationChange(Vector3 rotationAngle)
        {
            transform.rotation = Quaternion.Euler(0f, 0f, rotationAngle.z);
        }

        private void HandlePositionChange(Vector2 normalizedPosition)
        {
            float x = normalizedPosition.x * _shipPositionService.GetDataModel().ScreenWidth;
            float y = normalizedPosition.y * _shipPositionService.GetDataModel().ScreenHeight;
            transform.position = new Vector3(x, y, transform.position.z);
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
    }
}