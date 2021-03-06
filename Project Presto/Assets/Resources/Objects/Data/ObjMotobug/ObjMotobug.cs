﻿using UnityEngine;

public class ObjMotobug : MonoBehaviour {
    // ========================================================================

    const float speed = 0.03125F;
    Animator animator;

    Vector3 positionPrev = Vector3.zero;
    // ========================================================================
    // OBJECT AND COMPONENT REFERENCES
    // ========================================================================

    SpriteRenderer spriteRenderer;
    float turnTimer = 0;

    // ========================================================================

    public bool moveRight {
        get { return spriteRenderer.flipX; }
        set { spriteRenderer.flipX = value; }
    }

    // ========================================================================

    int direction => moveRight ? 1 : -1;

    void InitReferences() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    void Start() {
        InitReferences();
        spriteRenderer.flipX = moveRight;
    }

    // ========================================================================

    void Update() {
        if (turnTimer > 0) {
            turnTimer -= Utils.cappedDeltaTime;
            if (turnTimer > 0) return;
            turnTimer = 0;
            moveRight = !moveRight;
            transform.position = positionPrev;
            animator.enabled = true;
            return;
        }

        RaycastHit hit;
        Physics.Raycast(
            transform.position, // origin
            Vector3.down, // direction,
            out hit,
            transform.localScale.y, // max distance
            ~Utils.IgnoreRaycastMask
        );
        
        Vector3 newPos = transform.position;

        if (hit.collider == null) {
            turnTimer = 1F;
            animator.enabled = false;
            return;
        }

        newPos.x += direction * speed * Utils.deltaTimeScale;
        newPos.y = hit.point.y + (transform.localScale.y / 2F);
        positionPrev = transform.position;
        transform.position = newPos;
    }
}
