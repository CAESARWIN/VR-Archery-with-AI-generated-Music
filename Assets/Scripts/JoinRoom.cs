using UnityEngine;
using Ubiq.Rooms;
using System;

public class RoomAutoJoiner : MonoBehaviour
{
    public RoomClient roomClient;

    void Start()
    {
        // 请确认此 GUID 与你 Node.js 配置文件中的 roomGuid 完全一致
        var roomId = new Guid("6765c52b-3ad6-4fb0-9030-2c9a05dc4731");
        roomClient.Join(roomId);
        Debug.Log("Attempting to join room " + roomId);
    }
}
