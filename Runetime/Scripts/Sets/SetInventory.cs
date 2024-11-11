using Mosaic;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mosaic
{
    public class SetInventory : ISetInventory
    {
        //On Respawn add all items back to the character
        private Dictionary<Guid, ModuleSet> _setsByID = new();
        private HashSet<Guid> _activeIDs = new();
        private Action<Guid> _onSetUpdated;
        private ICore _core;


        public SetInventory(ICore core, List<ModuleSet> startingModuleSets)
        {
            _core = core;

            foreach (ModuleSet moduleSet in startingModuleSets) 
            { 
                AddItem(moduleSet,Guid.NewGuid());
            }

        }


        public void AddItem(ModuleSet set, Guid setID)
        {
            _setsByID.Add(setID, set);
            Activate(setID);
            _onSetUpdated?.Invoke(setID);
        }
        public void RemoveItem(Guid setID)
        {

            Deactivate(setID);
            _setsByID.Remove(setID) ;
            _onSetUpdated?.Invoke(setID);
        }



        public void Activate(Guid setID)
        {
            if (!_activeIDs.Contains(setID))
            {
                _activeIDs.Add(setID);
                foreach (Behavior behavior in _setsByID[setID].Behaviors)
                {
                    _core.StateMachine.AddBehavior(behavior, setID);
                }
                foreach (Modifier modifier in _setsByID[setID].Modifiers)
                {
                    _core.Modifiers.AddModifier(modifier, _core, setID);
                }
                foreach (ModifierDecorator decorator in _setsByID[setID].Decorators)
                {
                    _core.Modifiers.AddModifierDecorator(decorator, setID);
                }
                _onSetUpdated?.Invoke(setID);
            }
            else
            {
                Debug.LogError("Set already active.");
            }
        }
        public void Deactivate(Guid setID)
        {
            _activeIDs.Remove(setID);
            _core.RemoveSet(setID);
            _onSetUpdated?.Invoke(setID);
        }

        public bool IsActive(Guid id)
        {
            return _activeIDs.Contains(id);
        }

        public void Subscribe(Action<Guid> onSetUpdated)
        {
            _onSetUpdated += onSetUpdated;
        }
        public void Unsubscribe(Action<Guid> onSetUpdated)
        {
            _onSetUpdated -= onSetUpdated;
        }

        public void OnRespawn(List<ModuleSet> startingModuleSets)
        {
            _setsByID.Clear();
            _activeIDs.Clear();
            _onSetUpdated = null;
            foreach (ModuleSet moduleSet in startingModuleSets)
            {
                AddItem(moduleSet, Guid.NewGuid());
            }
        }

        public ModuleSet GetSet(Guid id)
        {
            _setsByID.TryGetValue(id, out ModuleSet set);
            return set;
        }
        public Dictionary<Guid, ModuleSet> GetSets()
        {
            return _setsByID;
        }




    }

    public interface ISetInventory
    {
        public ModuleSet GetSet(Guid id);
        public Dictionary<Guid, ModuleSet> GetSets();
        public void AddItem(ModuleSet set, Guid setID);
        public void RemoveItem(Guid id);
        public void Activate(Guid setID);
        public void Deactivate(Guid setID);
    }

}