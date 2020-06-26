using UnityEngine;

public class FreedomUtils
{
    // https://forum.unity.com/threads/clean-est-way-to-find-nearest-object-of-many-c.44315/
    Transform FindClosestTargetMono(Transform origin, float distanceLimit = 24F)
    {
        Transform bestTarget = null;
        var closestDistanceSqr = Mathf.Infinity;
        foreach (var target in Object.FindObjectsOfType<MonoBehaviour>())
        {
            var potentialTarget = target.transform;
            if (!potentialTarget.gameObject.activeSelf) continue;
            if (!target.enabled) continue;

            var directionToTarget = potentialTarget.position - origin.position;
            var dSqrToTarget = directionToTarget.sqrMagnitude;
            if (!(dSqrToTarget < closestDistanceSqr)) continue;
            closestDistanceSqr = dSqrToTarget;
            bestTarget = potentialTarget;
        }

        if (closestDistanceSqr > distanceLimit) return null;
        return bestTarget;
    }
}