using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

public class CollectionScript : MonoBehaviour
{
    public Transform scrollViewContent; 
    public GameObject fishItemPrefab;
    private List<Fish> fishes;
    private StatManager statManager;
    private void OnEnable()
    {
        fishes = FindFirstObjectByType<FishData>().fishes;
        statManager = FindFirstObjectByType<StatManager>();
        int i = 0;
        foreach (var item in fishes)
        {
            GameObject fishItem = Instantiate(fishItemPrefab);
            fishItem.GetComponent<FishCollectionItem>().SetFish(item, statManager.GetFishAmountByList(i) > 0);
            fishItem.transform.SetParent(scrollViewContent, false);
            i++;
        }
    }
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnDisable()
    {
        foreach (Transform child in scrollViewContent)
        {
            Destroy(child.gameObject);
        }
    }
}
