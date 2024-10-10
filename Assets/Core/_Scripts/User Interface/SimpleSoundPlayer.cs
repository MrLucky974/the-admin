using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleSoundPlayer : MonoBehaviour
{
    [SerializeField] private SoundType m_soundType;

    public void Play(float volume = 1f)
    {
        SoundManager.PlaySound(m_soundType, volume);
    }
}
