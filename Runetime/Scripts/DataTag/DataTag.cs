using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ModularCharacter
{
    /// <summary>
    /// By taging the character with data, we can enable the sharing of data between modules
    /// as well as allowing data to persist outside of the modules lifetime.
    /// This is essential for featurse such as smooth movement between state transitions.
    /// DataTags can also be used to transmit information about hit events.
    /// </summary>
    public abstract class DataTag
    {
        private IDataTagUpdateEventTrigger _updateEventTrigger;
        public void SetHandler(IDataTagUpdateEventTrigger trigger)
        {
            _updateEventTrigger = trigger;
        }
        protected void TriggerUpdateEvent<T>() where T : DataTag
        {
            if (_updateEventTrigger != null)
            {
                _updateEventTrigger.TriggerUpdateEvent<T>(this);
            }
        }
    }


    public class TransformDataTag  : DataTag
    {
        public Vector3 Position;
        public Quaternion Rotation;
    }
    public class MovementDataTag : DataTag
    {
        public Vector3 InputDirection;
        public Vector3 Velocity;
    }

}

