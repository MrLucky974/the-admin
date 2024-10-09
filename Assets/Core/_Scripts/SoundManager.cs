using LuckiusDev.Utils;
using System;
using System.Linq;
using UnityEngine;

public enum SoundType
{
    TAB_SELECT,
    ERROR,
    MODAL_CONFIRM,
    MODAL_CANCEL,
    ACTION_CONFIRM,
}

[RequireComponent(typeof(AudioSource)), ExecuteInEditMode]
public class SoundManager : Singleton<SoundManager>
{
    public const float MIN_PITCH = -3f;
    public const float MAX_PITCH = 3f;

    [SerializeField] private AudioSource m_audioSource;

    [Space]

    [SerializeField] private SoundList[] m_sounds;

    private void Start()
    {
        // If no audio source is referenced from the inspector and the component
        // cannot be found on the game object, add the component
        if (m_audioSource == null && TryGetComponent(out m_audioSource) is false)
        {
            m_audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public static void SetPitch(float pitch = 1f)
    {
        pitch = Mathf.Clamp(pitch, MIN_PITCH, MAX_PITCH);
        Instance.m_audioSource.pitch = pitch;
    }

    public static void PlaySound(SoundType type, float volume = 1f)
    {
        AudioClip[] clips = Instance.m_sounds[(int)type].Sounds;
        AudioClip randomClip = clips.PickRandom();
        Instance.m_audioSource.PlayOneShot(randomClip, volume);
        Instance.m_audioSource.pitch = 1f;
    }

    private void OnEnable()
    {
#if UNITY_EDITOR

        // Store all current sounds with their name reference
        var temp = new (string name, AudioClip[] sounds)[m_sounds.Length];
        int index = 0;
        foreach (var sound in m_sounds)
        {
            temp[index++] = (sound.m_name, sound.Sounds);
        }

        // Resize the list with the new amount of types
        string[] names = Enum.GetNames(typeof(SoundType));
        Array.Resize(ref m_sounds, names.Length);
        for (int i = 0; i < m_sounds.Length; i++)
        {
            m_sounds[i].m_name = names[i];
            m_sounds[i].Clear();

            // Check for a match in the sound type identifier,
            // and if true update with the correct sounds
            foreach (var tempList in temp)
            {
                if (tempList.name == names[i])
                {
                    foreach (var sound in tempList.sounds)
                    {
                        m_sounds[i].AddSound(sound);
                    }
                }
            }
        }

#endif
    }
}

[Serializable]
public struct SoundList
{
    [HideInInspector] public string m_name;
    [SerializeField] private AudioClip[] m_sounds;
    public AudioClip[] Sounds { get { return m_sounds; } }
    
    public void AddSound(AudioClip clip)
    {
        m_sounds = m_sounds.Concat(new AudioClip[] { clip }).ToArray();
    }

    public void Clear()
    {
        m_sounds = Array.Empty<AudioClip>();
    }
}