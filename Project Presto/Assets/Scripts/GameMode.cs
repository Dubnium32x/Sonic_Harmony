using UnityEngine;
using UnityEngine.Audio;

public class GameMode : MonoBehaviour {
    public virtual void Awake() {
        Time.timeScale = 1;
        var mixer = Resources.Load<AudioMixer>("Main");
        mixer.SetFloat("Music Pitch", 1);
        mixer.SetFloat("Music Pitch Shift", 1);
        mixer.SetFloat("Music Volume", 0);
        mixer.SetFloat("SFX Volume", 0);
    }

    private const int physicsStepsPerFrame = 4;

    public virtual void Update() {
        Utils.SetFramerate();

        var deltaTime = Utils.cappedUnscaledDeltaTime;
        // Limit small fluctuations in deltatime
        deltaTime = Mathf.Round(deltaTime * Application.targetFrameRate) / Application.targetFrameRate;

        if (deltaTime <= 2F / Application.targetFrameRate)
            deltaTime = 1F / Application.targetFrameRate;

        InputCustom.preventRepeatLock = false;

        // We update the frame in a loop over the current delta time to allow
        // the game to catch up if it lags.
        while (deltaTime > 0) {
            var modDeltaTime = deltaTime > 1F / 60F ? 1F / 60F : deltaTime;

            // (meme)
            if (GlobalOptions.GetBool("gbaMode"))
                modDeltaTime += (Random.value * 0.032F) - 0.016F;
            
            foreach (var gameBehaviour in GameBehaviour.allGameBehvaiours) {
                float gbDeltaTime;
                if (!gameBehaviour.useUnscaledDeltaTime)
                {
                    gbDeltaTime = modDeltaTime * Time.timeScale;
                }
                else
                {
                    gbDeltaTime = modDeltaTime;
                }

                gameBehaviour.UpdateDelta(gbDeltaTime);
            }
            
            // Run multiple simulations per frame to avoid clipping.
            // Without this, the character sometimes clips into the ground slightly before popping out.
            for (var i = 0; i < physicsStepsPerFrame; i++)
                Physics.Simulate(modDeltaTime * Time.timeScale * (1F / physicsStepsPerFrame));

            foreach (var gameBehaviour in GameBehaviour.allGameBehvaiours) {
                float gbDeltaTime;
                if (gameBehaviour.useUnscaledDeltaTime)
                {
                    gbDeltaTime = modDeltaTime;
                }
                else
                {
                    gbDeltaTime = modDeltaTime * Time.timeScale;
                }

                gameBehaviour.LateUpdateDelta(gbDeltaTime);
            }

            // Since we're updating the character multiple times per frame, we
            // need to prevent Input.GetButtonDown from firing multiple times.
            InputCustom.preventRepeatLock = true;
            deltaTime -= modDeltaTime;
        }
    }
}