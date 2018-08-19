using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultPlayer : BasePlayer
{
    public override List<Vector3> VerticalMove { get { return new List<Vector3> { new Vector3(0, 0, 0), new Vector3(0, 30, 0), new Vector3(0, 0, 0) }; } }
}
