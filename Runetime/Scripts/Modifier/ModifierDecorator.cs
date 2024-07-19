using System;
using UnityEngine;

namespace Mosaic
{
    public abstract class ModifierDecorator : ScriptableObject, IModifier //This allows us to avoid using the generic type outside of this class
    {
        public abstract void Begin();
        public abstract void End();
        public abstract bool EndCondition();
        public abstract Type GetComponentType();
        public abstract int GetPriority();
        public abstract void SetProcess(ModifierProcess process);
        public abstract void Tick();
        public abstract YieldInstruction Yield();
    }
    public abstract class ModifierDecorator<T> : ModifierDecorator where T : IModifier
    {
        [SerializeField]
        private int _priority;

        private ModifierProcess _process;

        public override int GetPriority()
        {
            return _priority;
        }
        public override void SetProcess(ModifierProcess process) // SMOOCH!
        {
            _process = process;
        }
        public override Type GetComponentType()
        {
            return typeof(T);
        }


        protected T GetComponent()
        {
            return (T)_process.GetChildOfDecorator(this);
        }
        public ICharacterCore GetCore()
        {
            return _process.GetCore();
        }
        public ICharacterCore GetOrigin()
        {
            return _process.GetOrigin();
        }
        public float GetStartTime()
        {
            return _process.GetStartTime();
        }



        public override YieldInstruction Yield()
        {
            return GetComponent().Yield();
        }

        public override void Begin()
        {
            GetComponent().Begin();
        }

        public override void End()
        {
            GetComponent().End();
        }

        public override bool EndCondition()
        {
            return GetComponent().EndCondition();
        }

        public override void Tick()
        {
            GetComponent().Tick();
        }


    }
}