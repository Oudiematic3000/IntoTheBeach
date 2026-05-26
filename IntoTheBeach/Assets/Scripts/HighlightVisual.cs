using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class HighlightVisual : MonoBehaviour
{
    public static HighlightVisual instance;
    [SerializeField] Tilemap highlightVisual;
    [SerializeField] Tilemap gridVisual;
    [SerializeField] TileBase outlineTile;

    private List<Vector3Int> currentTiles = new List<Vector3Int>();

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public void PaintOutline(List<Vector3Int> tiles)
    {
        if (TilesMatch(tiles)) return;

        currentTiles = new List<Vector3Int>(tiles);

        LeanTween.cancel(gameObject);
        highlightVisual.ClearAllTiles();

        foreach (var tile in tiles)
            if (gridVisual.HasTile(tile))
                highlightVisual.SetTile(tile, outlineTile);

        highlightVisual.color = new Color(1f, 1f, 1f, 0f);
        LeanTween.value(gameObject, 0f, 1f, 0.5f)
            .setOnUpdate((float val) =>
            {
                highlightVisual.color = new Color(1f, 1f, 1f, val);
            });
    }

    public void ClearOutline()
    {
        if (currentTiles.Count == 0) return; 
        currentTiles.Clear();
        LeanTween.cancel(gameObject);
        highlightVisual.color = new Color(1f, 1f, 1f, 1f);
        highlightVisual.ClearAllTiles();
    }

    private bool TilesMatch(List<Vector3Int> tiles)
    {
        if (tiles.Count != currentTiles.Count) return false;
        for (int i = 0; i < tiles.Count; i++)
            if (tiles[i] != currentTiles[i]) return false;
        return true;
    }
}