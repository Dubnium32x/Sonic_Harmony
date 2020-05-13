using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnhoverColorChange : MonoBehaviour
{
    // Start is called before the first frame 
    private Renderer textRenderer;
    void Start()
    {
        textRenderer = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnMouseEnter()
    {
        textRenderer.material.color = Color.red;
    }

    void OnMouseExit()
    {
        textRenderer.material.color = Color.black;
    }
}
