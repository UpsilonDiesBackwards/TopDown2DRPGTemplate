using Core;
using UI;
using UnityEngine;

/*
 * Triggers a specified dialogue to open when the player enters a trigger volume.
 */

[RequireComponent(typeof(BoxCollider2D))]
public class DialogueTrigger : MonoBehaviour {
    public Dialogue dialogue;

    [SerializeField] private bool _autoPlay;
    public bool _triggered;

    [SerializeField] private bool deactivateAfterTrigger;

    [SerializeField] private float cooldownTime = 2.0f;
    private float _cooldownTimer;

    private BoxCollider2D _boxCollider;

    void Start() {
        _boxCollider = GetComponent<BoxCollider2D>();
    }

    void Update() {
        if (!Player.Instance.dead) {
            if (_cooldownTimer > 0) {
                _cooldownTimer -= Time.deltaTime;
                if (_cooldownTimer <= 0 && !deactivateAfterTrigger) {
                    _boxCollider.enabled = true;
                }
            }

            if ((_triggered || _autoPlay) && DialogueSystem.Instance != null) {
                DialogueSystem.Instance.OpenDialogue(dialogue);
                _autoPlay = false;

                _boxCollider.enabled = false;
                _cooldownTimer = cooldownTime;

                if (deactivateAfterTrigger) {
                    gameObject.SetActive(false);
                }
            }
        }
    }

    private void OnTriggerStay2D(Collider2D col) {
        if (col.gameObject == Player.Instance.gameObject) {
            if (!_triggered) {
                _triggered = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D col) {
        if (col.gameObject == Player.Instance.gameObject) {
            _triggered = false;
        }
    }
}
