using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "BehaviorInputType", menuName = "CharacterModule/BehaviorInputType", order = 1)]
public class BehaviorInputType : ScriptableObject
{
    [TextArea(3, 10)]
    [SerializeField]
    private string _description;
    public string Description => _description;
}
