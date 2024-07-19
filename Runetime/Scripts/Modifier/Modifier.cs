
using Codice.CM.Common;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using Unity.VisualScripting;
using UnityEngine;

namespace Mosaic
{
    public interface IModifier
    {
        public int GetPriority();
        public void SetProcess(ModifierProcess process);
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
        public int GetPriority()
        {
            return -1;
        }
        public void SetProcess(ModifierProcess process)
        {
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
        public abstract void Begin();//yield can be set at begin
        public abstract void Tick();
        public abstract void End();
    }

    public abstract class CModifierDuration : Modifier
    {
        [SerializeField]
        protected float _duration = 5f;

        public override bool EndCondition()
        {
            return Time.time < GetStartTime() + _duration;
        }
    }
}