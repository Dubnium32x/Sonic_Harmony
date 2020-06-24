using UnityEngine;

public class CreateExplosions : MonoBehaviour {
    public Vector2 maxVariance;
    public float timePerExplosion = 0.5F;
    float timer = 0;

    void Update() {
        timer -= Utils.cappedDeltaTime;
        if (!(timer <= 0)) return;
        Instantiate(
            Constants.Get<GameObject>("prefabExplosionBoss"),
            transform.position + new Vector3(
                maxVariance.x * ((Random.value * 2) - 1),
                maxVariance.y * ((Random.value * 2) - 1),
                -0.2F
            ),
            Quaternion.identity
        );
        timer = timePerExplosion;
    }
}