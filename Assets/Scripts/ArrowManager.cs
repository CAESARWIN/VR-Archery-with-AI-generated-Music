using BNG;
using UnityEngine;

using System;
using Ubiq.Messaging;
using System.Collections.Generic;

public class ArrowManager : MonoBehaviour
{
    [Header("Arrow Settings")]
    public GameObject arrowPrefab;
    public Transform spawnPoint;
    public int totalArrows = 20;
    public int arrowsLeft;

    private int needToSpawn = 0;

    // Ubiq networking
    private readonly NetworkId netId = new NetworkId(120);
    private NetworkContext net;

    // Timing state
    private float pickupTime = -1f;         // time when current arrow was grabbed
    private bool timerRunning = false;      // whether the timer is currently running
    private int shotCounter = 0;            // increments every pickup; for logging
    private int? currentShotId = null;      // id of the current running shot

    private readonly List<float> arrowTimes = new List<float>();
    private readonly List<int> arrowScores = new List<int>();

    // Optional: suppress noisy logs on duplicate hits
    [Header("Debug / Safety")]
    public bool warnOnInvalidHit = false;   // set true if you still want warnings

    [Serializable]
    private struct Message
    {
        public string type;
        public int arrowsLeft;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        arrowsLeft = totalArrows;
        needToSpawn = arrowsLeft -1;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void Start()
    {
        // ★ 初始化 Ubiq 网络
        net = NetworkScene.Register(this, netId);

        // 初始发送一次
        SendArrowCount();
    }

    public void SpawnArrow()
    {
        if (arrowPrefab != null && spawnPoint != null)
        {
            if (needToSpawn >= 0)
            {
                arrowsLeft--;
                if (needToSpawn > 0)
                {
                    Instantiate(arrowPrefab, spawnPoint.position, spawnPoint.rotation);
                }
                needToSpawn--;

                // ★ 每次发射后发送箭矢数量
                SendArrowCount();
            }
        }
    }

    public void MarkPickup()
    {
        // If a previous timer is still running (e.g., player grabbed a second arrow quickly),
        // we close it gracefully to avoid inconsistent state.
        if (timerRunning)
        {
            if (warnOnInvalidHit)
            {
                Debug.LogWarning("[ArrowManager] Pickup while previous shot still running. Resetting timer.");
            }
        }

        shotCounter++;
        currentShotId = shotCounter;
        pickupTime = Time.time;
        timerRunning = true;

        Debug.Log($"[ArrowManager] PICKUP #{currentShotId-1} at {pickupTime:F4}s (timer started)");
    }

    public void MarkHit(int score)
    {
        if (!timerRunning)
        {
            if (warnOnInvalidHit)
            {
                Debug.LogWarning("[ArrowManager] MarkHit called but timer is not running. Did you call MarkPickup on grab?");
            }
            // Quietly ignore duplicate / late hits (e.g., bounces) when no active timer.
            return;
        }

        float dt = Time.time - pickupTime;
        arrowTimes.Add(dt);
        arrowScores.Add(score);

        int id = currentShotId.HasValue ? currentShotId.Value : -1;
        Debug.Log($"[Hit Info] Arrow #{id-1} time: {dt:F4}s, Score: {score}");

        // reset for the next arrow
        timerRunning = false;
        pickupTime = -1f;
        currentShotId = null;
    }

    public float GetTimeTakenToHitTarget()
    {
        return timerRunning ? Time.time - pickupTime : 0f;
    }

    private void SendArrowCount()
    {
        Message msg = new Message
        {
            type = "ArrowCount",
            arrowsLeft = arrowsLeft
        };

        net.SendJson(msg);

        Debug.Log($"[ArrowManager] Sent arrow count: {arrowsLeft}");
    }

    // ★ 必须实现以兼容 Ubiq
    public void ProcessMessage(ReferenceCountedSceneGraphMessage message)
    {
        Debug.Log("[ArrowManager] Received message: " + message.ToString());
    }
}
