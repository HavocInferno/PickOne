using UnityEngine;
using UnityEngine.Events;

namespace Game.Controls
{
    abstract public class AbstractController<T> : MonoBehaviour
    {
        public class OnChangeEvent : UnityEvent<T> { };
        public OnChangeEvent OnChange = new OnChangeEvent();

        abstract protected T ReadControls(float timeDelta);

        public void Update()
        {
            if (isActiveAndEnabled)
            {
                OnChange.Invoke(ReadControls(Time.deltaTime));
            }
        }
    }

    // This instance is defined in order to expose controllers to UnityEditor.
    [System.Serializable]
    abstract public class AbstractControllerVector2 : AbstractController<Vector2> { }
}