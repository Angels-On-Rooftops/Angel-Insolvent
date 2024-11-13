using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.RoomSystem;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using static MIDI2EventUnity;
using static UnityEngine.InputSystem.InputAction;

public class AudioSystem : MonoBehaviour
{
    [SerializeField]
    AudioMixer Mixer;

    [SerializeField]
    List<MIDI2EventUnity> Songs;

    [SerializeField]
    MIDI2EventUnity StartingSong;

    [SerializeField]
    float fadeTime = 1;

    [SerializeField]
    float fadedDB = -40;

    static MIDI2EventUnity current;
    public static AudioSystem Instance { get; private set; }

    Maid SongPlayingMaid = new();

    [SerializeField]
    InputAction CycleSong;

    public MIDI2EventUnity CurrentSystem
    {
        get => current;
    }

    public static int CurrentIndex
    {
        get => Instance.Songs.IndexOf(current);
    }

    private void Awake()
    {
        Debug.Assert(Instance is null, "Can only have one instance of AudioSystem!");

        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        current = StartingSong;
    }

    private void OnEnable()
    {
        SongPlayingMaid.GiveEvent(
            RoomSystem.Instance,
            "RoomChanged",
            (Room newRoom) =>
            {
                Songs.Clear();

                Array.ForEach(
                    FindObjectsOfType<MIDI2EventUnity>(),
                    (MIDI2EventUnity midi2event) =>
                    {
                        Songs.Add(midi2event);
                    }
                );
                // TODO cache midi tracks and add them to the song list, then change the current song

                Debug.Log(string.Join(",", Songs));

                // HERE FOR TIMEBOX 6, REMOVE THIS LATER
                if (newRoom == Room.Cantata)
                {
                    SwitchToSong(
                        Songs.FindIndex(
                            0,
                            Songs.Count,
                            (MIDI2EventUnity m2e) => m2e.gameObject.name == "LofiMIDI2Event"
                        )
                    );
                }
            }
        );

        SongPlayingMaid.GiveEvent(
            CycleSong,
            "performed",
            (CallbackContext _) =>
            {
                SwitchToSong((CurrentIndex + 1) % Songs.Count);
            }
        );

        CycleSong.Enable();
    }

    private void OnDisable()
    {
        SongPlayingMaid.Cleanup();
    }

    public static void SwitchToSong(int index)
    {
        if (index >= 0 && index < Instance.Songs.Count)
        {
            Instance.StartCoroutine(Instance.FadeToNext(current, Instance.Songs[index]));
        }
        else
        {
            Debug.Log("Song index out of bounds!");
        }
    }

    public static void FadeOut()
    {
        Instance.FadeToNext(current, null);
    }

    IEnumerator FadeToNext(MIDI2EventUnity previous, MIDI2EventUnity next)
    {
        //Debug.Log(previous.VolumeSliderName);
        if (previous == null)
        {
            StartNextIfRelevant(next);
            yield break;
        }

        float elapsedTime = 0;
        Mixer.GetFloat(previous.VolumeSliderName, out float startingVol);
        while (elapsedTime < fadeTime)
        {
            float volStep = Mathf.Lerp(startingVol, fadedDB, elapsedTime / fadeTime);
            Mixer.SetFloat(previous.VolumeSliderName, volStep);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        previous.Stop();
        StartNextIfRelevant(next);
        Mixer.SetFloat(previous.VolumeSliderName, startingVol);
    }

    void StartNextIfRelevant(MIDI2EventUnity toStart)
    {
        if (toStart != null)
        {
            toStart.Restart();
        }
        current = toStart;
    }
}
