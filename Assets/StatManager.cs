using UnityEngine;
using System.Collections.Generic;


public class StatManager : MonoBehaviour
{
    private int fishMultiplier;
    private int fishAmount;
    private int fishRate;
    private List<int> augmentIndiceList;
    private List<int> fishAmoutList; // Nombre de poisson pour chaque poisson différent
    private AugmentData augmentData;
    private FishData fishData;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        fishMultiplier = PlayerPrefs.GetInt("fishMultiplier", 1);
        fishAmount = PlayerPrefs.GetInt("fishAmount", 0);
        fishRate = PlayerPrefs.GetInt("fishRate", 0);
        augmentData = FindFirstObjectByType<AugmentData>();
        
        if (augmentData == null)
        {
            Debug.LogError("AugmentData n'a pas été trouvé dans la scène !");
            return;
        }
        
        augmentIndiceList = new List<int>();
        foreach (var item in augmentData.augments)
        {
            augmentIndiceList.Add(PlayerPrefs.GetInt("augment" + item.augmentName+"Indice", 0));

        }
        fishAmoutList = new List<int>();
        fishData = FindFirstObjectByType<FishData>();
        foreach (var fish in fishData.fishes)
        {
            fishAmoutList.Add(PlayerPrefs.GetInt(fish.name + "Amount", 0));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ResetStat()
    {
        Debug.Log("Reset");
        PlayerPrefs.SetInt("fishMultiplier", 1);
        PlayerPrefs.SetInt("fishAmount", 0);
        PlayerPrefs.SetInt("fishRate", 0);
        foreach (var item in augmentData.augments)
        {
            PlayerPrefs.SetInt("augment" + item.augmentName + "Indice", 0);
        }

        foreach (var fish in fishData.fishes)
        {
           PlayerPrefs.SetInt(fish.name + "Amount", 0);
        }
        FindFirstObjectByType<GameManager>().UpdateUI();

    }

    public int GetFishMultiplier()
    {
        return fishMultiplier;
    }

    public int GetFishAmount()
    {
        return fishAmount;
    }

    public int GetFishRate()
    {
        return fishRate;
    }

    public void SetFishRate(int fishRate)
    {
        this.fishRate = fishRate;
        PlayerPrefs.SetInt("fishRate", fishRate);
        PlayerPrefs.Save();

    }

    public int GetAugmentIndice(int augmentId)
    {
        if (augmentId >= augmentIndiceList.Count)
        {
            Debug.LogError("Augment id is out of range");
            return -1;
        }
        return augmentIndiceList[augmentId];
    }

    public void AddFishAmountByList(Fish fish)
    {
        int fishId=fishData.fishes.IndexOf(fish);
        if (fishId >= fishAmoutList.Count)
        {
            Debug.LogError("Fish id is out of range");
            return;
        }

        fishAmoutList[fishId]++;
        PlayerPrefs.SetInt(fish.name+"Amount", fishAmoutList[fishId]);    
    }

    public int GetFishAmountByList(int fishId)
    {
        if (fishId >= fishAmoutList.Count)
        {
            Debug.LogError("Fish id is out of range");
            return -1;
        }
        return fishAmoutList[fishId];
    }


    public void SetFishMultiplier(int fishMultiplier)
    {
        this.fishMultiplier = fishMultiplier;
        PlayerPrefs.SetInt("fishMultiplier", fishMultiplier);
        PlayerPrefs.Save();

    }

    public void SetFishAmount(int fishAmount)
    {
        this.fishAmount = fishAmount;
        PlayerPrefs.SetInt("fishAmount", fishAmount);
        PlayerPrefs.Save();
    }

    public void SetAugmentIndice(int augmentId,int augmentIndice)
    {
        augmentIndiceList[augmentId] = augmentIndice;
        PlayerPrefs.SetInt("augment"+ augmentData.augments[augmentId].augmentName + "Indice", augmentIndice);
        PlayerPrefs.Save();
    }
}
