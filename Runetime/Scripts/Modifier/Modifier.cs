
using System;
using UnityEngine;

namespace Mosaic
{
    public interface IModifier
    {
        public int GetPriority();
        public void SetProcess(Guid id, ModifierProcess process);
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
        private Guid _id;
        public int GetPriority()
        {
            return -1;
        }
        public void SetProcess(Guid id, ModifierProcess process)
        {
            _id = id;
            _process = process;
        }
        protected ICharacterCore GetCore()
        {
            return _process.GetCore();
        }
        protected ICharacterCore GetOrigin()
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