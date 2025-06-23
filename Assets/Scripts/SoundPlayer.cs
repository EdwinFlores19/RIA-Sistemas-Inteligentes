using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SoundPlayer : MonoBehaviour
{
    [SerializeField] protected AudioSource audioSource;
    private bool autoDestructionInitiated = false;

    public bool IsPlaying() { return audioSource.isPlaying; }
    public bool IsOnLoop() { return audioSource.loop; }
    public void PlaySound(AudioClip audioClip)
    {
        audioSource.loop = false;
        audioSource.pitch = UnityEngine.Random.Range(0.91f, 1.09f);
        audioSource.PlayOneShot(audioClip);
    }

    protected void PlayClipOnLoop(AudioClip AudioClip)
    {
        audioSource.loop = true;
        audioSource.pitch = UnityEngine.Random.Range(0.91f, 1.09f);
        audioSource.clip = AudioClip;
    }

    public void PlaySoundNoRandomPitch(AudioClip audioClip)
    {
        audioSource.loop = false;
        audioSource.pitch = 1f;
        audioSource.PlayOneShot(audioClip);
    }

    public abstract void PlaySound(int index);
    public abstract void PlayOnLoop(int index);

    public void Stop()
    {
        audioSource.loop = false;
        audioSource.Stop();
    }
}
