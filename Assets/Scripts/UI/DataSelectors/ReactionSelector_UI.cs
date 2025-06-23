using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReactionSelector_UI : DataSelector_UI
{
    public override void AssignDataToDataRow(int option)
    {
        datarow.SetReaction((ItemReaction)option);
    }

    protected override void NotifySelecting()
    {
        datarow.NotifySelectingOn(2);
    }
}
