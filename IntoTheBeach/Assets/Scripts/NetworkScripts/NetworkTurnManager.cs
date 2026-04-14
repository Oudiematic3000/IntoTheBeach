using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class NetworkTurnManager : NetworkBehaviour
{
    public static NetworkTurnManager Instance { get; private set; }
    public static event Action<NetUnitResult[]> OnTurnResolved; 

    private Dictionary<ulong, NetUnitPlan[]> submittedPlans = new();
    private int expectedPlayerCount = 2;

    public override void OnNetworkSpawn()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    //[Rpc(SendTo.Server, InvokePermission=RpcInvokePermission.Everyone)]
    //public void SubmitTurnPlanServerRpc(NetUnitPlan[] plans, ServerRpcParams rpcParams = default)
    //{
    //    ulong senderID = rpcParams.Receive.SenderClientId;
    //    submittedPlans[senderID] = plans;

    //    if (submittedPlans.Count >= expectedPlayerCount)
    //    {
            
    //    }
    //}

   
}
public struct NetVector3Int : INetworkSerializable
{
    public int x, y, z;

    public void NetworkSerialize<T>(BufferSerializer<T> s) where T : IReaderWriter
    {
        s.SerializeValue(ref x);
        s.SerializeValue(ref y);
        s.SerializeValue(ref z);
    }

    public Vector3Int ToVector3Int() => new Vector3Int(x, y, z);
    public static NetVector3Int From(Vector3Int v) => new NetVector3Int { x = v.x, y = v.y, z = v.z };
}

public struct NetPath : INetworkSerializable
{
    public NetVector3Int move;
    public path.MoveType moveType;

    public void NetworkSerialize<T>(BufferSerializer<T> s) where T : IReaderWriter
    {
        s.SerializeValue(ref move);
        s.SerializeValue(ref moveType);
    }

    public static NetPath From(path p) => new NetPath
    {
        move = NetVector3Int.From(p.move),
        moveType = p.moveType
    };

    public path ToPath() => new path(move.ToVector3Int(), moveType);
}

public struct NetUnitPlan : INetworkSerializable
{
    public int unitID;
    public NetVector3Int startPos;
    public NetVector3Int resultant;
    public NetPath[] paths;

    public void NetworkSerialize<T>(BufferSerializer<T> s) where T : IReaderWriter
    {
        s.SerializeValue(ref unitID);
        s.SerializeValue(ref startPos);
        s.SerializeValue(ref resultant);

        int pathCount = paths?.Length ?? 0;
        s.SerializeValue(ref pathCount);

        if (s.IsReader)
            paths = new NetPath[pathCount];

        for (int i = 0; i < pathCount; i++)
            s.SerializeNetworkSerializable(ref paths[i]);
    }

    public static NetUnitPlan From(int id, MoveAction action) => new NetUnitPlan
    {
        unitID = id,
        startPos = NetVector3Int.From(action.startPos),
        resultant = NetVector3Int.From(action.resultant),
        paths = action.paths.Select(p => NetPath.From(p)).ToArray()
    };

    public MoveAction ToMoveAction()
    {
        var action = new MoveAction(startPos.ToVector3Int(), resultant.ToVector3Int());
        action.paths = paths.Select(p => p.ToPath()).ToList();
        return action;
    }
}

public struct NetUnitResult : INetworkSerializable
{
    public int unitID;
    public NetVector3Int startPos;
    public NetVector3Int finalPos;
    public NetPath[] paths;

    public void NetworkSerialize<T>(BufferSerializer<T> s) where T : IReaderWriter
    {
        s.SerializeValue(ref unitID);
        s.SerializeValue(ref startPos);
        s.SerializeValue(ref finalPos);

        int pathCount = paths?.Length ?? 0;
        s.SerializeValue(ref pathCount);

        if (s.IsReader)
            paths = new NetPath[pathCount];

        for (int i = 0; i < pathCount; i++)
            s.SerializeNetworkSerializable(ref paths[i]);
    }

    public static NetUnitResult From(int id, MoveAction action) => new NetUnitResult
    {
        unitID = id,
        startPos = NetVector3Int.From(action.startPos),
        finalPos = NetVector3Int.From(action.resultant),
        paths = action.paths.Select(p => NetPath.From(p)).ToArray()
    };
}