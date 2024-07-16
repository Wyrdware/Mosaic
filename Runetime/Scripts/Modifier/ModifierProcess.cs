
using Codice.CM.Common;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using Unity.VisualScripting;
using UnityEngine;

namespace Mosaic
{
    [System.Serializable] 

    public abstract class ModifierProcess : ScriptableObject
    {
        private ICharacterCore _core;
        private ICharacterCore _origin;

        private float _startTime;

        private Coroutine _process;
        private bool _isInstance = false;

        public delegate void EndEventHandler();
        private event EndEventHandler _endEvent;


        public static ModifierProcess Initialize(ModifierProcess modifier, ICharacterCore core, ICharacterCore Origin)
        {
            ModifierProcess newModifier = Instantiate(modifier);
            newModifier._core = core;
            newModifier._origin = Origin;
            newModifier._isInstance = true;
            modifier._startTime = Time.time;
            return newModifier;
        }
        public static ModifierProcess ActivateInstance(ModifierProcess modifier)
        {
            Debug.Assert(modifier._isInstance);

            modifier._process = modifier._core.monoBehaviour.StartCoroutine(Process(modifier));

            return  modifier;
        }


        public virtual ICharacterCore GetCore()
        {
            return _core;
        }
        public virtual ICharacterCore GetOrigin()
        {
            return _origin;
        }
        public virtual float GetStartTime()
        {
            return _startTime;
        }
        public virtual YieldInstruction Yield()
        {
            return null;
        }

        public virtual void SubscribeToEnd(EndEventHandler endMod)
        {
            Debug.Assert(_isInstance);
            _endEvent += endMod;
        }

        public void Clear()
        {
            Debug.Assert(_isInstance);
            _core.monoBehaviour.StopCoroutine(_process);
            End();
            _endEvent.Invoke();
            Destroy(this);

        }

        private static IEnumerator Process(ModifierProcess modifier)
        {
            modifier.Begin();

            while(modifier.EndCondition())
            {
                modifier.Tick();
                yield return modifier.Yield();
            }

            modifier.Clear();
        }



        public abstract bool EndCondition();//Confusing wording, while true the process will continue
        public abstract void Begin();//yield can be set at begin
        public abstract void Tick();
        public abstract void End();
    
    
    }

    public abstract class CModifierDuration : ModifierProcess
    {
        [SerializeField]
        protected float _duration = 5f;

        public override bool EndCondition()
        {
            return Time.time < GetStartTime() + _duration;
        }
    }
}