using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Mosaic
{
    public class ModifierHandler
    {
        private readonly ICore _core;
        //Create a wrapper class for all modifiers, this will handle the process along with adding and removing decorators.
        private readonly Dictionary<Type, List<Guid>> _processByType= new();// using a diction
        private readonly Dictionary<Type, List<Guid>> _decoratorsByType = new();

        //We track each instance of a modifier so that we can easily remove the specific one that was added.
        private readonly Dictionary<Guid, (ModifierProcess,Guid)> _processByID = new();
        private readonly Dictionary<Guid, (ModifierDecorator,Guid)> _decoratorByID= new();

        //grouping modifiers into sets allows us to manage groups of shared modifiers.
        private readonly Dictionary<Guid, List<Guid>> _processIDsBySetID = new();
        private readonly Dictionary<Guid, List<Guid>> _decoratorIDsBySetID = new();

        private Action<Modifier> _onAddMod;
        private Action<Modifier> _onRemoveMod;
        private Action<ModifierDecorator> _onAddDecorator;
        private Action<ModifierDecorator> _onRemoveDecorator;

        public ModifierHandler(ICore core, List<Modifier> modifiers, List<ModifierDecorator> decorators)
        {
            _core = core;

            foreach(ModifierDecorator decorator in decorators)
            {
                AddModifierDecorator(decorator, Guid.Empty);
            }
            foreach(Modifier modifier in modifiers)
            {
                AddModifier(modifier, _core, Guid.Empty);
            }
        }
        public void OnRespawn(List<Modifier> modifiers, List<ModifierDecorator> decorators)
        {
            //remove all modifiers first to avoid reordering the lists
            List<Guid> processIDs = new(_processByID.Keys);
            foreach (Guid id in processIDs)
            {
                RemoveModifier(id);
            }
            //remove all decorators
            List<Guid> decoratorIDs = new(_decoratorByID.Keys);
            foreach (Guid id in decoratorIDs)
            {
                RemoveModifierDecorator(id);
            }
            
            //adding decorators first is more efficient because the list won't need to be reordered
            foreach (ModifierDecorator decorator in decorators)
            {
                AddModifierDecorator(decorator, Guid.Empty);
            }
            foreach (Modifier modifier in modifiers)
            {
                AddModifier(modifier, _core, Guid.Empty);
            }
        }
        public void ClearAllProcessies()
        {
            while (_processByID.Count > 0)
            {
                _processByID.First().Value.Item1.Clear();
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
        public Guid AddModifierDecorator(ModifierDecorator decorator, Guid setID)
        {
            //create unique id for the modifier
            Guid id = Guid.NewGuid();


            // store the unique id and the modifier in a dictionary
            _decoratorByID.Add(id, (decorator,setID));

            _decoratorsByType.TryAdd(decorator.GetComponentType(), new List<Guid>());
            _decoratorsByType[decorator.GetComponentType()].Add(id);

            _decoratorIDsBySetID.TryAdd(setID, new List<Guid>());
            _decoratorIDsBySetID[setID].Add(id);

            if (_processByType.ContainsKey(decorator.GetComponentType()))
            {
                foreach (Guid processID in _processByType[decorator.GetComponentType()])
                {
                    _processByID[processID].Item1.AddDecorator(decorator, id, setID);
                }
            }
            

            _onAddDecorator?.Invoke(decorator);  
            return id;
        }
        public void AddModifierDecorator(List<ModifierDecorator> decorators, Guid setID)
        {
            foreach(ModifierDecorator decorator in decorators)
            {
                AddModifierDecorator(decorators, setID);
            }
        }
        public void RemoveModifierDecorator(Guid id)
        {
            Guid setID = _decoratorByID[id].Item2;
            _decoratorIDsBySetID[setID].Remove(id);

            ModifierDecorator decorator = _decoratorByID[id].Item1;
            
            //Remove decorator from all modifiers it applies to
            if (_processByType.TryGetValue(decorator.GetComponentType(), out List<Guid> processIDs))
            {
                foreach (Guid processID in processIDs)
                {
                    _processByID[processID].Item1.RemoveDecorator(id);

                }
            }
            _decoratorByID.Remove(id);
            _decoratorsByType[decorator.GetComponentType()].Remove(id);


            _onRemoveDecorator?.Invoke(decorator);
        }

        public Guid AddModifier( Modifier modifier, ICore origin, Guid setID)
        {
            Guid id = Guid.NewGuid();
            Type modifierType = modifier.GetType();
            _processIDsBySetID.TryAdd(setID, new List<Guid>());
            _processIDsBySetID[setID].Add(id);
            //Get a list of all modifier decorators
            // Decorator, ID, SetID
            List<(ModifierDecorator, Guid, Guid)> idDecorator = new(); 
            if(_decoratorsByType.TryGetValue(modifierType, out List<Guid> decorators))
            {
                foreach (Guid decoratorID in decorators)
                {
                    idDecorator.Add((_decoratorByID[decoratorID].Item1, decoratorID, _decoratorByID[decoratorID].Item2));
                }
            }

            //Create the new process
            ModifierProcess newProcess = new ModifierProcess(modifier, idDecorator, _core, origin, id, setID);//Create an instane of the modifier to be manipulated

            _processByType.TryAdd(modifierType, new List<Guid>());
            _processByType[modifierType].Add(id);
            _processByID.Add(id, (newProcess, setID));

            //Putting this in the handler instead of the modifier process itself so the entire lifecycle is handled in a single location
            newProcess.SubscribeToEnd(() => 
            { 
                _processByType[modifierType].Remove(id);
                _processByID.Remove(id);
            
            } );//When a modifier is removed, there is no need to worry about the decorators since they are part of the object
            
            // Now that it's set up, start the modifier
            newProcess.StartModifier();
            _onAddMod?.Invoke(modifier);
            return id;

        }
        public void AddModifier(List<Modifier> modifiers, ICore origin, Guid setID)
        {
            foreach (Modifier modifier in modifiers)
            {
                AddModifier(modifier, origin, setID);
            }
        }
        public void RemoveModifier(Guid id)
        {
            if (_processByID.ContainsKey(id))
            {
                Guid setID = _processByID[id].Item2;

                ModifierProcess process = _processByID[id].Item1;
                Modifier modifier = process.GetModifier() as Modifier;
                _processIDsBySetID[setID].Remove(id);
                process.Clear();


                _onRemoveMod?.Invoke(modifier);
            }
        }
        public void RemoveSet(Guid setID)
        {
            if(_processIDsBySetID.ContainsKey(setID))
            {
                List<Guid> processIDs = new(_processIDsBySetID[setID]);
                foreach (Guid id in processIDs)
                {
                    RemoveModifier(id);
                }
            }
            if (_decoratorIDsBySetID.ContainsKey(setID))
            {
                List<Guid> decoratorIDs = new( _decoratorIDsBySetID[setID]);
                foreach (Guid id in decoratorIDs)
                {
                    RemoveModifierDecorator(id);
                }
            }
        }

        public void SubscribeToModAdded(Action<Modifier> onModAdded)
        {
            _onAddMod += onModAdded;
        }
        public void UnsubscribeToModAdded(Action<Modifier> onModAdded)
        {
            _onAddMod -= onModAdded;
        }
        public void SubscribeToModRemoved(Action<Modifier> onModRemoved)
        {
            _onRemoveMod += onModRemoved;
        }
        public void UnsubscribeToModRemoved(Action<Modifier> onModRemoved)
        {
            _onRemoveMod -= onModRemoved;
        }
        public void SubscribeToDecoratorAdded(Action<ModifierDecorator> onDecoratorAdded)
        {
            _onAddDecorator += onDecoratorAdded;
        }
        public void UnsubscribeToDecoratorAdded(Action<ModifierDecorator> onDecoratorAdded)
        {
            _onAddDecorator -= onDecoratorAdded;
        }
        public void SubscribeToDecoratorRemoved(Action<ModifierDecorator> onDecoratorRemoved)
        {
            _onRemoveDecorator += onDecoratorRemoved;
        }
        public void UnsubscribeToDecoratorRemoved(Action<ModifierDecorator> onDecoratorRemoved)
        {
            _onRemoveDecorator -= onDecoratorRemoved;
        }

    }
}