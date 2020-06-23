using UnityEngine;
using System.Collections;

public class RenderTextureCamera : MonoBehaviour {
    public Rect screenRectRelative = new Rect(0,0,1,1);
    Rect screenRectRelativePrev;
    Rect screenRect;
    bool integerScaling;

    public Rect GetScreenRect() {
        if (!integerScaling)
            return new Rect(
                screenRectRelative.x * Screen.width,
                screenRectRelative.y * Screen.height,
                screenRectRelative.width * Screen.width,
                screenRectRelative.height * Screen.height
            );
        var ratio = (
            (screenRectRelative.width * Screen.width) /
            (screenRectRelative.height * Screen.height)
        );
        var intScaleFac = (int)((screenRectRelative.height * Screen.height) / camera.targetTexture.height);

        var height = camera.targetTexture.height * intScaleFac;
        var width = (int)(height * ratio);
        var xOffset = (Screen.width - width) / 2;
        var yOffset = (Screen.height - height) / 2;

        return new Rect(
            (screenRectRelative.x * Screen.width) +
            (screenRectRelative.width * xOffset),
            (screenRectRelative.y * Screen.height) +
            (screenRectRelative.height * yOffset),
            screenRectRelative.width * width,
            screenRectRelative.height * height
        );

    }

    new Camera camera;
    void Awake() {
        camera = GetComponent<Camera>();
    }

    void Update() {
        integerScaling = GlobalOptions.GetBool("integerScaling");

        if (LevelManager.current != null)
            integerScaling &= LevelManager.current.characters.Count <= 1;
    }

    int renderTextureWidthPev; // Hack

    public void ResizeRenderTexture() {
        if (camera.targetTexture == null) return;
        Rect screenRectNew = GetScreenRect();
        if (
            (screenRectNew == screenRect) &&
            (renderTextureWidthPev == camera.targetTexture.width)
        ) return;
        screenRect = screenRectNew;

        camera.targetTexture.Release();
        camera.targetTexture.width = (int)Mathf.Round(
            (float)camera.targetTexture.height * (
                ((float)screenRect.width) /
                ((float)screenRect.height)
            )
        );
        renderTextureWidthPev = camera.targetTexture.width;

        var viewportRect = new Rect(
            0,0,
            camera.targetTexture.width,
            camera.targetTexture.height
        );
        camera.rect = viewportRect;
    }

    static bool clearScreen = true;
    void OnPreRender() {
        ResizeRenderTexture();
        clearScreen = true;
    }

    void OnGUI() { }
    
    static readonly WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();
    IEnumerator OnPostRender() {
        yield return waitForEndOfFrame;
        if (clearScreen) {
            GL.Clear(false, true, Color.black);
            clearScreen = false;
        }
        Graphics.DrawTexture(screenRect, camera.targetTexture);
    }
}