using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Mosaic
{
    public class ModifierEventHandler
    {
        [System.Serializable]
        public struct EventMods
        {
            public ModifierEventType Type;
            public List<Modifier> Mods;
        }

        private ICharacterCore _characterCore;
        private Dictionary<ModifierEventType, List<Modifier>> _eventModifiers = new();

        public ModifierEventHandler(ICharacterCore characterCore, List<EventMods> mods)
        {
            this._characterCore = characterCore;
            foreach (EventMods eventMods in mods)
            {
                Debug.Assert(!_eventModifiers.ContainsKey(eventMods.Type));

                _eventModifiers[eventMods.Type] = eventMods.Mods;
            }
        }


        public void ActivateEvent(ModifierEventType eventType)
        {
            foreach (Modifier modifier in _eventModifiers[eventType])
            {
                _characterCore.Modifiers.ActivateMod(modifier);
            }
        }
        public void AddEventMod(ModifierEventType eventType, Modifier mod)
        {
            _eventModifiers[eventType].Add(mod);
        }
        public void RemoveEventMod(ModifierEventType eventType, Modifier mod)
        {
            _eventModifiers[eventType].Remove(mod);
        }
    }
}