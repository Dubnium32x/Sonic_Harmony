using UnityEngine;

[AddComponentMenu("Freedom Engine/Objects/Checkpoint")]
[RequireComponent(typeof(Collider))]
public class Checkpoint : MonoBehaviour
{
    public AudioClip activateClip;
    public Animator animator;
    public bool SwitchZ = false;
    private new AudioSource audio;
    public Transform startPoint;

    private bool triggered;

    private void Start()
    {
        if (!TryGetComponent(out audio))
        {
            audio = gameObject.AddComponent<AudioSource>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggered || !other.CompareTag("Player")) return;
        triggered = true;
        animator.SetBool("Activated", true);
        audio.PlayOneShot(activateClip, 0.5f);
        if (SwitchZ)
        {
            StageManager.Instance.SetStartState(startPoint.position, Quaternion.identity, ScoreManager.Instance.time);
        }
        else
        {
            var fixedpoint = startPoint.position;
            fixedpoint.z = other.transform.position.z;
            StageManager.Instance.SetStartState(fixedpoint, Quaternion.identity, ScoreManager.Instance.time);
        }
        //LevelSettings.Instance.SetRespawnPoint(startPoint.position,other.transform.rotation);
    }
}
