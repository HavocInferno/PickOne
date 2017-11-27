using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Spawns objects randomly in specified area.
/// </summary>
public class Spawner : NetworkBehaviour
{
	public GameObject prefab;
	public int numberOfEnemies;
    public Rect area = new Rect(-8.0f, -8.0f, 16.0f, 16.0f);

    GameObject Spawn()
    {
        var spawnPosition = new Vector3(
            Random.Range(area.xMin, area.xMax),
            0.0f,
            Random.Range(area.yMin, area.yMax));

        var spawnRotation = Quaternion.Euler(
            0.0f,
            Random.Range(0, 360),
            0.0f);

        var enemy = Instantiate(prefab, spawnPosition, spawnRotation);
        enemy.SetActive(true);
        NetworkServer.Spawn(enemy);
        return enemy;
    }

	public override void OnStartServer()
	{
		for (int i = 0; i < numberOfEnemies; i++) Spawn();
	}
}