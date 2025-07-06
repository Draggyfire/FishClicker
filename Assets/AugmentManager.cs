using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AugmentManager : MonoBehaviour
{
    AugmentData augmentData;
    private List<int> currentAugment = new List<int>();

    private void Awake()
    {
        augmentData = FindFirstObjectByType<AugmentData>();
        if (augmentData == null)
        {
            Debug.LogError("AugmentData n'a pas été trouvé dans la scène !");
            return;
        }
        Debug.Log("AugmentManager Started");
        UpdateAugmentUI();
    }

    public int GetCurrentAugment(int augmentId)
    {
        return currentAugment[augmentId];
    }

    public void SetCurrentAugment(int currentFishAugment)
    {
        currentAugment.Add(currentFishAugment);
    }

    public void UpdateAugmentUI()
    {
        int count = Mathf.Min(augmentData.augments.Count, currentAugment.Count);
        for (int i = 0; i < count; i++)
        {
            augmentData.augments[i].UpdateFishAugmentUI(currentAugment[i]);
        }
    }

    public int GetCurrentAugmentCost(int augmentId)
    {
        return augmentData.augments[augmentId].fishAugment[currentAugment[augmentId]].cost;
    }

    public Augment GetNextAugment(int augmentId)
    {
        Augment nextAugment = augmentData.augments[augmentId].fishAugment[currentAugment[augmentId]];
        currentAugment[augmentId]++;
        return nextAugment;
    }

}
