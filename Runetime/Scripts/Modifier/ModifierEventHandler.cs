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
            public List<(Modifier,ICharacterCore)> Mods;
        }

        private ICharacterCore _characterCore;
        private Dictionary<ModifierEventType, List<(Modifier,ICharacterCore)>> _eventModifiers = new();

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
            foreach ((Modifier, ICharacterCore) modifier in _eventModifiers[eventType])
            {
                _characterCore.Modifiers.ApplyModifier(modifier.Item1, modifier.Item2);
            }
        }
        public void AddEventMod(ModifierEventType eventType, Modifier modifier, ICharacterCore origin)
        {
            _eventModifiers[eventType].Add((modifier,origin));
        }
        public void RemoveEventMod(ModifierEventType eventType, Modifier modifier, ICharacterCore origin)
        {
            _eventModifiers[eventType].Remove((modifier,origin));
        }
    }
}