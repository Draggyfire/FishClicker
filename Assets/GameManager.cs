using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;

public class GameManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject augmentPanel;
    public GameObject optionPanel;
    public GameObject collectionPanel;

    [Header("Manager")]
    public JaugeManager jaugeManager;
    public ShakeManager shakeManager;
    public AugmentManager augmentManager;

    [Header("GameUI")]  
    public TextMeshProUGUI txtNbFish;
    public TextMeshProUGUI txtMultiply;
    public GameObject popup;
    public GameObject ShakeImageGameObject;

    [Header("Animation")]
    public GameObject capturedFish;
    public Animator shipAnimator;
    public Animator fishCapturedAnimator;
    public AnimationClip fishCapturedAnimation;

    [Header("Data")]
    public RateConfig rateConfig;
    public FishData fishData;

    private bool fishHere = false;
    private int nbFish;
    private bool isFishing = false;

    private bool bossFishing = false;
    private int fishMultiplier;
    private StatManager statManager;
    private AugmentData augmentData;
    private float startDelay; // Entre start et stop second random pour la p�che
    private float stopDelay;
    private int fishRate;

    private bool showAnimNewFish = true;

    private const int FISH_BEFORE_BOSS = 10;
    private int actualFish = 0;

    private bool IsFishingBoss = false;
    private bool success = false;
    private bool waitForShake = false;
    private Fish currentFish;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startDelay = 3f;
        stopDelay = 5f;
        statManager = FindFirstObjectByType<StatManager>();

        if (statManager == null)
        {
            Debug.LogError("Stat Manager not found");
        }

        augmentData = FindFirstObjectByType<AugmentData>();

        if (augmentData == null)
        {
            Debug.LogError("Augment Data not found");
        }

        //augmentManager = FindFirstObjectByType<AugmentManager>();

        if (augmentManager == null)
        {
            Debug.LogError("Augment Manager not found");
        }

        if (shakeManager == null)
        {
            Debug.LogError("Shake Manager not found");
        }

        if (ShakeImageGameObject == null)
        {
            Debug.LogError("Shake Image Game Object Not Found");
        }

        nbFish = statManager.GetFishAmount(); // A changer pour enregistrer le nombre de poisson du joueur au d�marrage
        fishMultiplier = statManager.GetFishMultiplier();
        fishRate = statManager.GetFishRate();
        int i = 0;
        foreach (var item in augmentData.augments)
        {
            Debug.Log(item);
            augmentManager.SetCurrentAugment(statManager.GetAugmentIndice(i));
        }
        UpdateUI();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Click()
    {
        Debug.Log("Click");
        // Faire des verif si le joueur ne peche pas d�j�
        if (shipAnimator.GetCurrentAnimatorStateInfo(0).IsName("ShipIdleAnimation") && fishCapturedAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle") && !fishHere && !isFishing) // Si l'animation est l'idle et que le joueur ne peche pas d�j� et qu'il n'a pas de poisson (A changer parce que j'pense que y'a de la redondance)
        {
            isFishing = true;
            StartFishing();
        }
        else if (shipAnimator.GetCurrentAnimatorStateInfo(0).IsName("FishingIdleAnimation") && fishHere) // Si l'animation est l'idle en peche et qu'un poisson � �t� trouv�
        {

            isFishing = false;

            if (waitForShake)
            {
                return;
            }

            if ((!IsFishingBoss || !success) && actualFish == FISH_BEFORE_BOSS)
            {
                IsFishingBoss = true;
                jaugeManager.StartBossFight();
                success = true;

            }
            else if (IsFishingBoss)
            {
                success = jaugeManager.StopRotationAndCheck();
                IsFishingBoss = !success;
                if (success)
                {
                    waitForShake = true;
                    ShakeImageGameObject.SetActive(true);
                    //Attendre que le téléphone soit secoué
                    StartCoroutine(WaitForShakeAndStopFishing());
                }
                else
                {
                    StopFishing(success);
                }
            }
            else
            {
                if (actualFish == FISH_BEFORE_BOSS + 1)
                {
                    actualFish = 0;
                }
                StopFishing(true); // AJOUTER UNE VERIFICATION SI LE JOUEUR A REUSSI LA JAUGE SI OUI true si non false   
            }

        }
    }

    private void StartFishing()
    {
        shipAnimator.SetBool("StartFishing", true); // Active l'animation de descente de la canne
        // Mettre un temps al�atoire
        SetFish(GetRandomFish(GetRandomRarety()));

        StartCoroutine(WaitingFish());
        // Faire en sorte que si il n'y a pas de click apres un certain moment le poisson part (d�marrer un compteur et verif si il est d�pass� dans l'update ?)

    }

    private void SetFish(Fish current)
    {
        if (current == null)
        {
            Debug.Log("No Fish ??");
            return;
        }
        capturedFish.GetComponent<Image>().sprite = current.sprite;
        currentFish=current;
    }

    private void PlayCapturedFishAnimation(float duration)
    {
        float originalLength = fishCapturedAnimation.length;
        fishCapturedAnimator.speed = originalLength / duration;
        fishCapturedAnimator.Play(fishCapturedAnimation.name, 0, 0f);
    }

    IEnumerator WaitingFish()
    {
        float randomTime = Random.Range(startDelay - fishRate, stopDelay - fishRate); // Temps al�atoire entre 1 et 10 secondes
        PlayCapturedFishAnimation(randomTime);
        yield return new WaitForSeconds(randomTime); // Attendre le temps aléatoire
        popup.SetActive(true); // Activer l'objet
        fishHere = true;
        Debug.Log("Objet activ� apr�s " + randomTime + " secondes !");
    }

    private void StopFishing(bool byPlayer)
    {
        fishHere = false;
        popup.SetActive(false);
        shipAnimator.SetBool("StartFishing", false);
        WaitForAnimation(shipAnimator, "ThrowFishingRodAnimationReverse");
        if (byPlayer)
        {
            AddFish();
            actualFish++;
        }
        else
        {
            fishCapturedAnimator.Play("Idle");
        }
    }

    private void AddFish()
    {
        if (showAnimNewFish)
        {
            StartCoroutine(AddFishAfterAnimation());
        }
        else
        {
            AddFishImmediately();
        }
    }

    private void AddFishImmediately()
    {
        nbFish += fishMultiplier;
        statManager.SetFishAmount(nbFish);
        statManager.AddFishAmountByList(currentFish);
        fishCapturedAnimator.Play("Idle");
        UpdateUI();
    }

    private IEnumerator AddFishAfterAnimation()
    {
        fishCapturedAnimator.speed = 1;
        fishCapturedAnimator.Play("NewFishAnimation");

        yield return new WaitUntil(() =>
            fishCapturedAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle")
        );

        AddFishImmediately();
    }

    IEnumerator WaitForAnimation(Animator animator, string animationName)
    {
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName(animationName) &&
                                         animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1);
    }



    public void UpdateUI()
    {
        txtNbFish.SetText(FormatNumber(nbFish));
        txtMultiply.SetText(fishMultiplier.ToString());
        augmentManager.UpdateAugmentUI();
        Debug.Log("Update UI");
    }

    public void AugmentMultiply()
    {

        int cost = augmentManager.GetCurrentAugmentCost(0);
        if (cost > nbFish)
        {
            Debug.Log("Impossible d'augment : Pas assez de poissons " + nbFish.ToString());
            return; // Si le joueur n'a pas assez pour payer l'augment
        }
        Augment augment = augmentManager.GetNextAugment(0);
        nbFish -= augment.cost; // Payement
        fishMultiplier += augment.multiplier;
        statManager.SetFishMultiplier(fishMultiplier);
        statManager.SetAugmentIndice(0, augmentManager.GetCurrentAugment(0));
        Debug.Log("Update UI");
        UpdateUI();
    }

    public void AugmentRate()
    {

        int cost = augmentManager.GetCurrentAugmentCost(1);
        if (cost > nbFish)
        {
            Debug.Log("Impossible d'augment : Pas assez de poissons " + nbFish.ToString());
            return; // Si le joueur n'a pas assez pour payer l'augment
        }

        Augment augment = augmentManager.GetNextAugment(1);
        nbFish -= augment.cost; // Payement
        fishRate += augment.multiplier;
        statManager.SetFishRate(fishRate);
        statManager.SetAugmentIndice(1, augmentManager.GetCurrentAugment(1));
        Debug.Log("Update UI");
        UpdateUI();
    }

    public void OpenAugmentPanel()
    {
        if (augmentPanel.activeSelf)
        {
            augmentPanel.SetActive(false);
        }
        else
        {
            augmentPanel.SetActive(true);
        }
    }

    public void CloseAugmentPanel()
    {
        augmentPanel.SetActive(false);
        UpdateUI();
    }

    public void OpenOptionPanel()
    {
        if (optionPanel.activeSelf)
        {
            optionPanel.SetActive(false);
        }
        else
        {
            optionPanel.SetActive(true);
        }
    }

    public void CloseOptionPanel()
    {
        optionPanel.SetActive(false);
        UpdateUI();
    }

    public void OpenCollectionPanel()
    {
        if (collectionPanel.activeSelf)
        {
            collectionPanel.SetActive(false);
        }
        else
        {
            collectionPanel.SetActive(true);
        }
    }

    public void CloseCollectionPanel()
    {
        collectionPanel.SetActive(false);
        UpdateUI();
    }

    public static string FormatNumber(long number)
    {
        if (number >= 1_000_000_000)
            return (number / 1_000_000_000f).ToString("0.#") + "B";
        else if (number >= 1_000_000)
            return (number / 1_000_000f).ToString("0.#") + "M";
        else if (number >= 1_000)
            return (number / 1_000f).ToString("0.#") + "k";
        else
            return number.ToString();
    }


    // Coroutine qui attend la secousse avant de continuer
    private IEnumerator WaitForShakeAndStopFishing()
    {
        // Attente active pour détecter la secousse
        while (!shakeManager.IsShaking())
        {
            yield return null;  // Attendre la prochaine frame
        }

        ShakeImageGameObject.SetActive(false);
        waitForShake = false;

        // Une fois que la secousse est détectée, arrêter la pêche
        StopFishing(true);
    }

    public void ToggleFishAnime(bool toggle)
    {
        Debug.Log(toggle);
        showAnimNewFish = !toggle;
    }

    private RaretyEnum GetRandomRarety()
    {
        float rand = Random.Range(0f, 100f);
        float cumulative = 0f;

        foreach (var entry in rateConfig.entries)
        {
            cumulative += entry.rate;
            if (rand <= cumulative)
            {
                Debug.Log($"Poisson de rang {entry.type}");
                return entry.type;
            }
        }

        return RaretyEnum.R;
    }

    private Fish GetRandomFish(RaretyEnum rarety)
    {
        List<Fish> fishes = new List<Fish>();
        foreach (Fish fish in fishData.fishes)
        {
            if(fish.rarety==rarety) fishes.Add(fish);
        }

        if (fishes.Count <= 0)
        {
            Debug.Log("No Fish Found");
            return null;
        }
        int i = Random.Range(0, fishes.Count); // retourne un int → parfait pour l'index
                Debug.Log($"{i} à l'hameçon !");

        Debug.Log($"{fishes[i].name} à l'hameçon !");
        return fishes[i];
    }
}
