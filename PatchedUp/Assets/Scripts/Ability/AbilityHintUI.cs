using UnityEngine;
using TMPro;

public class AbilityHintUI : MonoBehaviour {
    [SerializeField] private TMP_Text hintLabel;

    private Transform _player;
    private PlayerAbilityManager _abilityManager;

    private void Start() {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) _player = player.transform;

        if (App.Instance != null)
            _abilityManager = App.Instance.GetManager<PlayerAbilityManager>();

        hintLabel.gameObject.SetActive(false);
    }

    private void Update() {
        if (_player == null) return;

        if (_abilityManager == null && App.Instance != null)
            _abilityManager = App.Instance.GetManager<PlayerAbilityManager>();

        AbilityHint[] hints = FindObjectsByType<AbilityHint>(FindObjectsSortMode.None);
        AbilityHint closest = null;
        float closestDist = float.MaxValue;

        foreach (var hint in hints) {
            float dist = Vector3.Distance(_player.position, hint.transform.position);
            if (dist > hint.HintRange || dist >= closestDist) continue;

            // Prüfen ob richtige Ability ausgerüstet ist
            if (!string.IsNullOrEmpty(hint.RequiredAbility)) {
                if (_abilityManager == null) continue;
                if (_abilityManager.GetCurrentAbilityName() != hint.RequiredAbility) continue;
            }

            closestDist = dist;
            closest = hint;
        }

        if (closest != null) {
            hintLabel.text = closest.HintText;
            hintLabel.gameObject.SetActive(true);
        }
        else {
            hintLabel.gameObject.SetActive(false);
        }
    }
}