using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Mosaic
{
    /// <summary>
    /// One of the two default components required for a character to function.
    /// </summary>
    [RequireComponent(typeof(ICore))]
    public abstract class CoreInput : MonoBehaviour
    {
        protected BehaviorInstance BehaviorInstance;

        public abstract void OnRespawn();
        public void OverrideControl(BehaviorInstance behaviorInstance)
        {

            if (this.BehaviorInstance != null)
            {
                this.BehaviorInstance.ControlEnd();
            }

            this.BehaviorInstance = behaviorInstance;

            if (behaviorInstance != null)
            {
                behaviorInstance.ControlStart();
            }
        }
    }

}