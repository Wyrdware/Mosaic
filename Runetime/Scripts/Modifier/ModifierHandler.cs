using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mosaic
{
    public class ModifierHandler
    {
        private readonly ICharacterCore _core;
        //Create a wrapper class for all modifiers, this will handle the process along with adding and removing decorators.
        private readonly Dictionary<Type, List<Guid>> _processByType= new();// using a diction
        private readonly Dictionary<Type, List<Guid>> _decoratorsByType = new();

        //We track each instance of a modifier so that we can easily remove the specific one that was added.
        private readonly Dictionary<Guid, ModifierProcess> _processByID = new();
        private readonly Dictionary<Guid, ModifierDecorator> _decoratorByID= new();


        public ModifierHandler(ICharacterCore core, List<Modifier> modifiers, List<ModifierDecorator> decorators)
        {
            _core = core;

            foreach(ModifierDecorator decorator in decorators)
            {
                AddModifierDecorator(decorator);
            }
            foreach(Modifier modifier in modifiers)
            {
                ApplyModifier(modifier, _core);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// The Decorator will apply to type T.
        /// <param name="decorator"></param>
        /// A special type of modifier that will be applied to all modifiers of type T.
        /// <param name="origin"></param>
        /// The core that is responsible for initiating this decorator.
        /// <returns></returns>
        public Guid AddModifierDecorator(ModifierDecorator decorator)
        {
            //create unique id for the modifier
            Guid id = Guid.NewGuid();
            // store the unique id and the modifier in a dictionary
            _decoratorByID.Add(id, decorator);

            _decoratorsByType.TryAdd(decorator.GetComponentType(), new List<Guid>());
            _decoratorsByType[decorator.GetComponentType()].Add(id);


            if (_processByType.ContainsKey(decorator.GetComponentType()))
            {
                foreach (Guid processID in _processByType[decorator.GetComponentType()])
                {
                    _processByID[processID].AddDecorator(id, decorator);
                }
            }


            return id;
        }

        public void RemoveModifierDecorator(Guid id)
        {
            ModifierDecorator decorator = _decoratorByID[id];

            //Remove decorator from all modifiers it applies to
            foreach (Guid processID in _processByType[_decoratorByID[id].GetComponentType()])
            {
                _processByID[processID].RemoveDecorator(id);
                
            }
            _decoratorByID.Remove(id);
            _decoratorsByType[decorator.GetComponentType()].Remove(id);
        }
           
        public Guid ApplyModifier(Modifier modifier, ICharacterCore origin)
        {
            Guid id = Guid.NewGuid();
            Type modifierType = modifier.GetType();

            //Get a list of all modifier decorators
            
            List<(Guid, ModifierDecorator)> idDecorator = new(); 
            if(_decoratorsByType.TryGetValue(modifierType, out List<Guid> decorators))
            {
                foreach (Guid decoratorID in decorators)
                {
                    idDecorator.Add((decoratorID, _decoratorByID[decoratorID]));
                }
            }

            //Create the new process
            ModifierProcess newProcess = new ModifierProcess(id, modifier, idDecorator, _core, origin);//Create an instane of the modifier to be manipulated

            _processByType.TryAdd(modifierType, new List<Guid>());
            _processByType[modifierType].Add(id);
            _processByID.Add(id, newProcess);

            //Putting this in the handler instead of the modifier process itself so the entire lifecycle is handled in a single location
            newProcess.SubscribeToEnd(() => 
            { 
                _processByType[modifierType].Remove(id);
                _processByID.Remove(id);
            
            } );//When a modifier is removed, there is no need to worry about the decorators since they are part of the object

            return id;

        }
    }
}