using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetBallSpawner : MonoBehaviour
{
    [Serializable]
    public struct SpawnPhase
    {
        public float interval;
        public float ballNumber;
        public float lastCount;
    }


    public static TargetBallSpawner Instance { get; protected set; }
    [SerializeField]
    protected string targetBallPrefID;
    [SerializeField]
    protected Transform[] spawnPositions;
    [SerializeField]
    protected List<SpawnPhase> phases;
    protected int currentPhase = 0;
    protected int currentCount = 0;
    protected float timer = float.MaxValue;
    public bool Started { get; set; } = false;


    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        currentPhase = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (Started)
        {
            timer += Time.deltaTime;
            if(timer >= phases[currentPhase].interval){
                for (int i = 0; i < phases[currentPhase].ballNumber;++i) {
                    Spawn(spawnPositions[i].position);
                }
                timer = 0;
                currentCount++;
                if(currentCount >= phases[currentPhase].lastCount && phases.Count > currentPhase + 1)
                {
                    currentPhase++;
                }
            }
        }
    }


    public void Spawn(Vector2 pos)
    {
        Photon.Pun.PhotonNetwork.InstantiateSceneObject(targetBallPrefID, pos, Quaternion.identity);
    }


}
