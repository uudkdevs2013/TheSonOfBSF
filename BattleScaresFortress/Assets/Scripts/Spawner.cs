using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour
{
	
	[SerializeField] private string[] _enemies;
	[SerializeField] private string[] _toughEnemies;
	[SerializeField] private GameObject[] _spawnPoints;
	[SerializeField] private float _maxSpawnRange;
	[SerializeField] private float _breakTimeBetweenWaves;
	[SerializeField] private GUISkin _guiSkin;
	
	public int CurrentWave { get; private set; }
	public bool IsInWave { get; private set; }
	
	private int _enemiesLeftToSpawn = 0;
	private float _timeBeforeNextSpawn;
	
	private bool _isShowingEnemiesApproaching = false;
	private bool _isShowingWaveComplete = false;
	
	private void Awake()
	{
		if (!PhotonNetwork.isMasterClient)
		{
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
				StartCoroutine("ShowWaveComplete");
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
	
	private void OnGUI()
	{
		GUI.skin = _guiSkin;
		
		GUI.Label(new Rect(Screen.width - 300, 100, 300, 50), "Wave: " + CurrentWave);
		
		if (_isShowingEnemiesApproaching)
		{
			GUI.Label(new Rect(Screen.width / 2 - 250, Screen.height - 150, 500, 100), "Enemies are approaching!");
		}
		if (_isShowingWaveComplete)
		{
			GUI.Label(new Rect(Screen.width / 2 - 200, Screen.height - 100, 400, 50), "Wave Completed!");
		}
	}
	
	private void SpawnEnemies(int numberOfEnemiesToSpawn)
	{
		int numberOfToughEnemiesToSpawn = 0;
		if (CurrentWave < 5)
		{
			numberOfToughEnemiesToSpawn = 0;
		}
		else if (CurrentWave < 10)
		{
			numberOfToughEnemiesToSpawn = numberOfEnemiesToSpawn / 5;
		}
		else if (CurrentWave < 15)
		{
			numberOfToughEnemiesToSpawn = numberOfEnemiesToSpawn / 10;
		}
		else
		{
			numberOfToughEnemiesToSpawn = numberOfEnemiesToSpawn;
		}
		numberOfEnemiesToSpawn -= numberOfToughEnemiesToSpawn;
		print("spawning:    " + numberOfEnemiesToSpawn + " normal enemies     " + numberOfToughEnemiesToSpawn + " tough enemies");
		SpawnEnemies(numberOfEnemiesToSpawn, _enemies);
		SpawnEnemies(numberOfToughEnemiesToSpawn, _toughEnemies);
	}
	
	private void SpawnEnemies(int numberOfEnemiesToSpawn, string[] enemiesToSpawn)
	{
		int spawnIndex = (int)(Random.value * ((float)_spawnPoints.Length));
		for (int i = 0; i < numberOfEnemiesToSpawn; ++i)
		{
			int index = (int)(Random.value * ((float)enemiesToSpawn.Length));
			
			var deltaPosition = new Vector2(Random.value * 2 - 1, Random.value * 2 - 1);
			deltaPosition *= _maxSpawnRange;
			var spawnPosition = _spawnPoints[spawnIndex].transform.position + new Vector3(deltaPosition.x, 0, deltaPosition.y);
			
			PhotonNetwork.Instantiate(enemiesToSpawn[index], spawnPosition, Quaternion.Euler(0, 0, 0), 0);
		}
		_enemiesLeftToSpawn -= numberOfEnemiesToSpawn;
	}
	
	private void GoToNextWave()
	{
		IsInWave = false;
		++CurrentWave;
		
		LevelLoader.RespawnPlayer();
		
		StartCoroutine(WaitAndStartWave());
	}
	
	private IEnumerator WaitAndStartWave()
	{
		yield return new WaitForSeconds(_breakTimeBetweenWaves);
		
		StartCoroutine("ShowEnemiesApproaching");
		_enemiesLeftToSpawn = CurrentWave * 4;
		IsInWave = true;
	}
	
	private IEnumerator ShowEnemiesApproaching()
	{
		StopCoroutine("ShowWaveComplete");
		_isShowingEnemiesApproaching = true;
		_isShowingWaveComplete = false;
		yield return new WaitForSeconds(2);
		_isShowingEnemiesApproaching = false;
	}
	
	private IEnumerator ShowWaveComplete()
	{
		StopCoroutine("ShowEnemiesApproaching");
		_isShowingWaveComplete = true;
		_isShowingEnemiesApproaching = false;
		yield return new WaitForSeconds(2);
		_isShowingWaveComplete = false;
	}
	
}
