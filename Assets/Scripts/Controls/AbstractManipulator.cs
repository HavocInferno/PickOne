using System.Collections.Generic;
using UnityEngine;

namespace Game.Controls
{
    abstract public class AbstractManipulator<T> : MonoBehaviour
    {
        protected virtual void HandleChange(T value) { }
    }

    // This instance is defined only to expose the subscriber to UnityEditor.
    abstract public class AbstractManipulatorVector2 : AbstractManipulator<Vector2>
    {
        [System.Serializable]
        public class Subscriber
        {
            public void Subscribe(AbstractManipulatorVector2 manipulator)
            {
                foreach (var controller in controllers)
                    controller.OnChange.AddListener(manipulator.HandleChange);
            }

            public List<AbstractControllerVector2> controllers = new List<AbstractControllerVector2>();
        }

        [SerializeField]
        public Subscriber subscriber;

        public void Awake()
        {
            subscriber.Subscribe(this);
        }
    }
}