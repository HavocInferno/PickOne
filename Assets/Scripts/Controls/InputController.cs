using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Controls
{
    [AddComponentMenu("Controls/InputController")]
    public class InputController : AbstractControllerVector2
    {
        public bool normalize = false;

        public string axisXName = "Mouse X";
        public string axisYName = "Mouse Y";

        protected override Vector2 ReadControls(float timeDelta)
        {
            Vector2 input = new Vector2(
                Input.GetAxis(axisXName),
                Input.GetAxis(axisYName));
            if (normalize)
                input.Normalize();
            return timeDelta * input;
        }
    }
}
