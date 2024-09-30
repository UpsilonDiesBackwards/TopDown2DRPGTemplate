using Core;
using Other;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

/*
 * This is the script that should be connected to a "Hurt Volume", a collider that will inflict a negative attribute
 * affect to the Player or an NPC.
 *
 * This is useful for an attack volume or kill volume.
 *
 * _attacksWhat: Attack Target
 * _attribute: Attribute that should be effected on the Attack Target (e.g Health)
 * _hitPower: Value amount that the attribute should be affected with.
 */

public class AttackHit : MonoBehaviour {
    [SerializeField] private AttacksWhat _attacksWhat;
    [SerializeField] private AttributeType _attribute;
    [SerializeField] private bool oneHitKill;
    [SerializeField] private float startCollisionDelay;
    [SerializeField] private GameObject parent;
    [SerializeField] private int _hitPower = 1;

    private void Start() {
        if (oneHitKill) _hitPower = 56709; // awooo!
    }

    public void OnTriggerStay2D(Collider2D col) {
        Vector2 attackDirection = (col.transform.position - parent.transform.position).normalized;

        if (oneHitKill) {
            col.GetComponent<RecoveryCounter>().counter = 0.1f;
            _hitPower                                    = 56709; // awooo!
        }
        
        if (_attacksWhat == AttacksWhat.Player || _attacksWhat == AttacksWhat.Everything) {
            
            if (col.GetComponent<Player>() != null) Player.Instance.GetHurt(attackDirection, _hitPower, _attribute);
            
        } else if (_attacksWhat == AttacksWhat.NPC || _attacksWhat == AttacksWhat.Everything) {
            
            if (col.GetComponent<NPC>() != null) {
                col.GetComponent<NPC>().GetHurt(attackDirection, _hitPower, _attribute); 
            }
            
        } else if (_attacksWhat == AttacksWhat.NPC || _attacksWhat == AttacksWhat.Everything) {
        }
    }

    private void OnValidate() {
        gameObject.name = _attacksWhat + _attribute.ToString() + $"HurtBox: ({_hitPower})";
    }

    private enum AttacksWhat {
        NPC,
        Player,
        Everything
    }
}