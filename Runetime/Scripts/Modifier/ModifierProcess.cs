
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
        protected ICharacterCore Core;
        protected ICharacterCore Origin;

        protected float StartTime;
        protected YieldInstruction Yield = null;//ticks every frame

        private Coroutine _process;
        private bool _isInstance = false;

        public delegate void EndEventHandler();
        private event EndEventHandler EndEvent;

        public static ModifierProcess CreateModifier(ModifierProcess modifier, ICharacterCore core, ICharacterCore origin)
        {
            ModifierProcess newModifier = Instantiate(modifier);

            newModifier.Core = core;
            newModifier.Origin = origin;//Modifier Process must check if the originator is null

            newModifier.StartTime = Time.time;
            newModifier._process = core.monoBehaviour.StartCoroutine(newModifier.Process());
            newModifier._isInstance = true;

            return  newModifier;
        }

        public void SubscribeToEnd(EndEventHandler endMod)
        {
            Debug.Assert(_isInstance);
            EndEvent += endMod;
        }

        public void Clear()
        {
            Debug.Assert(_isInstance);
            Core.monoBehaviour.StopCoroutine(_process);
            End();
            EndEvent.Invoke();
            Destroy(this);

        }

        private IEnumerator Process()
        {
            Begin();

            while(EndCondition())
            {
                Tick();
                yield return Yield;
            }

            Clear();
        }

        protected abstract bool EndCondition();//Confusing wording, while true the process will continue
        protected abstract void Begin();//yield can be set at begin
        protected abstract void Tick();
        protected abstract void End();
    }

    public abstract class CModifierDuration : ModifierProcess
    {
        [SerializeField]
        protected float _duration = 5f;

        protected override bool EndCondition()
        {
            return Time.time < StartTime + _duration;
        }
    }
}