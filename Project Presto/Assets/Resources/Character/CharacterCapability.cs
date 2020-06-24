using UnityEngine;

public class CharacterCapability {
    public Character character;
    public string name;
    public Transform transform;

    public CharacterCapability(Character character) {
        this.character = character;
        this.transform = character.transform;
        Init();
    }

    public virtual void Init() { }
    public virtual void StateInit(string stateName, string prevStateName) { }
    public virtual void StateDeinit(string stateName, string nextStateName) { }
    public virtual void Update(float deltaTime) { }
    public virtual void OnCollisionEnter(Collision collision) { }
    public virtual void OnCollisionStay(Collision collision) { }
    public virtual void OnTriggerExit(Collider other) { }
    public virtual void OnTriggerEnter(Collider other) { }
    public virtual void OnTriggerStay(Collider other) { }
    public virtual void OnCollisionExit(Collision collision) { }
}