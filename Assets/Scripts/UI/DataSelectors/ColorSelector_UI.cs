using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorSelector_UI : DataSelector_UI
{
    public override void AssignDataToDataRow(int option)
    {
        datarow.SetColor((ItemColor)option);
    }

    protected override void NotifySelecting()
    {
        datarow.NotifySelectingOn(1);
    }
}