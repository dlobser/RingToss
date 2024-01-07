using UnityEngine;

namespace Quilt.Flappy
{
    [CreateAssetMenu(fileName = "CreationSettingsFlappy", menuName = "Quilt/CreationSettingsFlappy", order = 1)]
    public class CreationSettings : Quilt.CreationSettings
    {
        // public Projectile projectilePrefab;
        // public Platform platformPrefab;
        public ProjectileSimpleConfig projectileConfig;
        public PlatformHoopConfig platformConfig;
        public Vector2 minMaxPlatformHeight;
        public Vector2 minMaxProjectileForce;
        public Vector2 minMaxPlatformSize;
        public Vector2 projectileDirection;
        public Vector2 minMaxPlatformSeparation;
        public int maxPlatforms;
        public GameObject camera;
    }
}