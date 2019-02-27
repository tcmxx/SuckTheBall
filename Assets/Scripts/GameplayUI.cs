using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameplayUI : MonoBehaviour
{
    public static GameplayUI Instance { get; private set; }
    public Text player0ScoreTextRef;
    public Text player1ScoreTextRef;
    public RectTransform abilityPanelTransformRef;

    private void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        player0ScoreTextRef.text = GamePlayController.Instance.Player0Info.score.ToString();
        player1ScoreTextRef.text = GamePlayController.Instance.Player1Info.score.ToString();
    }

    public bool IsInsideAbilityPanel(Vector2 pointerPosition)
    {
        return RectTransformUtility.RectangleContainsScreenPoint(abilityPanelTransformRef, pointerPosition);
    }
}
