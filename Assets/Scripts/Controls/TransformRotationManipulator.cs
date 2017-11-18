using UnityEngine;

namespace Game.Controls
{
    /// <summary>
    /// Script that rotates a transform around two specified axis
    /// and can serve as a callback for any Controller<Vector2>. 
    /// </summary>
    [AddComponentMenu("Controls/TransformRotationManipulator")]
    public class TransformRotationManipulator : AbstractManipulatorVector2
    {
        [SerializeField]
        private RotationAxis phi = new RotationAxis
        {
            axis = -Vector3.up,
            sensivity = 5.0f,
            unbound = true,
            minimum = 0.0f,
            maximum = 360.0f
        };

        [SerializeField]
        private RotationAxis theta = new RotationAxis
        {
            axis = Vector3.right,
            sensivity = 5.0f,
            unbound = false,
            minimum = 10.0f,
            maximum = 80.0f
        };

        public Vector3 transformAxisX = Vector3.forward;
        public Vector3 transformAxisY = Vector3.right;

        public float Phi
        {
            get
            {
                return phi.Angle;
            }
            set
            {
                phi.Angle = value;
                RecalculateRotation();
            }
        }

        public float Theta
        {
            get
            {
                return theta.Angle;
            }
            set
            {
                theta.Angle = value;
                RecalculateRotation();
            }
        }

        public bool swapRotationAxes = false;

        Quaternion originalRotation;

        protected override void HandleChange(Vector2 delta)
        {
            Rotate(delta);
        }

        public void Rotate(Vector2 delta)
        {
            phi.Angle = phi.Angle + delta.x * phi.sensivity;
            theta.Angle = theta.Angle + delta.y * theta.sensivity;
            RecalculateRotation();
        }

        void Start()
        {
            originalRotation = transform.localRotation;
            RecalculateRotation();
        }

        public Quaternion GetQuaternion()
        {
            if (swapRotationAxes)
            {
                return theta.GetQuaternion() * phi.GetQuaternion();
            }
            else
            {
                return phi.GetQuaternion() * theta.GetQuaternion();
            }
        }

        void RecalculateRotation()
        {
            transform.localRotation = originalRotation * GetQuaternion();
        }
    }

}
