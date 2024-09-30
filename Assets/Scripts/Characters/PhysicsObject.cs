using System.Collections.Generic;
using UnityEngine;

/*
 * This handles the Physics-side of an object (Player, NPC, etc).
 *
 * It is /// HIGHLY /// recommended that this is left unmodified as this can result in dodgy behaviour in all that inherit it!
 */

public class PhysicsObject : MonoBehaviour {
    [Header("Physics"), SerializeField]
     public float moveSpeed = 5.0f;
    [HideInInspector] public Vector2 targetVel;
    protected Rigidbody2D rb;
    [SerializeField] protected ContactFilter2D contactFilter;
    [SerializeField] protected RaycastHit2D[] hitBuffer = new RaycastHit2D[16];
    [SerializeField] protected List<RaycastHit2D> hitBufferList = new List<RaycastHit2D>(16);
    protected const float minMoveDistance = 0.001f;
    protected const float shellRadius = 0.01f;

    private void OnEnable() {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Start() {
        contactFilter.useTriggers = false;
        contactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
        contactFilter.useLayerMask = true;
    }

    public void Update() {
        targetVel = Vector2.zero;
    }
    
    public void FixedUpdate() {
        Vector2 deltaPos = targetVel * UnityEngine.Time.deltaTime;

        Movement(deltaPos);
    }

    void Movement(Vector2 move) {
        float distance = move.magnitude;
        
        if (distance > minMoveDistance) {
            int count = rb.Cast(move, contactFilter, hitBuffer, distance + shellRadius);
            hitBufferList.Clear();

            for (int i = 0; i < count; i++) {
                hitBufferList.Add(hitBuffer[i]);
            }

            for (int i = 0; i < hitBufferList.Count; i++) {
                Vector2 currentNormal = hitBufferList[i].normal;
                float projection = Vector2.Dot(targetVel, currentNormal);
                if (projection < 0) {
                    targetVel = targetVel - projection * currentNormal;
                }

                float modifiedDistance = hitBuffer[i].distance - shellRadius;
                distance = modifiedDistance < distance ? modifiedDistance : distance;
            }
        }
        rb.position = rb.position + move.normalized * distance;
    }
}
