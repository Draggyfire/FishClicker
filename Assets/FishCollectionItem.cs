using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FishCollectionItem : MonoBehaviour
{
    private Fish fish;
    public Image fishImage;
    public Image rarety;
    public TextMeshProUGUI fishName;
    public TextMeshProUGUI fishDescription;
    private bool isFound;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetFish(Fish fish,bool isFound)
    {
        this.fish = fish;
        this.isFound = isFound;
        UpdateUI();
    }

    public void UpdateUI()
    {
        fishImage.sprite = fish.sprite;
        rarety.sprite = Resources.Load<Sprite>("Textures/Rarety/"+ fish.rarety.ToString());
        if (!isFound)
        {
            fishName.text = "???";
            fishDescription.text = "???";
            fishImage.color = Color.black;
        }
        else
        {
            fishImage.color = Color.white;
            fishName.text = fish.name;
            fishDescription.text = fish.description;
        }
    }
}
