using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoSoundPlayer : SoundPlayer
{
    [SerializeField] private AudioClip[] clips;
    private List<int> indexList;

    private void OnEnable()
    {
        InitializeIndexList();
    }

    public void PlayRandomSound()
    {
        PlaySound(clips[GetRandomIndex()]);
    }

    private void InitializeIndexList()
    {
        indexList = new List<int>(clips.Length);
        for (int i = 0; i < clips.Length; i++) indexList.Add(i);
    }

    private int GetRandomIndex()
    {
        if (indexList.Count == 0) InitializeIndexList();

        int chosenSlot = Random.Range(0, indexList.Count);
        int slotValue = indexList[chosenSlot];
        indexList.RemoveAt(chosenSlot);

        return slotValue;
    }

    public override void PlaySound(int index)
    {
        PlaySound(clips[index]);
    }

    public override void PlayOnLoop(int index)
    {
        PlayClipOnLoop(clips[index]);
    }
}
