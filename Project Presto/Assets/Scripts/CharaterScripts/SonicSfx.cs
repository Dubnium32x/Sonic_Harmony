using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SonicSfx : MonoBehaviour
{

    public AudioSource SonicJumpSound;
    public CharacterCTRL properties;
    List<AudioClip> chosenSounds = new List<AudioClip>();
    private bool CurrentlyActive = false;

    // Save previous state to check state changes
    CharacterCTRL.SonicState prevState;

    void Start()
    {
        var mysounds = Resources.LoadAll<AudioClip>("");

        var jump = mysounds.First(Result => Result.name.ToLower().Contains("jump"));

        var ring_ding = mysounds.First(Result => Result.name.ToLower().Contains("ring_ding"));

        var ring_loss = mysounds.First(Result => Result.name.ToLower().Contains("ring_loss"));
        var death = mysounds.First(Result => Result.name.ToLower().Contains("death"));
        var peel = mysounds.First(Result => Result.name.ToLower().Contains("peelout_charge_remake"));
        var peel_launch = mysounds.First(Result => Result.name.ToLower().Contains("peelout_launch_remake"));
        var spindash = mysounds.First(Result => Result.name.ToLower().Contains("spindash"));
        chosenSounds.Add(jump);
        chosenSounds.Add(ring_ding);
        chosenSounds.Add(ring_loss);
        chosenSounds.Add(death);
        chosenSounds.Add(peel);
        chosenSounds.Add(peel_launch);
        chosenSounds.Add(spindash);
    }
    // Update is called once per frame
    void Update()
    {
        var mystate = properties.sonicState;
        switch (mystate)
        {
            case CharacterCTRL.SonicState.Normal:
                // Released peel charge
                if (prevState == CharacterCTRL.SonicState.ChargingPeel)
                    PlayOnce(chosenSounds[5], mystate);
                break;
            case CharacterCTRL.SonicState.ChargingPeel:
                PlayOnce(chosenSounds[4], mystate);
                break;
            case CharacterCTRL.SonicState.Peel:
                // Released peel charge
                if (prevState == CharacterCTRL.SonicState.ChargingPeel)
                    PlayOnce(chosenSounds[5], mystate);
                break;
            case CharacterCTRL.SonicState.ChargingSpin:
                if (Input.GetButtonDown("Jump"))
                {
                    PlaySound(chosenSounds[6]);
                }
                break;
            case CharacterCTRL.SonicState.Spindash:
                if (!CurrentlyActive)
                {
                    CurrentlyActive = true;
                    PlaySound(chosenSounds[6]);
                }
                break;
            case CharacterCTRL.SonicState.SpinningAir:
                break;
        }

        // Reset the state for one-time SFX
        if (mystate == CharacterCTRL.SonicState.Normal)
            prevState = mystate;

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
        SonicJumpSound.PlayOneShot(SoundFile);
    }

    // Plays the sound only once per state Sonic is in
    void PlayOnce(AudioClip SoundFile, CharacterCTRL.SonicState currentState)
    {
        if (prevState != currentState)
        {
            prevState = currentState;
            SonicJumpSound.PlayOneShot(SoundFile);
        }
    }
}
