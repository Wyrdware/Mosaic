using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Codice.Client.BaseCommands.Import.Commit;

namespace Mosaic
{
    public class ModifierProcess
    {

        //used to track added modifiers
        private Dictionary<IModifier, IModifier> originalToInstancedMap = new();
        
        /// <summary>
        /// Index of 0 is the Leaf Modifier. All others are decorators.
        /// </summary>
        private List<IModifier> _modifiers;



        private ICharacterCore _core;
        private ICharacterCore _origin;

        private float _startTime;

        private Coroutine _process;
        private bool _isInstance = false;

        public delegate void EndEventHandler();
        private event EndEventHandler _endEvent;

        private Type _modifierType;


        public ModifierProcess(Modifier modifier, List<ModifierDecorator> decorators,ICharacterCore core, ICharacterCore origin)
        {
           
            //Set Values
            _core = core;
            _origin = origin;
            _isInstance = true;
            _startTime = Time.time;
            _modifierType = modifier.GetType();
            //Instanciate modifier
            Modifier instance = ScriptableObject.Instantiate(modifier);
            instance.SetProcess(this);
            _modifiers = new() { instance };
            originalToInstancedMap.Add(modifier, instance);
            //Instanciate decorators
            AddDecorator(decorators);

            //Start Process
            _process = _core.monoBehaviour.StartCoroutine(Process());

        }

        private IModifier CreateDecorator(ModifierDecorator decorator)
        {
            ScriptableObject decoratorAsSO = (ScriptableObject)decorator;//This mess of casting feels awful and is confusing to look at, but it works so I'm leaving it for now
            IModifier instance = (IModifier)ScriptableObject.Instantiate(decoratorAsSO);
            instance.SetProcess(this);
            return instance;
        }
        public void AddDecorator(ModifierDecorator decorator)
        {
            IModifier instance = CreateDecorator(decorator);
            _modifiers.Add(instance);
            originalToInstancedMap.Add(decorator, instance);
            _modifiers.Sort((x, y) => y.GetPriority().CompareTo(x.GetPriority()));
        }
        public void AddDecorator(List<ModifierDecorator> decorators)
        {
            foreach (ModifierDecorator decorator in decorators)
            {
                IModifier instance = CreateDecorator(decorator);
                _modifiers.Add(instance);
                originalToInstancedMap.Add(decorator, instance);
            }
            _modifiers.Sort((x, y) => y.GetPriority().CompareTo(x.GetPriority()));
        }

        //TODO: This likely won't work, we will need to add an ID system to save 
        public void RemoveDecorator(ModifierDecorator decorator)
        {
            IModifier instanceToRemove = originalToInstancedMap[decorator];
            originalToInstancedMap.Remove(decorator);

            _modifiers.Remove(instanceToRemove);
            _modifiers.Sort((x, y) => y.GetPriority().CompareTo(x.GetPriority()));
        }
        public IModifier GetChildOfDecorator(IModifier decorator)
        {
            int indexOfComponent = _modifiers.IndexOf(decorator) + 1;
            //This will through an index out of bounds error if called by the leaf modifier; 
            return _modifiers[indexOfComponent];
        }
        public ICharacterCore GetCore()
        {
            return _core;
        }
        public ICharacterCore GetOrigin()
        {
            return _origin;
        }
        public float GetStartTime()
        {
            return _startTime;
        }



        //TODO: Connect functions to modifiers.
        private void Begin()
        {
            _modifiers[0].Begin();
        }
        private bool EndCondition()
        {
            return _modifiers[0].EndCondition();
        }
        private void Tick()
        {
            _modifiers[0].Tick();
        }
        private YieldInstruction Yield()
        {
            return _modifiers[0].Yield();
        }
        private void End()
        {
            _modifiers[0].End();
        }



        public void SubscribeToEnd(EndEventHandler endMod)
        {
            _endEvent += endMod;
        }

        public void Clear()
        {
            Debug.Assert(_isInstance);
            _core.monoBehaviour.StopCoroutine(_process);
            End();

            foreach(var instance in _modifiers)
            {
                ScriptableObject.Destroy((ScriptableObject)instance);    
            }

            _endEvent.Invoke();
        }

        private IEnumerator Process()
        {
            Debug.Log("APPLYING " );
            Begin();

            while (EndCondition())
            {
                Tick();
                yield return Yield();
            }
            yield return null;
            Clear();
        }

    }
}

