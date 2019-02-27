using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
[RequireComponent(typeof(EventTrigger))]
public class AbilityButton : MonoBehaviour
{
    public AbilityInfo AbilityInfo { get; private set; }
    [SerializeField]
    protected GameObject graphicsRef;
    [SerializeField]
    protected GameObject confirmButtonsRef;
    [SerializeField]
    protected GameObject rotationButtonsRef;
    [SerializeField]
    protected CanvasGroup canvasGroup;

    [SerializeField]
    protected Image iconRef;
    [SerializeField]
    protected Text costRef;



    protected enum Status
    {
        Default,
        Moving,
        Placing,
        Placed,
        Confirmed,
        Rotating
    }
    protected Status status = Status.Default;
    protected Image imageRef;

    //record some posiiton/rotation related status
    protected Vector2 rotStartVector;
    protected float rotStartRotation;
    protected Vector2 centerPositionCache;
    protected Vector2 lastFramePosition;

    //the object to show when putting ability
    protected AbilityObject abilityMarker;

    private Camera mainCamera;

    public bool Usable { get; private set; } = true;
    public bool ForceUnusable { get; set; } = false;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    public void Initialized(AbilityInfo info)
    {
        AbilityInfo = info;
        lastFramePosition = transform.position;

        imageRef = GetComponent<Image>();

        //intiialize the triggeres
        var trigger = GetComponent<EventTrigger>();

        EventTrigger.Entry onBeginDrag = new EventTrigger.Entry();
        onBeginDrag.eventID = EventTriggerType.BeginDrag;
        onBeginDrag.callback.AddListener(OnBeginDrag);
        trigger.triggers.Add(onBeginDrag);

        EventTrigger.Entry onDrag = new EventTrigger.Entry();
        onDrag.eventID = EventTriggerType.Drag;
        onDrag.callback.AddListener(OnDrag);
        trigger.triggers.Add(onDrag);

        EventTrigger.Entry onEndDrag = new EventTrigger.Entry();
        onEndDrag.eventID = EventTriggerType.EndDrag;
        onEndDrag.callback.AddListener(OnEndDrag);
        trigger.triggers.Add(onEndDrag);

        mainCamera = Camera.main;

        var rotButtons = rotationButtonsRef.GetComponentsInChildren<Button>();
        foreach (var rotButton in rotButtons)
        {
            SetupRotationCallbacked(rotButton.gameObject);
        }


        abilityMarker = AbilityInfo.InstantiateForLocalIndicator(Vector2.zero, 0).GetComponent<AbilityObject>();
        abilityMarker.gameObject.SetActive(false);
        abilityMarker.SetStatus(AbilityObject.Status.GhostPlaceable);

        iconRef.sprite = AbilityInfo.abilityButtonSprite;
        costRef.text = AbilityInfo.manaCost.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if (status == Status.Placing || status == Status.Placed)
        {
            transform.position = centerPositionCache;
        }
        else if (status == Status.Default)
        {
            LerpPosition();
        }

        SetUsable(GamePlayController.Instance.LocalPlayerManaEnough(AbilityInfo.manaCost) && !ForceUnusable);
    }


    public void LerpPosition()
    {
        centerPositionCache = transform.parent.position;
        transform.position = Vector3.Lerp(lastFramePosition, centerPositionCache, Time.deltaTime * 5f);

        lastFramePosition = transform.position;
    }

    public void SetUsable(bool usable)
    {
        if (usable == Usable)
            return;
        Usable = usable;

        canvasGroup.interactable = usable;
        canvasGroup.alpha = usable ? 1 : 0.5f;
        var trigger = GetComponent<EventTrigger>();
        trigger.enabled = usable;
    }

    #region drag listeners

    public void OnBeginDrag(BaseEventData eventData)
    {
        if (status == Status.Default)
        {
            TransDefaultToMoving();
        }
        else if (status == Status.Placed)
        {
            TransPlacedToPlacing();
        }

    }
    public void OnDrag(BaseEventData eventDataBase)
    {
        PointerEventData eventData = (PointerEventData)eventDataBase;
        if (status == Status.Default)
        {

        }
        else if (status == Status.Moving)
        {
            if (GameplayUI.Instance.IsInsideAbilityPanel(eventData.position))
            {
                transform.position = eventData.position;
            }
            else
            {
                TransMovingToPlacing();
            }
        }
        else if (status == Status.Placing)
        {
            centerPositionCache = eventData.position;
            transform.position = eventData.position;
            var worldPos = mainCamera.ScreenToWorldPoint(eventData.position);
            worldPos.z = 0;
            abilityMarker.transform.position = worldPos;
            if (GameplayUI.Instance.IsInsideAbilityPanel(eventData.position))
            {
                TransPlacingToMoving();
            }
        }


    }

    public void OnEndDrag(BaseEventData eventData)
    {
        if (status == Status.Moving)
            TransMovingToDefault();
        if (status == Status.Placing)
            TransPlacingToPlaced();
    }

    #endregion
    #region state transition functions

