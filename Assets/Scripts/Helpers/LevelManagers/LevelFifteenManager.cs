using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelFifteenManager : ILevel {

	private GameObject monk;

	private float windTimer = 0.0f;
	private int windForceDirection;
	public float currWindStrength;

	public float xWindMin;
	public float xWindMax;

	public float maxWindTime;
	public float minWindWaitTime;

	public enum WindState{IDLE, WAITING, ACTIVE}
	public WindState windstate = WindState.IDLE;

	//Clouds managing
	public float minSpawnTime = 0.5f;
	public float maxSpawnTime = 2.0f;
	public float currSpawnTime = 0.0f;
	float spawnTimer = 0.0f;
	public List<GameObject> Cloudspool;
	private bool _spawnClouds = false;

	public void ReturnToPool(int index)
	{
		if(index < Cloudspool.Count)
		{
			Cloudspool[index].transform.position = new Vector3(-10,-10,0);
		}
		else
		{
			Debug.LogError("Wrong index to return at the pool");
		}
	}

	public override void InitLevel()
	{
		base.InitLevel();

		windTimer = 0.0f;
		spawnTimer = 0.0f;
		windstate = WindState.WAITING;
		_spawnClouds = true;
	}

	public override void EndLevel()
	{
		base.EndLevel();
		_spawnClouds = false;
	}

	protected override void Start()
	{
        //preaparing the pool
        GameObject[] clouds = GameObject.FindGameObjectsWithTag("BouncerPeg");
        for (int i = 0; i < clouds.Length; i++)
        {
            clouds[i].GetComponent<CloudController>().index = i;
            Cloudspool.Add(clouds[i]);

            //Checking for deadly clouds
        }

        base.Start();

		currSpawnTime = Random.Range(minSpawnTime, maxSpawnTime);
		monk = gameObject.transform.Find("Monk").gameObject;

		windstate = WindState.IDLE;

		windTimer = 0.0f;
		spawnTimer = 0.0f;
		_spawnClouds = false;

	}

	void ApplyWind()
	{
		GameObject[] coins = GameObject.FindGameObjectsWithTag("Coin");

		for (int i = 0; i < coins.Length; i++)
        {
			if(coins[i] != null && !coins[i].GetComponent<CoinMono>().isDeleting)
			{
				coins[i].GetComponent<Rigidbody2D>().AddForce(new Vector3(currWindStrength * windForceDirection * Time.deltaTime, 0,0));
			}
		}
	}

	private bool SpawnCloud()
	{
		foreach (var item in Cloudspool) {
			if(!item.GetComponent<CloudController>().isActive)
			{
				item.GetComponent<CloudController>().SpawnRandomPosition();
				return true;
			}
		}
		return false;
	}

	// Update is called once per frame
	public override void UpdateLevel() {

		//WindManaging
		switch (windstate) {
		case WindState.IDLE:
			break;
		case WindState.WAITING:
			windTimer += Time.deltaTime;
			if(windTimer > minWindWaitTime)
			{
				windstate = WindState.ACTIVE;
				windTimer = 0;
			}
			break;
		case WindState.ACTIVE:
			windTimer += Time.deltaTime;
			ApplyWind();
			if(windTimer > maxWindTime)
			{
				windstate = WindState.WAITING;
				windTimer = 0;
			}
			break;
		default:
			break;
		}
		//Cloud managing
		spawnTimer += Time.deltaTime;
		if(spawnTimer > currSpawnTime && _spawnClouds)
		{
			if(SpawnCloud())
			{
				currSpawnTime = Random.Range(minSpawnTime, maxSpawnTime);
				spawnTimer = 0.0f;
			}
		}
	}

    #region Difficulty setting
    protected override void PrepareEasyLevel()
    {
        xWindMin = 220;
        xWindMax = 300;

        maxWindTime = 8;
        minWindWaitTime = 6;

        minSpawnTime = 0.8f;
        maxSpawnTime = 2.0f;
    }
    protected override void PrepareMediumLevel()
    {
        xWindMin = 280;
        xWindMax = 320;

        maxWindTime = 10;

        minWindWaitTime = 5;

        minSpawnTime = 0.5f;
        maxSpawnTime = 1.5f;

    }
    protected override void PrepareHardLevel()
    {
        xWindMin = 300;
        xWindMax = 320;

        maxWindTime = 13;

        minWindWaitTime = 3;

        minSpawnTime = 0.5f;
        maxSpawnTime = 1.3f;

        foreach (var obj in Cloudspool)
        {
            CoinDestroyer coinDestroyer = obj.GetComponent<CoinDestroyer>();
            if (coinDestroyer != null)
            {
                coinDestroyer.enabled = true;
                obj.transform.Find("Death").gameObject.SetActive(true);
            }
        }

    }
    #endregion Difficulty setting

}
