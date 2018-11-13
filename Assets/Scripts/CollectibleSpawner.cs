using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Spawner class that helps in spawning Collectibles for specific player.
/// </summary>
public class CollectibleSpawner : MonoBehaviour {

    /// <summary>
    /// Zone in which collectibles will be spawned.
    /// </summary>
    private BoxCollider2D box;
    private Bounds spawnZone;

    /// <summary>
    /// List of collectible prefabs random one from that will be spawned when spawn is requested.
    /// </summary>
    public List<GameObject> collectiblesPrefab;
    
	// Use this for initialization
	void Start () {
        box = GetComponent<BoxCollider2D>();
        spawnZone = box.bounds;
	}

    /// <summary>
    /// Spawned function that takes Player's GUID to spawn collectible.
    /// </summary>
    /// <param name="withGuid">GUID of player to whom collectible is being spawned.</param>
    /// <returns>Spawned collectible script object.</returns>
    public CollectibleItem spawnCollectible(System.Guid withGuid)
    {
        if (collectiblesPrefab.Count == 0)
            return null;
        Vector2 randNormalizedPt = Random.insideUnitCircle;
        Vector3 spawnLoc = new Vector3(spawnZone.center.x + (spawnZone.extents.x * randNormalizedPt.x),
            spawnZone.center.y + (spawnZone.extents.y * randNormalizedPt.y), 0);
        GameObject go=Instantiate(collectiblesPrefab[Random.Range(0, collectiblesPrefab.Count)],spawnLoc,Quaternion.identity);
        CollectibleItem item = go.GetComponent<CollectibleItem>();
        item.userId = withGuid;
        return item;
    }
}
