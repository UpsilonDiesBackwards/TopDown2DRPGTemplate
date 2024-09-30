using System.Collections;
using Core;
using Other;
using UnityEngine;

/*
 * Example of an Attribute Modifier that increases the players health over time when in range. 
 */

public class Campfire : MonoBehaviour {
    [Header("Configuration")] public float radius = 2.5f;

    public int healAmount = 1;

    private bool _isHealing;

    private void Update() {
        if (IsInRange() && !_isHealing) StartCoroutine(Heal());
    }

    private void OnDrawGizmos() {
        // Draw the radius as a gizmo in the Scene view
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    private bool IsInRange() {
        var distance = Vector2.Distance(transform.position, Player.Instance.transform.position);
        return distance <= radius;
    }

    private IEnumerator Heal() {
        _isHealing = true;

        while (IsInRange() && Player.Instance.health != Player.Instance.maxHealth) {
            Player.Instance.Heal(healAmount, AttributeType.Health);
            yield return new WaitForSeconds(Player.Instance.GetComponent<RecoveryCounter>().recoveryTime);
        }

        _isHealing = false;
    }
}