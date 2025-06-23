using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveItem : Item
{
    public override void SetDestroyed(bool destroyed)
    {
        base.SetDestroyed(destroyed);
        if (destroyed) { LevelManager.instance.TrembleLevelVisualizer(); }
    }
}
