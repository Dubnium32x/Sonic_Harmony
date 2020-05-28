using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class OnEnterPause : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        #if UNITY_EDITOR
        EditorApplication.Step();
        #endif
    }
}
