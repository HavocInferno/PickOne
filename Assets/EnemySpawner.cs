using UnityEngine;
using UnityEngine.Networking;

public class EnemySpawner : NetworkBehaviour {

	public GameObject enemyPrefab;
	public int numberOfEnemies;

	public Vector2 spawnRange = new Vector2(16f, 16f);

	public override void OnStartServer()
	{
		for (int i=0; i < numberOfEnemies; i++)
		{
			var spawnPosition = new Vector3(
				Random.Range(-spawnRange.x, spawnRange.x),
				0.0f,
				Random.Range(-spawnRange.y, spawnRange.y));

			var spawnRotation = Quaternion.Euler( 
				0.0f, 
				Random.Range(0,180), 
				0.0f);

			var enemy = (GameObject)Instantiate(enemyPrefab, spawnPosition, spawnRotation);
			NetworkServer.Spawn(enemy);
		}
	}
}