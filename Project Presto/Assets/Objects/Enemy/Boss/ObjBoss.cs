using UnityEngine;

public class ObjBoss : GameBehaviour {
    AudioSource audioSource;
    public CameraZone cameraZonePost;
    public CameraZone cameraZonePre;
    public int health = 8;
    public AudioClip introClip;

    float invulnTimer = 0;
    public AudioClip loopClip;
    public MusicManager.MusicStackEntry musicStackEntry;
    public int points = 8000;

    bool isInvulnerable => (
        (invulnTimer > 0) ||
        (health <= 0)
    );

    public override void Awake() {
        base.Awake();
        audioSource = GetComponent<AudioSource>();
        musicStackEntry = new MusicManager.MusicStackEntry {
            introClip = introClip,
            loopClip = loopClip,
            priority = 2
        };
        MusicManager.current.Add(musicStackEntry);
    }

    void OnTriggerStay(Collider other) {
        OnTriggerEnter(other);
    }

    void CharacterBounce(Character character) {
        Vector3 velocityTemp = character.velocity;
        velocityTemp *= -1;
        character.velocity = velocityTemp;
        character.GroundSpeedSync();
    }

    void OnTriggerEnter(Collider other) {
        if (isInvulnerable) return;

        Character[] characters = other.gameObject.GetComponentsInParent<Character>();
        if (characters.Length == 0) return;
        Character character = characters[0];

        if (character.isHarmful) {
            if (--health <= 0) Defeat(character);
            else Hurt(character);
        } else {
            character.Hurt(character.position.x <= transform.position.x);
            Laugh();
        }
    }

    public virtual void Laugh() { }

    public virtual void Defeat(Character sourceCharacter) {
        CharacterBounce(sourceCharacter);
        sourceCharacter.score += points;
    }

    public virtual void Hurt(Character sourceCharacter) {
        invulnTimer = 0.5F;
        CharacterBounce(sourceCharacter);
        SFX.Play(audioSource, "sfxBossHit");
    }

    public override void UpdateDelta(float modDeltaTime) {
        invulnTimer -= modDeltaTime;
    }
}