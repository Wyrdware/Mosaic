using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mosaic
{
    /// <summary>
    /// By taging the character with data, we can enable the sharing of data between modules
    /// as well as allowing data to persist outside of the modules lifetime.
    /// This is essential for featurse such as smooth movement between state transitions.
    /// DataTags can also be used to transmit information about hit events.
    /// </summary>
    public abstract class DataTag : ICloneable
    {
        private IDataTagUpdateEventTrigger _updateEventTrigger;

        /// <summary>
        /// Set all non persistent values to there default. The standard structure for a DataTag includes Default values and active values.
        /// Active values should be set back to there defaults on respawn. This includes values such as position, movement speed, health, etc.
        /// persistent values such as death count, score, inventory, etc should not be updated.
        /// </summary>
        public abstract void OnRespawn();

        public object Clone()
        {
            return this.MemberwiseClone();
        }

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
        public Vector3 DefaultPosition;
        public Quaternion DefaultRotation;

        public Vector3 Position;
        public Quaternion Rotation;

        public override void OnRespawn()
        {
            Position = DefaultPosition;
            Rotation = DefaultRotation;
        }
    }
    public class MovementDataTag : DataTag
    {
        public Vector3 DefaultDirection;
        public Vector3 DefaultVelocity;
        public Vector3 InputDirection;
        public Vector3 Velocity;

        public override void OnRespawn()
        {
            InputDirection = DefaultDirection;
            Velocity = DefaultVelocity;
        }
    }

}

