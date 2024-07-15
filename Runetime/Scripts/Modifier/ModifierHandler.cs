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
        private readonly Dictionary<Type, List<ModifierProcessDecorator>> _modifierDecorators = new();
        public ModifierHandler(ICharacterCore core, List<ModifierProcess> modifiers)
        {
            _core = core;
            foreach(ModifierProcess modifier in modifiers)
            {
                ApplyModifier(modifier, _core);
            }
        }

        /*   public ModifierProcessDecorator ApplyModifierDecorator(ModifierProcessDecorator modifier, ICharacterCore origin)
           {
               ModifierProcessDecorator newModifierDecorator = ModifierProcessDecorator.CreateModifier(modifier, _core, origin) as ModifierProcessDecorator;
               return;
           }
           */
        public ModifierProcess ApplyModifier(ModifierProcess modifier, ICharacterCore origin)
        {

            ModifierProcess newModifier = ModifierProcess.CreateModifier(modifier, _core, origin);
            _modifiers.Add(newModifier);
            newModifier.SubscribeToEnd(() => { _modifiers.Remove(newModifier); } );
            return newModifier;
        }

        //TODO add GetMod(params) to get a specific modifier that can then be removed/monitored

    }
}