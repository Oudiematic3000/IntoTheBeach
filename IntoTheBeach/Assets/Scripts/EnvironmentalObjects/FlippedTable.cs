using System.Collections.Generic;
using UnityEngine;

public class FlippedTable : MonoBehaviour, IMovementBlocker, IAttackBlocker
{
    public EnvironmentalObject envObj;
    private GridState gridState;
    [SerializeField] List<Vector3Int> blockDirections;
    [SerializeField] List<GameObject> extraPositions = new List<GameObject>();
  

    void Start()
    {
        gridState = GameManager.Instance.GridState;

        envObj = new EnvironmentalObject();
        envObj.OccupiedTiles.Add(gridState.WorldToCell(transform.position));

        if (extraPositions.Count > 0)
        {
            foreach(GameObject go in extraPositions) envObj.OccupiedTiles.Add(gridState.WorldToCell(go.transform.position));

        }

        gridState.RegisterEnvironmentalObject(envObj);
    }

    void OnDestroy()
    {
        gridState.UnregisterEnvironmentalObject(envObj);
    }
    public bool BlocksAttackFromDirection(Vector3Int direction)
    {
        throw new System.NotImplementedException();
    }
}
