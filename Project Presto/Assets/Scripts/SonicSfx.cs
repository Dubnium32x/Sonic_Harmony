using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SonicSfx : MonoBehaviour
{

    public AudioSource S1_A0;
    public CharacterCTRL properties;
    List<AudioClip> chosenSounds = new List<AudioClip>();
    private bool CurrentlyActive = false;
    void Start()
    {
        var mysounds = Resources.LoadAll<AudioClip>("");

        var jump = mysounds.First(Result => Result.name.ToLower().Contains("jump"));

        var ring_ding = mysounds.First(Result => Result.name.ToLower().Contains("ring_ding"));

        var ring_loss = mysounds.First(Result => Result.name.ToLower().Contains("ring_loss"));
        var death = mysounds.First(Result => Result.name.ToLower().Contains("death"));
        var peel = mysounds.First(Result => Result.name.ToLower().Contains("peel"));
        var spindash = mysounds.First(Result => Result.name.ToLower().Contains("spindash"));
        chosenSounds.Add(jump);
        chosenSounds.Add(ring_ding);
        chosenSounds.Add(ring_loss);
        chosenSounds.Add(death);
        chosenSounds.Add(peel);
        chosenSounds.Add(spindash);
    }
    // Update is called once per frame
    void Update()
    {
        var mystate = properties.sonicState;
        switch (mystate)
        {
            case CharacterCTRL.SonicState.Normal:
                break;
            case CharacterCTRL.SonicState.Peel:
                if (!CurrentlyActive)
                {
                    CurrentlyActive = true;
                    PlaySound(chosenSounds[4]);
                }
                break;
            case CharacterCTRL.SonicState.Spindash:
                if (!CurrentlyActive)
                {
                    CurrentlyActive = true;
                    PlaySound(chosenSounds[5]);
                }
                break;
            case CharacterCTRL.SonicState.SpinningAir:
                break;
        }

        if (!properties.jumped && mystate == CharacterCTRL.SonicState.Normal && CurrentlyActive)
        {
            CurrentlyActive = false;
        }
        if (properties.jumped && !CurrentlyActive)
        {
            CurrentlyActive = true;
            PlaySound(chosenSounds[0]);
        }

        if (properties.RingGotB)
        {
            properties.RingGotB = false;
            PlaySound(chosenSounds[1]);
        }

        if (properties.GotHurtCheck && !properties.Death)
        {
            PlaySound(chosenSounds[2]);
        }

        if (properties.GotHurtCheck && properties.Death)
        {
            PlaySound(chosenSounds[3]);
        }
    }

    void PlaySound(AudioClip SoundFile)
    {
        if (!S1_A0.isPlaying)
        {
            S1_A0.PlayOneShot(SoundFile);
        }
    }
}
