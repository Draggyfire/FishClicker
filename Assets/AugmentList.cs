
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

[System.Serializable]

public class AugmentList
{
    public string augmentName;
    public List<Augment> fishAugment = new List<Augment>();
    public TextMeshProUGUI AugmentLvl;
    public TextMeshProUGUI AugmentCost;
    public TextMeshProUGUI AugmentValue;
    public Button augmentButton;

    public bool CheckCurrentFishAugment(int currentFishAugment)
    {
        return (fishAugment.Count > currentFishAugment);
    }

    public void UpdateFishAugmentUI(int currentFishAugment)
    {
        if (!CheckCurrentFishAugment(currentFishAugment))
        {
           AugmentLvl.SetText("MAX");
            AugmentCost.SetText("");
            AugmentValue.SetText("");
            augmentButton.interactable = false;
            return;
        }
        Augment currentAugment = fishAugment[currentFishAugment];
        AugmentCost.SetText(currentAugment.cost.ToString());
        AugmentValue.SetText(currentAugment.multiplier.ToString());
        AugmentLvl.SetText((currentFishAugment + 1).ToString());
    }
}
