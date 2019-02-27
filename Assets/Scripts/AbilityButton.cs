using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
[RequireComponent(typeof(EventTrigger))]
public class AbilityButton : MonoBehaviour
{
    
    [SerializeField]
    protected AbilityInfo abilityInfo;
    [SerializeField]
    protected GameObject graphicsRef;
    [SerializeField]
    protected GameObject confirmButtonsRef;
    protected enum Status
    {
        Default,
        Moving,
        Placing,
        Placed,
        Confirmed
    }
    protected Status status = Status.Default;
    protected Image imageRef;

    protected AbilityObject abilityMarker;

    private Camera mainCamera;

    private void Awake()
    {
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

    }
    // Start is called before the first frame update
    void Start()
    {

        abilityMarker = abilityInfo.InstantiateForLocalIndicator(Vector2.zero, 0).GetComponent<AbilityObject>();
        abilityMarker.gameObject.SetActive(false);
        abilityMarker.SetStatus(AbilityObject.Status.GhostPlaceable);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnBeginDrag(BaseEventData eventData)
    { 
        if(status == Status.Default)
        {
            TransDefaultToMoving();
        }else if(status == Status.Placed)
        {
            TransPlacedToPlacing();
        }
        
    }
    public void OnDrag(BaseEventData eventDataBase)
    {
        PointerEventData eventData = (PointerEventData)eventDataBase;
        if (status == Status.Default)
        {

        }else if(status == Status.Moving)
        {
            if (GameplayUI.Instance.IsInsideAbilityPanel(eventData.position))
            {
                transform.position = eventData.position;
            }
            else
            {
                TransMovingToPlacing();
            }
        }else if(status == Status.Placing)
        {
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
        if(status == Status.Moving)
            TransMovingToDefault();
        if (status == Status.Placing)
            TransPlacingToPlaced();
    }


    protected void TransMovingToPlacing()
    {
        status = Status.Placing;
        graphicsRef.SetActive(false);
        imageRef.color = new Color(1,1,1,0);
        confirmButtonsRef.SetActive(true);
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
        imageRef.color = new Color(1, 1, 1, 1);
        abilityMarker.gameObject.SetActive(false);
    }

    protected void TransDefaultToMoving()
    {
        status = Status.Moving;
    }
    protected void TransMovingToDefault()
    {
        status = Status.Default;
        transform.localPosition = Vector3.zero;
    }



    protected void TransPlacingToPlaced()
    {
        status = Status.Placed;
    }
    protected void TransPlacedToPlacing()
    {
        status = Status.Placing;
    }

    public void TransToDefault()
    {
        status = Status.Default;
        transform.localPosition = Vector3.zero;
        graphicsRef.SetActive(true);
        confirmButtonsRef.SetActive(false);
        imageRef.color = new Color(1, 1, 1, 1);
        abilityMarker.gameObject.SetActive(false);
    }

    public void OnConfirmed()
    {
        var command = abilityInfo.GenerateCommand(abilityMarker.transform.position, abilityMarker.transform.rotation.eulerAngles.z, GamePlayController.Instance.PlayerIndex);
        CommandController.Instance.BroadcastCommand(command);
        abilityInfo.DestroyLocalIndicator(abilityMarker.gameObject);
        Destroy(transform.parent.gameObject);
    }
}
