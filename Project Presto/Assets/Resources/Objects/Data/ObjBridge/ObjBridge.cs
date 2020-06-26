using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjBridge : MonoBehaviour {
    // ========================================================================
    // CONSTANTS
    // ========================================================================

    const float maxDepression = -0.5F;
    const int depressSpeed = 2;

    // ========================================================================
    // PRIVATE VARIABLES
    // ========================================================================

    float acrossBridgeAmtCurrent = 0;
    public Character characterCurrent;

    // ========================================================================
    // OBJECT AND COMPONENT REFERENCES
    // ========================================================================

    Transform links;

    float timer = 0;

    // ========================================================================

    Character character { get {
        for (var i = 0; i < links.childCount; i++) {
            var child = links.GetChild(i);
            var detector = child.GetComponent<CharacterGroundedDetector>();
            if (detector == null) continue;
            foreach (var character in detector.characters.Where(character => character != null))
            {
                return character;
            }
        }
        return null;
    }}

    float? acrossBridgeAmt { get {
        if (characterCurrent == null) return null;

        var bounds = new Bounds();
        var colliders = links.GetComponentsInChildren<Collider>();
        foreach (var col in colliders)
        {
            bounds.Encapsulate(col.bounds);
        }

        var bridgeBoundsRightWorld = bounds.max.x;
        var bridgeWidth = 2F * (bridgeBoundsRightWorld - transform.position.x);
        var bridgeBoundsLeftWorld = bridgeBoundsRightWorld - bridgeWidth;

        var amt = (
            characterCurrent.position.x -
            bridgeBoundsLeftWorld
        ) / bridgeWidth;

        if ((amt > 1) || (amt < 0)) return null;
        return amt;
    }}

    void InitReferences() {
        links = transform.Find("Links");
    }

    void Start() {
        InitReferences();
    }

    // ========================================================================

    void Update() {
        characterCurrent = character;

        if (characterCurrent != null) {
            timer += depressSpeed * Utils.deltaTimeScale;
            timer = Mathf.Min(90, timer);
            var acrossBridgeAmtTemp = acrossBridgeAmt;
            if (acrossBridgeAmt != null && acrossBridgeAmtTemp != null)
            {
                acrossBridgeAmtCurrent = (float) acrossBridgeAmtTemp;
            }
        } else if (timer > 0) {
            timer -= depressSpeed * Utils.deltaTimeScale;
            timer = Mathf.Max(0, timer);
        }

        // Set the position of each log
        for (var i = 0; i < links.childCount; i++) {
            var child = links.GetChild(i);
            var childPosition = child.position;

            if (timer == 0) {
                childPosition.y = transform.position.y;
            } else {
                var logPosAmt = i / (float)links.childCount;
                var dipAmt = 1 - Mathf.Abs(logPosAmt - (float)acrossBridgeAmtCurrent);
                var dipLimiter = Mathf.Sin(Mathf.PI * logPosAmt);
                var timerLimiter = Mathf.Sin(Mathf.Deg2Rad * timer);

                childPosition.y = transform.position.y + (dipAmt * dipLimiter * maxDepression * timerLimiter);
            }

            child.position = childPosition;
        }
    }
}