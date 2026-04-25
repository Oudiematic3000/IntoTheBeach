using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class NetworkTurnManager : NetworkBehaviour
{
    public static NetworkTurnManager Instance { get; private set; }

    private Dictionary<ulong, NetUnitPlan[]> submittedPlans = new();
    private int expectedPlayerCount = 2;

    public override void OnNetworkSpawn()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    public void SubmitTurnPlanServerRpc(NetUnitPlan[] plans, RpcParams rpcParams = default)
    {
        expectedPlayerCount = NetworkManager.Singleton.ConnectedClientsList
            .Select(c => c.PlayerObject?.GetComponent<PlayerData>())
            .Where(pd => pd != null)
            .ToList().Count;
        ulong senderID = rpcParams.Receive.SenderClientId;
        submittedPlans[senderID] = plans;
        Debug.Log("Received Plan from " + senderID);
        if (submittedPlans.Count >= expectedPlayerCount)
        {
            ResolveTurn();
        }
    }

    private void ResolveTurn()
    {
        var allPlans = submittedPlans.Values
            .SelectMany(plans => plans)
            .ToArray();

        var resolver = new TurnResolver(GameManager.Instance.GridState);
        NetUnitResult[] results = resolver.Resolve(allPlans);

        submittedPlans.Clear();
        BroadcastResolvedTurnClientRpc(results);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void BroadcastResolvedTurnClientRpc(NetUnitResult[] results)
    {
        Debug.Log("broadcasting results: "+results.Length);
        TurnStateMachine.Instance.currentState = new BoardSyncTurnState(TurnStateMachine.Instance, results);
    }
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

public struct NetAttackAction : INetworkSerializable
{
    public NetVector3Int unitPos;
    public int attackPatternTypeID; 
    public int direction;
    public int unitID;

    public void NetworkSerialize<T>(BufferSerializer<T> s) where T : IReaderWriter
    {
        s.SerializeValue(ref unitPos);
        s.SerializeValue(ref attackPatternTypeID);
        s.SerializeValue(ref direction);
        s.SerializeValue(ref unitID);
    }

    public static NetAttackAction From(AttackAction action) => new NetAttackAction
    {
        unitPos = NetVector3Int.From(action.unitPos),
        attackPatternTypeID = action.attackPattern.TypeID,
        direction = action.direction,
        unitID = action.unitID
    };

    public AttackAction ToAttackAction() => new AttackAction(
        unitPos.ToVector3Int(),
        AttackPatternRegistry.FromID(attackPatternTypeID),
        direction,
        unitID
    );
}

public struct NetUnitPlan : INetworkSerializable
{
    public int unitID;
    public NetVector3Int startPos;
    public NetVector3Int resultant;
    public NetPath[] paths;
    public bool hasMoveAction;      
    public bool hasAttackAction;
    public NetAttackAction attackAction;

    public void NetworkSerialize<T>(BufferSerializer<T> s) where T : IReaderWriter
    {
        s.SerializeValue(ref unitID);
        s.SerializeValue(ref hasMoveAction);   
        if (hasMoveAction)
        {
            s.SerializeValue(ref startPos);
            s.SerializeValue(ref resultant);
            int pathCount = paths?.Length ?? 0;
            s.SerializeValue(ref pathCount);
            if (s.IsReader) paths = new NetPath[pathCount];
            for (int i = 0; i < pathCount; i++)
                s.SerializeNetworkSerializable(ref paths[i]);
        }
        s.SerializeValue(ref hasAttackAction);
        if (hasAttackAction)
            s.SerializeNetworkSerializable(ref attackAction);
    }

    public static NetUnitPlan From(UnitPlan plan) => new NetUnitPlan
    {
        unitID = plan.unitID,
        hasMoveAction = plan.moveAction != null,
        startPos = plan.moveAction != null ? NetVector3Int.From(plan.moveAction.startPos) : default,
        resultant = plan.moveAction != null ? NetVector3Int.From(plan.moveAction.resultant) : default,
        paths = plan.moveAction != null ? plan.moveAction.paths.Select(p => NetPath.From(p)).ToArray() : null,
        hasAttackAction = plan.attackAction != null,
        attackAction = plan.attackAction != null ? NetAttackAction.From(plan.attackAction) : default
    };

    public MoveAction ToMoveAction() =>
        hasMoveAction ? new MoveAction(startPos.ToVector3Int(), resultant.ToVector3Int())
        { paths = paths.Select(p => p.ToPath()).ToList() } : null;

    public AttackAction ToAttackAction() =>
        hasAttackAction ? attackAction.ToAttackAction() : null;
}

public struct NetUnitResult : INetworkSerializable
{
    public int unitID;
    public NetVector3Int startPos;
    public NetVector3Int finalPos;
    public NetPath[] paths;
    public bool hasAttackAction;
    public NetAttackAction attackAction;
    public int damageTaken;
    public NetVector3Int[] reactedTiles;
    public void NetworkSerialize<T>(BufferSerializer<T> s) where T : IReaderWriter
    {
        s.SerializeValue(ref unitID);
        s.SerializeValue(ref startPos);
        s.SerializeValue(ref finalPos);
        int pathCount = paths?.Length ?? 0;
        s.SerializeValue(ref pathCount);
        if (s.IsReader) paths = new NetPath[pathCount];
        for (int i = 0; i < pathCount; i++)
            s.SerializeNetworkSerializable(ref paths[i]);
        s.SerializeValue(ref hasAttackAction);
        if (hasAttackAction)
            s.SerializeNetworkSerializable(ref attackAction);
        s.SerializeValue(ref damageTaken);
        int reactedCount = reactedTiles?.Length ?? 0;
        s.SerializeValue(ref reactedCount);
        if (s.IsReader) reactedTiles = new NetVector3Int[reactedCount];
        for (int i = 0; i < reactedCount; i++)
            s.SerializeNetworkSerializable(ref reactedTiles[i]);
    }

    public static NetUnitResult From(int id, MoveAction moveAction, AttackAction attackAction = null, int damageTaken = 0, NetVector3Int[] reactedTiles = null) => new NetUnitResult
    {
        unitID = id,
        startPos = NetVector3Int.From(moveAction.startPos),
        finalPos = NetVector3Int.From(moveAction.resultant),
        paths = moveAction.paths.Select(p => NetPath.From(p)).ToArray(),
        hasAttackAction = attackAction != null,
        attackAction = attackAction != null ? NetAttackAction.From(attackAction) : default,
        damageTaken = damageTaken,
        reactedTiles = reactedTiles ?? Array.Empty<NetVector3Int>()
    };

    public MoveAction ToMoveAction()
    {
        var action = new MoveAction(startPos.ToVector3Int(), finalPos.ToVector3Int());
        action.paths = paths.Select(p => p.ToPath()).ToList();
        return action;
    }

    public AttackAction ToAttackAction() =>
        hasAttackAction ? attackAction.ToAttackAction() : null;
}