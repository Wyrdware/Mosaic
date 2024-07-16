using System;
using UnityEngine;

namespace Mosaic
{
    public abstract class ModifierDecorator : ModifierProcess //This allows us to avoid using the generic type outside of this class
    {
        public abstract void Decorate(ModifierProcess modifierProcess);
        public abstract Type GetComponentType();
        public abstract ModifierProcess GetComponent();
    }
    public abstract class ModifierProcessDecorator<T> : ModifierDecorator where T : ModifierProcess
    {
        protected T _component;//See decorator pattern for detailed explanation


        public override void Decorate(ModifierProcess modifierProcess)
        {
            Debug.Assert(modifierProcess is T);
            _component = modifierProcess as T;
        }
        public override ModifierProcess GetComponent()
        {
            return _component;
        }


        public override Type GetComponentType()
        {
            return typeof(T);
        }




        public override ICharacterCore GetCore()
        {
            return _component.GetCore();
        }
        public override ICharacterCore GetOrigin()
        {
            return _component.GetOrigin();
        }
        public override float GetStartTime()
        {
            return _component.GetStartTime();
        }
        public  override YieldInstruction Yield()
        {
            return _component.Yield();
        }

        public override void Begin()
        {
            _component.Begin();
        }

        public override void End()
        {
            _component.End();
        }

        public override bool EndCondition()
        {
            return _component.EndCondition();
        }

        public override void Tick()
        {
            _component.Tick();
        }


    }
}