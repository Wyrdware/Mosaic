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
            foreach ((Modifier, ICore) modifier in _eventModifiers[eventType])
            {
                _characterCore.Modifiers.ApplyModifier(modifier.Item1, modifier.Item2);
            }
        }
        public void AddEventMod(ModifierEventType eventType, Modifier modifier, ICore origin)
        {
            _eventModifiers[eventType].Add((modifier,origin));
        }
        public void RemoveEventMod(ModifierEventType eventType, Modifier modifier, ICore origin)
        {
            _eventModifiers[eventType].Remove((modifier,origin));
        }
    }
}