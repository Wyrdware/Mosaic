using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "BehaviorInputType", menuName = "CharacterModule/BehaviorInputType", order = 1)]
public class BehaviorInputType : ScriptableObject
{
    //TODO: find a solution to bind this to the input system so that the developer does not need to manually enter the controls
    [Tooltip("The controls the player uses to activate this input.")]
    [TextArea(3, 10)]
    [SerializeField]
    private string _playerControls;
    [Tooltip("The conditions under which the NPCs AI will activate this input.")]
    [TextArea(3, 10)]
    [SerializeField]
    private string _npcControls;
    public string PlayerControls => _playerControls;
    public string NPCControls => _npcControls;
}
