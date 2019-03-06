using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class GameplayUI : MonoBehaviour
{
    public static GameplayUI Instance { get; private set; }
    public Text player0ScoreTextRef;
    public Text player1ScoreTextRef;
    public Text player0NameTextRef;
    public Text player1NameTextRef;
    public RectTransform abilityPanelTransformRef;
    public RectTransform abilityListTransformRef;
    public RectTransform cardSpawnPositionRef;
    public GameObject manaBarRef;
    public int maxAbilities = 3;
    public GameObject abilityButtonPref;

    public List<AbilityButton> CurrentAbilities { get; private set; } = new List<AbilityButton>();
    protected Image[] manaBarImages;
    

    private void Awake()
    {
        Instance = this;

        manaBarImages = manaBarRef.GetComponentsInChildren<Image>();
    }
    // Start is called before the first frame update
    void Start()
    {
        var playerList = PhotonNetwork.PlayerList;
        if(playerList.Length > 0)
            player0NameTextRef.text = playerList[0].NickName;
        if (playerList.Length > 1)
            player1NameTextRef.text = playerList[1].NickName;
        
        for(int i = 0; i < maxAbilities; ++i)
        {
            AddAbilityButton();
        }
    }

    // Update is called once per frame
    void Update()
    {
        player0ScoreTextRef.text = GamePlayController.Instance.Player0Info.score.ToString();
        player1ScoreTextRef.text = GamePlayController.Instance.Player1Info.score.ToString();

        UpdateManabar();
    }

    public bool IsInsideAbilityPanel(Vector2 pointerPosition)
    {
        return RectTransformUtility.RectangleContainsScreenPoint(abilityPanelTransformRef, pointerPosition);
    }

    public void AddAbilityButton()
    {
        var obj = Instantiate(abilityButtonPref);
        AbilityButton button = obj.GetComponentInChildren<AbilityButton>();
        
        obj.transform.SetParent(abilityListTransformRef);
        LayoutRebuilder.ForceRebuildLayoutImmediate(abilityListTransformRef);
        
        button.transform.position = cardSpawnPositionRef.position;
        button.transform.parent.localScale = Vector3.one;
        button.Initialized(GamePlayController.Instance.NextAbiliityInfo());
        
        CurrentAbilities.Add(button);

        foreach(var b in CurrentAbilities)
        {
            b.LerpPosition();
        }

        
    }

    protected void UpdateManabar()
    {
        float mana = GamePlayController.Instance.GetLocalPlayerInfo().mana;
        for (int i = 0; i < manaBarImages.Length; ++i)
        {
            if(i <= mana - 1)
            {
                manaBarImages[i].fillAmount = 1;
            }else if(i == (int)mana)
            {
                manaBarImages[i].fillAmount = mana - (int)mana;
            }
            else
            {
                manaBarImages[i].fillAmount = 0;
            }
            
        }
    }

    //called by the AbilityButton when ability is used
    public void OnAbilityUsed(AbilityButton button)
    {
        CurrentAbilities.Remove(button);
        AddAbilityButton();
        SetOtherButtonsForceUnusable(button, false);
    }

    public void SetOtherButtonsForceUnusable(AbilityButton excludeButton, bool unusable)
    {
        foreach(var b in CurrentAbilities)
        {
            if(b != excludeButton)
            {
                b.ForceUnusable = unusable;
            }
        }
    }
}
