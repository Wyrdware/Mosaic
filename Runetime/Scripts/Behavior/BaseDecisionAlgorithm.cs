using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Mosaic
{
    public abstract class BaseDecisionAlgorithm : ScriptableObject
    {
        public abstract float CheckDecesion(ICore character);
    }
}