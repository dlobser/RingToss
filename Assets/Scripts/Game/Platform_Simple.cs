using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform_Simple : Platform
{

    public override void SetSize(Vector3 scale)
    {
        this.transform.localScale = Vector3.one * scale.x;
        platformScale = scale;
    }

    public override void SetPosition(Vector3 position)
    {
        this.transform.localPosition = position;
        platformPosition = position;
    }

}
