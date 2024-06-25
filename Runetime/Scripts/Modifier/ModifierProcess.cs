
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using Unity.VisualScripting;
using UnityEngine;

namespace Mosaic
{
    [System.Serializable] 
    public class Modifier
    {
        public ModifierProcess Process;
        public ICharacterCore Originator;
    }
    public abstract class ModifierProcess : ScriptableObject
    {
        protected ICharacterCore _target;
        protected ICharacterCore _originator;

        protected float _startTime;
        protected YieldInstruction _tick = null;//ticks every frame

        private Coroutine _process;
        private bool _isInstance = false;

        public delegate void EndEventHandler();
        private event EndEventHandler EndEvent;

        public static ModifierProcess CreateModifier(Modifier modifier, ICharacterCore target)
        {
            ModifierProcess newModifier = Instantiate(modifier.Process);

            newModifier._target = target;
            newModifier._originator = modifier.Originator == null? target : modifier.Originator;

            newModifier._startTime = Time.time;
            newModifier._process = target.monoBehaviour.StartCoroutine(newModifier.Process());
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
            _target.monoBehaviour.StopCoroutine(_process);
            End();
            EndEvent.Invoke();
            Destroy(this);

        }

        private IEnumerator Process()
        {
            Begin();

            while(EndCondition())
            {
                yield return _tick;
                Tick();
            }

            Clear();
        }

        protected abstract bool EndCondition();
        protected abstract void Begin();
        protected abstract void Tick();
        protected abstract void End();
    }

    public abstract class CModifierDuration : ModifierProcess
    {
        [SerializeField]
        protected float _duration = 5f;

        protected override bool EndCondition()
        {
            return Time.time < _startTime + _duration;
        }
    }
}