using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Rate
{
    public RaretyEnum type;
    [Range(0f, 100f)]
    public float rate;
}