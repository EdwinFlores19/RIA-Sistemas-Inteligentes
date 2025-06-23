using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeSelector_UI : DataSelector_UI
{
    public override void AssignDataToDataRow(int option)
    {
        datarow.SetShape((ItemShape)option);
    }

    protected override void NotifySelecting()
    {
        datarow.NotifySelectingOn(0);
    }
}
