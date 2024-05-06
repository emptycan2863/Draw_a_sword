using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuageHandCardOver : CardObject
{
    protected override void OnDragOverEvent() {
        UICardViewer.SetCardViewer(linkCardData);
    }
}
