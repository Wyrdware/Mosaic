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
        public abstract void Initialize(ModifierProcess process, Guid id, Guid setID);
        public abstract void Tick();
        public abstract YieldInstruction Yield();
    }
    public abstract class ModifierDecorator<T> : ModifierDecorator where T : IModifier
    {
        [SerializeField]
        private int _priority;

        //This is used to access the child of the decorator
        private Guid _id;


        //this allows the decorator to propogate it's own set to any generated instances
        protected Guid SetID;

        private ModifierProcess _process;

        public sealed override int GetPriority()
        {
            return _priority;
        }
        public sealed override void Initialize(ModifierProcess process, Guid id, Guid setID)
        {
            _id = id;
            SetID = setID;
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