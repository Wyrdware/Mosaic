using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ModularCharacter
{
    public enum DataTagEventType { Added, Updated, Removed }
    public class DataTagRepository : IDataTagRepository, IDataTagUpdateEventTrigger
    {
        private Dictionary<Type, DataTag> _dataTags = new Dictionary<Type, DataTag>();
        DynamicEventDispatcher addedEventDispatcher = new();
        DynamicEventDispatcher updatedEventDispatcher = new();
        DynamicEventDispatcher removedEventDispatcher = new();

        /// <summary>
        /// Add or update a tag of type T. If the tag already exists it will be replaced with the new tag.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tag"></param>
        public void AddOrUpdateTag<T>(DataTag tag) where T : DataTag
        {
            if (_dataTags.ContainsKey(typeof(T)))
            {
                _dataTags[typeof(T)].SetHandler(null);
                _dataTags[typeof(T)] = tag;
                _dataTags[typeof(T)].SetHandler(this);
                updatedEventDispatcher.TriggerEvent<T>(tag);// The DataTag of type T has just been deleted and replaced with a new tag
            }
            else
            {
                _dataTags.Add(typeof(T), tag);
                tag.SetHandler(this);
                addedEventDispatcher.TriggerEvent<T>(tag);
            }
        }

        /// <summary>
        /// Gets the tag of type T, if the tag does not exist it will create a new tag and return it.
        /// </summary>
        /// <typeparam name="T"> Type of DataTag being requested. </typeparam>
        /// <returns></returns>
        public T GetTag<T>() where T : DataTag, new()
        {
            if (_dataTags.TryGetValue(typeof(T), out DataTag tag))
            {
                return tag as T;
            }
            else//There is no way to guarentee a DataTag will never be null, so instead of performing a null check we automaticaly create a new tag. If you want to check if a tag exists use IsTagged<T>()
            {
                Debug.Log(typeof(T) + " was not found, Creating new tag!");
                T newTag = new();
                _dataTags.Add(typeof(T), newTag);
                newTag.SetHandler(this);
                addedEventDispatcher.TriggerEvent<T>(newTag);
                return newTag;
            }
        }

        public bool RemoveTag<T>() where T : DataTag
        {
            if (_dataTags.ContainsKey(typeof(T)))
            {
                _dataTags[typeof(T)].SetHandler(null);
                _dataTags.Remove(typeof(T));
                removedEventDispatcher.TriggerEvent<T>(null);
                return true;
            }
            else 
            { 
                return false; 
            }
        }

        /// <summary>
        /// Checks wether the tag has been created
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public bool IsTagged<T>() where T : DataTag
        {
            return _dataTags.ContainsKey(typeof(T));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">The data tag to subscribe to</typeparam>
        /// <param name="eventType">Specific event related tot he data tag</param>
        /// <param name="handler"> The specified event</param>
        public void Subscribe<T>(DataTagEventType eventType, Action<DataTag> handler)
        {
            switch (eventType)
            {
                case DataTagEventType.Added:
                    addedEventDispatcher.Subscribe<T>(handler);
                    break;
                case DataTagEventType.Updated:
                    updatedEventDispatcher.Subscribe<T>(handler);
                    break;
                case DataTagEventType.Removed:
                    removedEventDispatcher.Subscribe<T>(handler);
                    break;
            }
        }

        public void Unsubscribe<T>(DataTagEventType eventType, Action<DataTag> handler)
        {
            switch (eventType)
            {
                case DataTagEventType.Added:
                    addedEventDispatcher.Unsubscribe<T>(handler);
                    break;
                case DataTagEventType.Updated:
                    updatedEventDispatcher.Unsubscribe<T>(handler);
                    break;
                case DataTagEventType.Removed:
                    removedEventDispatcher.Unsubscribe<T>(handler);
                    break;
            }
        }

        public void TriggerUpdateEvent<T>(DataTag tag) where T : DataTag
        {
            updatedEventDispatcher.TriggerEvent<T>(tag);
        }

        private class DynamicEventDispatcher
        {
            private Dictionary<Type, Action<DataTag>> _eventTable = new Dictionary<Type, Action<DataTag>>();

            public void Subscribe<T>(Action<DataTag> handler)
            {
                if (handler == null)
                    throw new ArgumentNullException(nameof(handler));

                if (!_eventTable.ContainsKey(typeof(T)))
                {
                    _eventTable[typeof(T)] = null;
                }
                _eventTable[typeof(T)] += handler;
            }

            public void Unsubscribe<T>(Action< DataTag> handler)
            {

                if (handler == null)
                    throw new ArgumentNullException(nameof(handler));

                if (_eventTable.ContainsKey(typeof(T)))
                {
                    _eventTable[typeof(T)] -= handler;
                }
            }

            public void TriggerEvent<T>(DataTag dataTag)//dataTag may be null, so <T> is essential
            {
                if (_eventTable.TryGetValue(typeof(T), out var handlers))
                {
                    handlers?.Invoke(dataTag); // The DataTag, passed as a parameter, can be null to represent it is not present.
                }
            }
        }
    }
    public interface IDataTagRepository
    {
        public void AddOrUpdateTag<T>(DataTag tag) where T : DataTag;
        public T GetTag<T>() where T : DataTag, new();
        public bool RemoveTag<T>() where T : DataTag;
        public bool IsTagged<T>() where T : DataTag;
        public void Subscribe<T>(DataTagEventType eventType, Action<DataTag> handler);
        public void Unsubscribe<T>(DataTagEventType eventType, Action<DataTag> handler);
    }
    public interface IDataTagUpdateEventTrigger
    {
        public void TriggerUpdateEvent<T>(DataTag tag) where T : DataTag;
    }
}