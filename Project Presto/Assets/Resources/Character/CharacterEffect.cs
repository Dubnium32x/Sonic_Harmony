using UnityEngine;

public class CharacterEffect {
    bool _destroyed = false;
    public Character character;
    public float duration;
    public string name;

    public CharacterEffect(Character character, string name, float duration = Mathf.Infinity) {
        this.character = character;
        this.name = name;
        this.duration = duration;
    }

    public void UpdateBase(float deltaTime) {
        this.duration -= deltaTime;
        if (duration <= 0) {
            DestroyBase();
            return;
        }
        Update(deltaTime);
    }

    public virtual void Update(float deltaTime) { }

    public void DestroyBase() {
        if (_destroyed) return;
        Destroy();
        _destroyed = true;
        character.effects.Remove(this);
    }

    public virtual void Destroy() { }

    public override string ToString() => name;

    ~CharacterEffect() {
        DestroyBase(); // Failsafe
    }
}