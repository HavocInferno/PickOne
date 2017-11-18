using UnityEngine;

namespace Game.Controls
{
    /// <summary>
    /// Script that translates a transform along two specified axis
    /// and can serve as a callback for any Controller<Vector2>. 
    /// </summary>
    [AddComponentMenu("Controls/TransformTranslationManipulator")]
    public class TransformTranslationManipulator : AbstractManipulatorVector2
    {
        public Vector3 translateAxisX = Vector3.forward;
        public Vector3 translateAxisY = Vector3.right;

        protected override void HandleChange(Vector2 delta)
        {
            transform.Translate(translateAxisX * delta.x + translateAxisY * delta.y);
        }
    }

}
