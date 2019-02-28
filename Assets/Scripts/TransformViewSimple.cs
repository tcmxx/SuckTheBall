﻿
using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class TransformViewSimple : MonoBehaviour, IPunObservable
{
    protected double interpolationBackTime = 0.2;
    public double extrapolationLimit = 0.5;
    public float maxSpeedDeltaSqr = 9f;

    public bool syncLocalPosition = true;
    public bool syncLocalRotation = true;
    public bool checkForSpeedHacks = false;

    protected PhotonView photonView;
    // Clients store twenty states with "playback" information from the server. This
    // array contains the official state of this object at different times according to
    // the server.
    private State[] proxyStates = new State[20];

    // Keep track of what slots are used
    private int proxyStateCount;

    /// <summary>
    /// Cached transform
    /// </summary>
    private Transform myTransform;

    /// <summary>
    /// The change in position in the most recent frame. Applies
    /// to all sessions including the owner
    /// </summary>
    public Vector3 LocalPositionDelta
    {
        get
        {
            return localPositionDelta;
        }
    }
    private Vector3 localPositionDelta;

    /// <summary>
    /// The position of this transform in the previous frame
    /// </summary>
    private Vector3 prevLocalPosition;

    /// <summary>
    /// Synchronized object state
    /// </summary>
    protected struct State
    {
        public double timestamp;
        public Vector3 pos;
        public Vector3 vel;
        public Quaternion rot;
    }

    #region MonoBehaviour

    void Awake()
    {
        myTransform = transform;
        prevLocalPosition = myTransform.localPosition;
        photonView = GetComponent<PhotonView>();
    }
    private void Start()
    {
        interpolationBackTime = NetworkController.Instance.interpolationBackTime;
    }
    void Update()
    {
        if (!photonView.IsMine)
        {
            // You are not the owner, so you have to converge the object's state toward the server's state.
            // Entity interpolation happens here; see https://developer.valvesoftware.com/wiki/Source_Multiplayer_Networking

            // This is the target playback time of this body
            double interpolationTime = PhotonNetwork.Time - interpolationBackTime;

            // Use interpolation if the target playback time is present in the buffer
            if (proxyStates[0].timestamp > interpolationTime)
            {
                // Go through buffer and find correct state to play back
                for (int i = 0; i < proxyStateCount; i++)
                {
                    if (proxyStates[i].timestamp <= interpolationTime || i == proxyStateCount - 1)
                    {
                        // The state one slot newer (<100ms) than the best playback state
                        State rhs = proxyStates[Mathf.Max(i - 1, 0)];
                        // The best playback state (closest to 100 ms old (default time))
                        State lhs = proxyStates[i];

                        // Use the time between the two slots to determine if interpolation is necessary
                        double length = rhs.timestamp - lhs.timestamp;
                        float t = 0.0F;
                        // As the time difference gets closer to 100 ms t gets closer to 1 in 
                        // which case rhs is only used
                        // Example:
                        // Time is 10.000, so sampleTime is 9.900 
                        // lhs.time is 9.910 rhs.time is 9.980 length is 0.070
                        // t is 9.900 - 9.910 / 0.070 = 0.14. So it uses 14% of rhs, 86% of lhs
                        if (length > 0.0001)
                            t = (float)((interpolationTime - lhs.timestamp) / length);

                        // if t=0 => lhs is used directly
                        if (syncLocalPosition) myTransform.localPosition = Vector3.Lerp(lhs.pos, rhs.pos, t);
                        if (syncLocalRotation) myTransform.localRotation = Quaternion.Slerp(lhs.rot, rhs.rot, t);
                        break;
                    }
                }
            }
            // Use extrapolation
            else
            {
                State latest = proxyStates[0];

                float extrapolationLength = (float)(interpolationTime - latest.timestamp);
                // Don't extrapolation for more than 500 ms, you would need to do that carefully
                if (extrapolationLength < extrapolationLimit)
                {
                    if (syncLocalPosition) myTransform.localPosition = latest.pos + latest.vel * extrapolationLength;
                    if (syncLocalRotation) myTransform.localRotation = latest.rot;
                }
            }
        }
        else
        {
            // You're the owner so there's no reason to do any convergance. Just update the 
            // position delta and previous position
        }

        // Update the local position delta
        localPositionDelta = myTransform.localPosition - prevLocalPosition;
        prevLocalPosition = myTransform.localPosition;
    }

    #endregion

    #region IPunCallbacks

    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(myTransform.localPosition);
            stream.SendNext(localPositionDelta);
            stream.SendNext(myTransform.localRotation);
        }
        else
        {
            // Network player, receive data
            Vector3 pos = (Vector3)stream.ReceiveNext();
            Vector3 velocity = (Vector3)stream.ReceiveNext();
            Quaternion rot = (Quaternion)stream.ReceiveNext();

            // If we're ignoring position data from the owning session, then use our own values. This
            // should only happen in special cases
            if (!syncLocalPosition)
            {
                pos = myTransform.localPosition;
                velocity = localPositionDelta;
            }

            // If we're ignoring rotation data from the owning session, then use our own values. This
            // should only happen in special cases
            if (!syncLocalRotation)
            {
                rot = myTransform.localRotation;
            }

            // Check for speed hacks
            if (checkForSpeedHacks && proxyStates.Length > 0)
            {
                Vector3 delta = pos - proxyStates[0].pos;
                if (delta.sqrMagnitude > maxSpeedDeltaSqr)
                {
#if UNITY_EDITOR
                    Debug.LogWarning("Speed hack detected. Throttling velocity of " + delta.magnitude);
                    pos = proxyStates[0].pos + delta.normalized * Mathf.Sqrt(maxSpeedDeltaSqr);
#endif
                }
            }

            // Shift the buffer sideways, deleting state 20
            for (int i = proxyStates.Length - 1; i >= 1; i--)
            {
                proxyStates[i] = proxyStates[i - 1];
            }

            // Record current state in slot 0
            State state;
            state.timestamp = info.SentServerTime;

            state.pos = pos;
            state.vel = velocity;
            state.rot = rot;
            proxyStates[0] = state;

            // Update used slot count, however never exceed the buffer size
            // Slots aren't actually freed so this just makes sure the buffer is
            // filled up and that uninitalized slots aren't used.
            proxyStateCount = Mathf.Min(proxyStateCount + 1, proxyStates.Length);

            // Check if states are in order
            if (proxyStates[0].timestamp < proxyStates[1].timestamp)
            {
#if UNITY_EDITOR
                Debug.LogWarning("Timestamp inconsistent: " + proxyStates[0].timestamp + " should be greater than " + proxyStates[1].timestamp);
#endif
            }
        }
    }
    
    #endregion
}
