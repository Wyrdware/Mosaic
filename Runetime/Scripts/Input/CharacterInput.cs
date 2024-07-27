using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Mosaic
{
    /// <summary>
    /// One of the two default components required for a character to function.
    /// </summary>
    [RequireComponent(typeof(ICore))]
    public abstract class CharacterInput : MonoBehaviour
    {
        protected BehaviorInstance stateInstance;

        public void OverrideControl(BehaviorInstance cco)
        {

            if (this.stateInstance != null)
            {
                this.stateInstance.ControlEnd();
            }

            this.stateInstance = cco;

            if (cco != null)
            {
                cco.ControlStart();
            }
        }
    }

}