using System;
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
            public List<(Modifier,ICore)> Mods;
        }

        private ICore _characterCore;
        private Dictionary<ModifierEventType, List<(Modifier,ICore)>> _eventModifiers = new();

        Guid placeholder = new Guid();
        public ModifierEventHandler(ICore characterCore, List<EventMods> mods)
        {
            this._characterCore = characterCore;
            foreach (EventMods eventMods in mods)
            {
                Debug.Assert(!_eventModifiers.ContainsKey(eventMods.Type));

                _eventModifiers[eventMods.Type] = eventMods.Mods;
            }
        }
        public void OnRespawn(List<EventMods> mods)
        {
            Debug.LogWarning("Respawn not fully impelemented.");
        }


        public void ActivateEvent(ModifierEventType eventType)
        {
            Debug.LogWarning("Event Mods not fully impelemented.");
            foreach ((Modifier, ICore) modifier in _eventModifiers[eventType])
            {
                _characterCore.Modifiers.AddModifier(modifier.Item1, modifier.Item2, placeholder);
            }
        }
        public void AddEventMod(ModifierEventType eventType, Modifier modifier, ICore origin)
        {
            Debug.LogWarning("Event Mods not fully impelemented.");
            _eventModifiers[eventType].Add((modifier,origin));
        }
        public void RemoveEventMod(ModifierEventType eventType, Modifier modifier, ICore origin)
        {
            Debug.LogWarning("Event Mods not fully impelemented.");
            _eventModifiers[eventType].Remove((modifier,origin));
        }
    }
}