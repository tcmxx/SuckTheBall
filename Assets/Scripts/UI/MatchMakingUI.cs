using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchMakingUI : MonoBehaviourPunCallbacks
{
    [SerializeField]
    protected InputField nameFieldRef;
    [SerializeField]
    protected Text statusTextRef;
    [SerializeField]
    protected GameObject matchMakingPanelRef;

    const string playerNamePrefKey = "PlayerName";

    // Start is called before the first frame update
    void Start()
    {
        string defaultName = string.Empty;
        if (nameFieldRef != null)
        {
            if (PlayerPrefs.HasKey(playerNamePrefKey))
            {
                defaultName = PlayerPrefs.GetString(playerNamePrefKey);
                nameFieldRef.text = defaultName;
            }
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        statusTextRef.text = Photon.Pun.PhotonNetwork.NetworkClientState.ToString();
    }


    public void OnMatchMaking()
    {
        if (string.IsNullOrEmpty(nameFieldRef.text))
        {
            ((Text)nameFieldRef.placeholder).text = "Please Enter a Name";
            return;
        }
        SetPlayerName(nameFieldRef.text);
        NetworkController.Instance.MatchMaking(PlayerPrefs.GetString(playerNamePrefKey));

        matchMakingPanelRef.SetActive(false);
    }
    /// <summary>
    /// Sets the name of the player, and save it in the PlayerPrefs for future sessions.
    /// </summary>
    /// <param name="value">The name of the Player</param>
    public void SetPlayerName(string value)
    {
        // #Important
        if (string.IsNullOrEmpty(value))
        {
            Debug.LogError("Player Name is null or empty");
            return;
        }
        PlayerPrefs.SetString(playerNamePrefKey, value);
    }
}
