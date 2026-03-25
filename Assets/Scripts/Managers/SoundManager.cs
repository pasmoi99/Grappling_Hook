using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Sounds;

    [Header("Hook")]
    public AudioClip Latching;
    public AudioClip HittingClang;

    [Header("Rope")]
    public AudioClip RopeFiring;
    public AudioClip RopeRetracting;

    [Header("Character")]
    public List<AudioClip> Steps;
    public AudioClip Landing;

    private void Awake()
    {
        Sounds = this;
    }
}
