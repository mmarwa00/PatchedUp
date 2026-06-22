using UnityEngine;

public class AbilityHint : MonoBehaviour {
    [SerializeField] private string hintText = "Press F to use ability";
    [SerializeField] private float hintRange = 0.5f;
    [SerializeField] private string requiredAbility = "";

    public string HintText => hintText;
    public float HintRange => hintRange;
    public string RequiredAbility => requiredAbility;
}