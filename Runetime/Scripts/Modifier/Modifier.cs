
using System;
using UnityEngine;

namespace Mosaic
{
    public interface IModifier
    {
        public int GetPriority();
        public void Initialize(ModifierProcess process, Guid id, Guid setID);
        public YieldInstruction Yield();
        public bool EndCondition();
        public void Begin();
        public void Tick();
        public void End();

    }

    /// <summary>
    /// Concrete modifier class
    /// </summary>
    [System.Serializable]
    public abstract class Modifier : ScriptableObject , IModifier
    {
        private ModifierProcess _process;

        protected Guid SetID;
        public int GetPriority()
        {
            return -1;
        }
        public void Initialize(ModifierProcess process, Guid id, Guid setID)
        { 
            SetID = setID;
            _process = process;
        }
        protected ICore GetCore()
        {
            return _process.GetCore();
        }
        protected ICore GetOrigin()
        {
            return _process.GetOrigin();
        }
        protected float GetStartTime()
        {
            return _process.GetStartTime();
        }


        public virtual YieldInstruction Yield()
        {
            return null;
        }
        public abstract bool EndCondition();//Confusing wording, while true the process will continue
        public abstract void Begin();
        public abstract void Tick();
        public abstract void End();


    }


    // Test class for a modifier that exists only for a specified duration
    public abstract class ModifierDuration : Modifier
    {
        [SerializeField]
        protected float _duration = 5f;

        public override bool EndCondition()
        {
            return Time.time < GetStartTime() + _duration;
        }
    }
}