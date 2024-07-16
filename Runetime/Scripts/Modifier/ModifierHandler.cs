using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mosaic
{
    public class ModifierHandler
    {
        private readonly List<ModifierProcess> _modifiers= new();
        private readonly ICharacterCore _core;
        private readonly Dictionary<Type, List<ModifierDecorator>> _modifierDecorators = new();
        public ModifierHandler(ICharacterCore core, List<ModifierProcess> modifiers, List<ModifierDecorator> decorators)
        {
            _core = core;

            foreach(ModifierDecorator decorator in decorators)
            {
                AddModifierDecorator(decorator, _core);
            }
            foreach(ModifierProcess modifier in modifiers)
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
        public ModifierDecorator AddModifierDecorator(ModifierDecorator decorator, ICharacterCore origin)
        {
            ModifierProcess newModifierDecorator = decorator;

            _modifierDecorators.TryAdd(decorator.GetComponentType(), new List<ModifierDecorator>());
            _modifierDecorators[decorator.GetComponentType()].Add(newModifierDecorator as ModifierDecorator);
            
            


            //TODO: Apply modifier decorator to all modfifiers of type T?




            return newModifierDecorator as ModifierDecorator;
        }

        public void RemoveModifierDecorator(ModifierDecorator decorator)
        {
            _modifierDecorators[decorator.GetComponentType()].Remove(decorator);

            //Remove decorator from all modifiers it applies to
        }
           
        public ModifierProcess ApplyModifier(ModifierProcess modifier, ICharacterCore origin)
        {

            ModifierProcess newModifier = ModifierProcess.Initialize(modifier,_core, origin);//Create an instane of the modifier to be manipulated
            //Apply all decorators to the modifier
            if (_modifierDecorators.TryGetValue(modifier.GetType(), out List<ModifierDecorator> decorators))
            {
                foreach (ModifierDecorator decorator in decorators)// Ordering the list could be used for a sort of priority
                {
                    Debug.Log("Adding a decorator");
                    ModifierDecorator instance = ModifierProcess.Initialize(decorator, _core, origin) as ModifierDecorator;//Make an instance to be manipulated

                    instance.Decorate(newModifier);// Adds a new instance to each of the modifiers.
                    newModifier = instance;
                }
            }
            newModifier = ModifierProcess.ActivateInstance(newModifier);
            
            



            _modifiers.Add(newModifier);

            newModifier.SubscribeToEnd(() => { _modifiers.Remove(newModifier); } );//When a modifier is removed, there is no need to worry about the decorators since they are part of the object
            return newModifier;
        }

        //TODO add GetMod(params) to get a specific modifier that can then be removed/monitored

    }
}