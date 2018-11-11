using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleSpawner : MonoBehaviour {

    private BoxCollider2D box;
    private Bounds spawnZone;

    public List<GameObject> collectiblesPrefab;
    
	// Use this for initialization
	void Start () {
        box = GetComponent<BoxCollider2D>();
        spawnZone = box.bounds;
	}

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
