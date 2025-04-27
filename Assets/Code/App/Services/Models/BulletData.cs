using UnityEngine;

namespace Code.App.Services.Models
{
    public class BulletData
    {
        public Vector2 Position;
        public Vector2 Direction;
        public float Speed;

        public BulletData(Vector2 position, Vector2 direction, float speed)
        {
            Position = position;
            Direction = direction.normalized;
            Speed = speed;
        }
    }
}