    protected void TransMovingToPlacing()
    {
        status = Status.Placing;
        graphicsRef.SetActive(false);
        imageRef.color = new Color(1, 1, 1, 0);
        confirmButtonsRef.SetActive(false);
        abilityMarker.gameObject.SetActive(true);

        //also update the positoin of the marker object
        var worldPos = mainCamera.ScreenToWorldPoint(transform.position);
        worldPos.z = 0;
        abilityMarker.transform.position = worldPos;
    }
    protected void TransPlacingToMoving()
    {
        status = Status.Moving;
        graphicsRef.SetActive(true);
        confirmButtonsRef.SetActive(false);
        rotationButtonsRef.SetActive(false);
        imageRef.color = new Color(1, 1, 1, 1);
        abilityMarker.gameObject.SetActive(false);
    }

    protected void TransDefaultToMoving()
    {
        status = Status.Moving;
        GameplayUI.Instance.SetOtherButtonsForceUnusable(this, true);
    }
    protected void TransMovingToDefault()
    {
        status = Status.Default;
        transform.localPosition = Vector3.zero;
        GameplayUI.Instance.SetOtherButtonsForceUnusable(this, false);
    }

    protected void TransPlacingToPlaced()
    {
        status = Status.Placed;
        confirmButtonsRef.SetActive(true);
        if (AbilityInfo.allowRotation)
            rotationButtonsRef.SetActive(true);
    }
    protected void TransPlacedToPlacing()
    {
        status = Status.Placing;
        confirmButtonsRef.SetActive(false);
        rotationButtonsRef.SetActive(false);
    }

    public void TransToDefault()
    {
        status = Status.Default;
        transform.localPosition = Vector3.zero;
        graphicsRef.SetActive(true);
        confirmButtonsRef.SetActive(false);
        rotationButtonsRef.SetActive(false);
        imageRef.color = new Color(1, 1, 1, 1);
        abilityMarker.gameObject.SetActive(false);
        GameplayUI.Instance.SetOtherButtonsForceUnusable(this, false);
    }

    public void TransPlacedToRotating()
    {
        status = Status.Rotating;
        confirmButtonsRef.SetActive(false);
    }
    public void TransRotatingToPlaced()
    {
        status = Status.Placed;
        confirmButtonsRef.SetActive(true);
    }
    #endregion

    //called when the confirm button is clicked.
    public void OnConfirmed()
    {
        var command = AbilityInfo.GenerateCommand(abilityMarker.transform.position, abilityMarker.transform.rotation.eulerAngles.z, GamePlayController.Instance.PlayerIndex);
        CommandController.Instance.BroadcastCommand(command);
        AbilityInfo.DestroyLocalIndicator(abilityMarker.gameObject);

        DestroyImmediate(transform.parent.gameObject);  //this has to be immediate otherwise the layout won't rebuild properly
        GameplayUI.Instance.OnAbilityUsed(this);
    }


    protected void SetupRotationCallbacked(GameObject rotationButton)
    {
        var trigger = rotationButton.GetComponent<EventTrigger>();
        EventTrigger.Entry onBeginDrag = new EventTrigger.Entry();
        onBeginDrag.eventID = EventTriggerType.BeginDrag;
        onBeginDrag.callback.AddListener(OnRotationButtonStart);
        trigger.triggers.Add(onBeginDrag);

        EventTrigger.Entry onDrag = new EventTrigger.Entry();
        onDrag.eventID = EventTriggerType.Drag;
        onDrag.callback.AddListener(OnRotationButtonDrag);
        trigger.triggers.Add(onDrag);

        EventTrigger.Entry onEndDrag = new EventTrigger.Entry();
        onEndDrag.eventID = EventTriggerType.EndDrag;
        onEndDrag.callback.AddListener(OnRotationButtonEnd);
        trigger.triggers.Add(onEndDrag);
    }

    public void OnRotationButtonDrag(BaseEventData eventDataBase)
    {
        PointerEventData eventData = (PointerEventData)eventDataBase;
        if (status == Status.Rotating)
        {
            var newVector = mainCamera.ScreenToWorldPoint(eventData.position) - abilityMarker.transform.position;
            newVector.z = 0;

            float delta = Quaternion.FromToRotation(rotStartVector, newVector).eulerAngles.z;

            abilityMarker.transform.rotation = Quaternion.Euler(0, 0, delta + rotStartRotation);

        }
    }
    public void OnRotationButtonStart(BaseEventData eventDataBase)
    {
        PointerEventData eventData = (PointerEventData)eventDataBase;
        if (status == Status.Placed)
        {
            TransPlacedToRotating();
            rotStartVector = mainCamera.ScreenToWorldPoint(eventData.position) - abilityMarker.transform.position;
            rotStartRotation = abilityMarker.transform.rotation.eulerAngles.z;
        }
    }
    public void OnRotationButtonEnd(BaseEventData eventDataBase)
    {
        PointerEventData eventData = (PointerEventData)eventDataBase;
        if (status == Status.Rotating)
        {
            TransRotatingToPlaced();
        }
    }
}