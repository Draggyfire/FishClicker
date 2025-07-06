using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Configs/RateConfig")]
[System.Serializable]
public class RateConfig : ScriptableObject
{
    public List<Rate> entries = new List<Rate>();
}