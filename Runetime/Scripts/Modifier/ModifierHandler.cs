using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mosaic
{
    public class ModifierHandler
    {
        private readonly List<ModifierProcess> _modifiers= new();
        private readonly ICharacterCore _core;
        public ModifierHandler(ICharacterCore core, List<Modifier> modifiers)
        {
            _core = core;
            foreach(Modifier modifier in modifiers)
            {
                ActivateMod(modifier);
            }
        }


        
        public ModifierProcess ActivateMod(Modifier modifier)
        {

            ModifierProcess newModifier = ModifierProcess.CreateModifier(modifier, _core);
            _modifiers.Add(newModifier);
            newModifier.SubscribeToEnd(() => { _modifiers.Remove(newModifier); } );
            return newModifier;
        }

        //TODO add GetMod(params) to get a specific modifier that can then be removed/monitored

    }
}