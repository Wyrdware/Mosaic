using Mosaic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Mosaic
{
    public interface IInspectorDataTag
    {
        public void AddTagToCore(Core core);
    }
    public abstract class InspectorDataTag<T> : MonoBehaviour,IInspectorDataTag where T : DataTag
    {
        [SerializeField]
        protected T _data;
        public void AddTagToCore(Core core)
        {
            core.DataTags.AddOrUpdateTag<T>(_data);
        }


    }
}
