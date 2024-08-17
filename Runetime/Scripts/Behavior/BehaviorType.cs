using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "BehaviorType", menuName = "CharacterModule/BehaviorType", order = 1)]
public class BehaviorType : ScriptableObject
{
    [TextArea(3, 10)]
    [SerializeField]
    private string _description;
    public string Description => _description;
}
