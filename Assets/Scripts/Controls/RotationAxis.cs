using UnityEngine;

namespace Game.Controls
{
    [System.Serializable]
    public class RotationAxis
    {
        private float angle = 0.0f;

        public Vector3 axis;
        public float sensivity;
        public bool unbound;
        public float minimum;
        public float maximum;
        public float Angle
        {
            set
            {
                angle = unbound ? (value % 360.0f) :
                    Mathf.Clamp(value, minimum, maximum);
            }
            get
            {
                Angle = angle;
                return angle;
            }
        }

        public Quaternion GetQuaternion()
        {
            return Quaternion.AngleAxis(Angle, axis);
        }
    }
}