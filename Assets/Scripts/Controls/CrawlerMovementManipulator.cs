using UnityEngine;

namespace Game.Controls
{
    [AddComponentMenu("Controls/CrawlerMovementManipulator")]
    [RequireComponent(typeof(Crawler))]
    public class CrawlerMovementManipulator : AbstractManipulatorVector2
    {
        public Transform aim;
        public float velocity = 1.0f;
        public float rotationSpeed = 1.0f;
        public Vector3 projectionPlaneNormal = Vector3.up;

        protected override void HandleChange(Vector2 delta)
        {
            var crawler = GetComponent<Crawler>();
            Vector3 direction = velocity * (aim.right * delta.x + aim.forward * delta.y);
            crawler.Move(direction);
        }
    }

}
