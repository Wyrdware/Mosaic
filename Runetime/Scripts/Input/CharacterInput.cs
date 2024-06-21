using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ModularCharacter
{
    /// <summary>
    /// One of the two default components required for a character to function.
    /// </summary>
    [RequireComponent(typeof(ICharacterCore))]
    public abstract class CharacterInput : MonoBehaviour
    {
        protected StateInstance stateInstance;

        public void OverrideControl(StateInstance cco)
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