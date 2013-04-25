using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour
{
	
	[SerializeField] private string[] _enemies;
	[SerializeField] private GameObject[] _spawnPoints;
	[SerializeField] private float _maxSpawnRange;
	[SerializeField] private float _breakTimeBetweenWaves;
	
	public int CurrentWave { get; private set; }
	public bool IsInWave { get; private set; }
	
	private int _enemiesLeftToSpawn = 0;
	private float _timeBeforeNextSpawn;
	
	private void Awake()
	{
		if (!PhotonNetwork.isMasterClient)
		{
			print("not master client");
			this.enabled = false;
		}
		CurrentWave = 0;
		IsInWave = false;
	}
	
	private void Start()
	{
		GoToNextWave();
	}
	
	private void Update()
	{
		if (IsInWave)
		{
			if (_enemiesLeftToSpawn == 0 && Enemy.NumberOfEnemies() == 0)
			{
				GoToNextWave();
				return;
			}
			
			if (_enemiesLeftToSpawn > 0)
			{
				_timeBeforeNextSpawn -= Time.deltaTime;
				if (_timeBeforeNextSpawn <= 0)
				{
					_timeBeforeNextSpawn = Mathf.Max(10, _enemiesLeftToSpawn / 2);
					if (_enemiesLeftToSpawn < 6)
					{
						SpawnEnemies(_enemiesLeftToSpawn);
					}
					else
					{
						SpawnEnemies(_enemiesLeftToSpawn / 2);
					}
				}
			}
		}
	}
	
	private void SpawnEnemies(int numberOfEnemiesToSpawn)
	{
		int spawnIndex = (int)(Random.value * ((float)_spawnPoints.Length));
		for (int i = 0; i < numberOfEnemiesToSpawn; ++i)
		{
			int index = (int)(Random.value * ((float)_enemies.Length));
			
			var deltaPosition = new Vector2(Random.value * 2 - 1, Random.value * 2 - 1);
			deltaPosition *= _maxSpawnRange;
			var spawnPosition = _spawnPoints[spawnIndex].transform.position + new Vector3(deltaPosition.x, 0, deltaPosition.y);
			
			var enemy = PhotonNetwork.Instantiate(_enemies[index], spawnPosition, Quaternion.Euler(0, 0, 0), 0);
			EnableScripts(enemy);
		}
		_enemiesLeftToSpawn -= numberOfEnemiesToSpawn;
	}
	
	private void EnableScripts(GameObject enemy)
	{
		var hover = enemy.GetComponent<Hover>();
		if (hover != null)
		{
			hover.enabled = true;
		}
	}
	
	private void GoToNextWave()
	{
		print("next wave");
		IsInWave = false;
		++CurrentWave;
		
		// respawn any dead players
		
		StartCoroutine(WaitAndStartWave());
	}
	
	private IEnumerator WaitAndStartWave()
	{
		yield return new WaitForSeconds(_breakTimeBetweenWaves);
		
		_enemiesLeftToSpawn = CurrentWave * 4;
		if (CurrentWave % 3 == 0)
		{
			SpawnValkyrie();
			if (CurrentWave == 3)
			{
				_enemiesLeftToSpawn = 0;
			}
			else
			{
				_enemiesLeftToSpawn /= 2;
			}
		}
		
		IsInWave = true;
	}
	
	private void SpawnValkyrie()
	{
#warning need to spawn the Valkyrie
	}
	
}