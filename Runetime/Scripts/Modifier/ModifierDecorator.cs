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
        public abstract void SetProcess(Guid id, ModifierProcess process);
        public abstract void Tick();
        public abstract YieldInstruction Yield();
    }
    public abstract class ModifierDecorator<T> : ModifierDecorator where T : IModifier
    {
        [SerializeField]
        private int _priority;
        private Guid _id;

        private ModifierProcess _process;

        public sealed override int GetPriority()
        {
            return _priority;
        }
        public sealed override void SetProcess(Guid id, ModifierProcess process)
        {
            _id = id;
            _process = process;
        }
        public sealed override Type GetComponentType()
        {
            return typeof(T);
        }


        private IModifier GetComponent()
        {
            return _process.GetChildOfDecorator(_id);
        }
        protected T GetModifier()
        {
            return (T) _process.GetModifier();
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


        //The following methods should be overriden
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