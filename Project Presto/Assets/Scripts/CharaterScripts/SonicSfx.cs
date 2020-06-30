using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SonicSfx : MonoBehaviour
{

    public AudioSource SonicJumpSound;
    public AudioSource SonicRingLossSound;
    public CharControlMotor properties;
    List<AudioClip> chosenSounds = new List<AudioClip>();
    private bool CurrentlyActive = false;

    // Save previous state to check state changes
    CharControlMotor.SonicState prevState;
    public AudioClip jump;
    public AudioClip death;
    public AudioClip ring_loss;
    public AudioClip ring_ding;
    public AudioClip peel;
    public AudioClip spindash;
    public AudioClip spindash_charge;
    public AudioClip peel_launch;
    public AudioClip brake;
    void Start()
    {
        var mysounds = Resources.LoadAll<AudioClip>("");

        
        jump = mysounds.First(Result => Result.name.ToLower().Contains("jump"));

        ring_ding = mysounds.First(Result => Result.name.ToLower().Contains("ring_ding"));


        ring_loss = mysounds.First(Result => Result.name.ToLower().Contains("ring_loss"));

        death = mysounds.First(Result => Result.name.ToLower().Contains("death"));
        
        peel = mysounds.First(Result => Result.name.ToLower().Contains("peelout_charge_remake"));

        peel_launch = mysounds.First(Result => Result.name.ToLower().Contains("peelout_launch_remake"));

        spindash_charge = mysounds.First(Result => Result.name.ToLower().Contains("spindash"));

        spindash = mysounds.First(Result => Result.name.ToLower().Contains("spin_remake"));
        chosenSounds.Add(jump);
        chosenSounds.Add(ring_ding);
        chosenSounds.Add(ring_loss);
        chosenSounds.Add(death);
        chosenSounds.Add(peel);
        chosenSounds.Add(peel_launch);
        chosenSounds.Add(spindash_charge);
        chosenSounds.Add(spindash);
        chosenSounds.Add(brake);
    }


    // Update is called once per frame
    void Update2()
    {
        var mystate = properties.sonicState;
        switch (mystate)
        {
            case CharControlMotor.SonicState.Normal:
                // Released peel charge
                if (prevState == CharControlMotor.SonicState.ChargingPeel)
                    PlayOnce(chosenSounds[5], mystate);
                break;
            case CharControlMotor.SonicState.ChargingPeel:
                PlayOnce(chosenSounds[4], mystate);
                break;
            case CharControlMotor.SonicState.Peel:
                // Released peel charge
                if (prevState == CharControlMotor.SonicState.ChargingPeel)
                    PlayOnce(chosenSounds[5], mystate);
                break;
            case CharControlMotor.SonicState.ChargingSpin:
                if (Input.GetButtonDown("Jump"))
                {
                    PlaySound(chosenSounds[6]);
                }
                break;
            case CharControlMotor.SonicState.Spindash:
                if (!CurrentlyActive)
                {
                    CurrentlyActive = true;
                    PlaySound(chosenSounds[7]);
                }
                break;
            case CharControlMotor.SonicState.SpinningAir:
                break;
            case CharControlMotor.SonicState.Brake:
                PlayOnce(chosenSounds[8], mystate);
                break;
        }

        // Reset the state for one-time SFX
        if (mystate == CharControlMotor.SonicState.Normal)
            prevState = mystate;

        if (!properties.jumped && mystate == CharControlMotor.SonicState.Normal && CurrentlyActive)
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
            // Ring Loss sound is separate from Sonic's sounds
            SonicRingLossSound.Play();
            //PlaySound(chosenSounds[2]);
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
    void PlayOnce(AudioClip SoundFile, CharControlMotor.SonicState currentState)
    {
        if (prevState == currentState) return;
        prevState = currentState;
        SonicJumpSound.PlayOneShot(SoundFile);
    }
}
