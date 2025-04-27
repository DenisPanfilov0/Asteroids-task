using UnityEngine;

namespace Code.App.Services.Models
{
    public class EnemyData
    {
        public Vector2 Position;
        public Vector2 Direction;
        public float Speed;
        public bool IsAsteroid;
        public bool IsSmallAsteroid;
        public int ParentAsteroidId;

        public EnemyData(Vector2 position, Vector2 direction, float speed, bool isAsteroid, bool isSmallAsteroid, int parentAsteroidId = -1)
        {
            Position = position;
            Direction = direction.normalized;
            Speed = speed;
            IsAsteroid = isAsteroid;
            IsSmallAsteroid = isSmallAsteroid;
            ParentAsteroidId = parentAsteroidId;
        }
    }
}