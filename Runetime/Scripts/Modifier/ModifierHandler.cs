using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mosaic
{
    public class ModifierHandler
    {
        //Create a wrapper class for all modifiers, this will handle the process along with adding and removing decorators.
        private readonly Dictionary<Type, List<ModifierProcess>> _modifiers= new();// using a diction
        private readonly ICharacterCore _core;
        private readonly Dictionary<Type, List<ModifierDecorator>> _modifierDecorators = new();
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
        public void AddModifierDecorator(ModifierDecorator decorator)
        {

            _modifierDecorators.TryAdd(decorator.GetComponentType(), new List<ModifierDecorator>());
            _modifierDecorators[decorator.GetComponentType()].Add(decorator as ModifierDecorator);


            //TODO: Apply modifier decorator to all modfifiers of type T?
            if (_modifiers.ContainsKey(decorator.GetComponentType()))
            {
                foreach (ModifierProcess modifierProcess in _modifiers[decorator.GetComponentType()])
                {
                    modifierProcess.AddDecorator(decorator);
                }
            }
        }

        public void RemoveModifierDecorator(ModifierDecorator decorator)
        {
            _modifierDecorators[decorator.GetComponentType()].Remove(decorator);


            //Remove decorator from all modifiers it applies to
            foreach (ModifierProcess modifierProcess in _modifiers[decorator.GetComponentType()])
            {
                modifierProcess.RemoveDecorator(decorator);
            }
        }
           
        public void ApplyModifier(Modifier modifier, ICharacterCore origin)
        {
            
            Type modifierType = modifier.GetType();

            //Get a list of all modifier decorators
            List<ModifierDecorator> decorators = new();
            _modifierDecorators.TryGetValue(modifierType, out decorators);

            //Create the new process
            ModifierProcess newProcess = new ModifierProcess(modifier, decorators, _core, origin);//Create an instane of the modifier to be manipulated

            _modifiers.TryAdd(modifierType, new List<ModifierProcess>());
            _modifiers[modifierType].Add(newProcess);

            //Putting this in the handler instead of the modifier process itself so the entire lifecycle can be easily accessed
            newProcess.SubscribeToEnd(() => { _modifiers[modifierType].Remove(newProcess); } );//When a modifier is removed, there is no need to worry about the decorators since they are part of the object

        }
        //TODO add GetMod(params) to get a specific modifier that can then be removed/monitored

    }
}