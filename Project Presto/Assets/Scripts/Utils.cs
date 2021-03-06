using System.Collections.Generic;
using System.Collections;
using System.Text;
using System;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using Object = UnityEngine.Object;

public static class Utils {
    // ========================================================================

    public enum AxisType {
        X, // Distance to object only calculated on x axis
        Y, // ... y axis
        XY // ... both axis
    }

    // ========================================================================

    public enum DistanceType {
        Camera, // Distance to object originates from screen position
        Character, // ... player position
        Closest // ... whichever is closest
    }

    public const float physicsScale = 1.875F; // 60 (framerate) / 32 (pixels per unit)

    // Start hack because fuck unity; "SceneManager.sceneLoaded -=" DOESN'T. FUCKING. WORK.
    // I'm not even kidding. try it yourself. fuck this shitty engine.
    static bool _LoadLevelAsyncEverUsed = false;
    static Level _LoadLevelAsyncLevel;

    public static LayerMask? _IgnoreRaycastMask = null;

    // ========================================================================

    // Store all case variants since str.ToLower() causes GC
    static readonly HashSet<string> stringsPositive = new HashSet<string> {
        "true", "True", "TRUE",
        "on",   "On",   "ON",
        "yes",  "Yes",  "YES",
        "y",    "Y",
        "t",    "T",
        "ok",   "Ok",   "OK"
    };

    // ========================================================================

    static readonly Dictionary<int, string> intStrCache = new Dictionary<int, string>();
    public static float deltaTimeScale => 60F * Utils.cappedDeltaTime;

    public static LayerMask IgnoreRaycastMask {
        get {
            if (_IgnoreRaycastMask != null) return (LayerMask)_IgnoreRaycastMask;
            _IgnoreRaycastMask = LayerMask.GetMask(
                "Ignore Raycast",
                "Player - Ignore Top Solid and Raycast",
                "Player - Ignore Top Solid",
                "Player - Rolling",
                "Player - Rolling and Ignore Top Solid",
                "Object - Ignore Other Objects",
                "Object - Top Solid Only and Ignore Other Objects",
                "Object - Monitor Solidity",
                "Object - Monitor Trigger"
            );
            return (LayerMask)_IgnoreRaycastMask;
        }
    }

    public static float cappedUnscaledDeltaTime { get {
        var deltaTime = Time.unscaledDeltaTime;
        if (deltaTime > Time.maximumDeltaTime)
            return 1F / Application.targetFrameRate;
        return deltaTime;
    }}

    public static float cappedDeltaTime { get {
        var deltaTime = Time.deltaTime;
        if (Time.deltaTime > Time.maximumDeltaTime)
            return 1F / Application.targetFrameRate;
        return deltaTime;
    }}

    // ========================================================================
    public static Character CheckIfCharacterInRange(
        Vector2 thisPos,
        float triggerDistance,
        AxisType axisType,
        DistanceType distanceType,
        HashSet<Character> characters
    ) {
        foreach(var character in characters) {
            Vector2 cameraPos;
            if (character.characterCamera != null)
                cameraPos = character.characterCamera.position;
            else
                cameraPos = character.position;
            
            Vector2 charPos = character.position;

            var cameraDist = Mathf.Infinity;
            var charDist = Mathf.Infinity;

            switch(axisType) {
                case AxisType.XY:
                    cameraDist = Vector2.Distance(thisPos, cameraPos);
                    charDist = Vector2.Distance(thisPos, charPos);
                    break;
                case AxisType.X:
                    cameraDist = Mathf.Abs(thisPos.x - cameraPos.x);
                    charDist = Mathf.Abs(thisPos.x - charPos.x);
                    break;
                case AxisType.Y:
                    cameraDist = Mathf.Abs(thisPos.y - cameraPos.y);
                    charDist = Mathf.Abs(thisPos.y - charPos.y);
                    break;
            }

            var otherDist = Mathf.Infinity;

            switch(distanceType) {
                case DistanceType.Character:
                    otherDist = charDist;
                    break;
                case DistanceType.Camera:
                    otherDist = cameraDist;
                    break;
                case DistanceType.Closest:
                    otherDist = Mathf.Min(cameraDist, charDist);
                    break;
            }

            if (otherDist <= triggerDistance) return character;
        }

        return null;
    }

    static void _LoadLevelAsyncOnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode) {
        foreach (var level in Object.FindObjectsOfType<Level>()) {
            if (level.gameObject.scene != scene) continue;
            _LoadLevelAsyncLevel = level;
            return;
        }
    }
    // End hack, mostly

    public static IEnumerator LoadLevelAsync(string scenePath, Action<Level> callback = null, bool ignoreDuplicates = false) {
        if (!_LoadLevelAsyncEverUsed) {
            SceneManager.sceneLoaded += _LoadLevelAsyncOnSceneLoaded;
            _LoadLevelAsyncEverUsed = true;
        }

        Scene nextLevelScene = SceneManager.GetSceneByPath(scenePath);

        if (ignoreDuplicates || !nextLevelScene.IsValid()) { // If scene isn't already loaded
            var asyncLoad = SceneManager.LoadSceneAsync(
                scenePath,
                LoadSceneMode.Additive
            );
            asyncLoad.allowSceneActivation = true;

            while (!asyncLoad.isDone) yield return null;
            if (callback == null) yield break;
            callback(_LoadLevelAsyncLevel);
            _LoadLevelAsyncLevel = null;
        } else {
            if (callback == null) yield break;
            foreach (var level in Object.FindObjectsOfType<Level>()) {
                if (level.gameObject.scene != nextLevelScene) continue;
                callback(level);
                break;
            }
        }
    }

    public static void SetFramerate() {
        Application.targetFrameRate = Screen.currentResolution.refreshRate;
        Time.fixedDeltaTime = 1F / Application.targetFrameRate;
        Time.maximumDeltaTime = 1F / 10F;
    }

    public static Tuple<int, int> CalculateFauxTransparencyFrameCount(float alpha) {
        // Tuple format is (off frames, on frames)
        if (alpha == 0) return Tuple.Create(int.MaxValue, 0);
        if (alpha == 1) return Tuple.Create(0, int.MaxValue);

        var onFramesDivisor = alpha * 2;
        var offFramesDivisor = 1 - onFramesDivisor;

        return Tuple.Create(
           (int)(1 / onFramesDivisor),
           (int)(1 / offFramesDivisor)
        );
    }

    public static bool StringBool(string str) => stringsPositive.Contains(str);

    public static string IntToStrCached(int val) {
        if (!intStrCache.ContainsKey(val))
            intStrCache[val] = val.ToString();
        return intStrCache[val];
    }

    public static string IntToStrCached(float val) => IntToStrCached((int)val);

    // ========================================================================

    public static void SetActiveScene(string sceneName) => 
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));

    public static void SetScene(Transform transform, Scene scene) {
        var sceneCurrent = SceneManager.GetActiveScene();
        SceneManager.SetActiveScene(scene);
        transform.SetParent(null);
        SceneManager.SetActiveScene(sceneCurrent);
    }

    public static void SetScene(Transform transform, string sceneName) =>
        SetScene(transform, SceneManager.GetSceneByName(sceneName));
}