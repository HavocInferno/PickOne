using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Spawns objects randomly in specified area.
/// </summary>
public class Spawner : NetworkBehaviour
{
	public GameObject prefab;
	public int numberOfEnemies;
    public Rect area = new Rect(0f, 0f, 20f, 20f);

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

	void OnDrawGizmos()
	{
		//Gizmos.DrawIcon(transform.position, "ModuleIcon");

		Gizmos.DrawWireCube(transform.position, new Vector3(area.width, 0, area.height));

		Gizmos.DrawLine(transform.position - Vector3.up * 1f, transform.position + Vector3.up * 1f);
		Gizmos.DrawLine(transform.position - Vector3.forward * 1f, transform.position + Vector3.forward * 1f);
		Gizmos.DrawLine(transform.position - Vector3.left * 1f, transform.position + Vector3.left * 1f);
	}
}