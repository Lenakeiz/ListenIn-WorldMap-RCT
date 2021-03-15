using UnityEngine;
using System.Collections;

public class LevelTwoManager : ILevel {

	private GameObject spaceship;

	public override void InitLevel()
	{
		base.InitLevel();

		SplineController sc = null;

		sc = spaceship.GetComponent<SplineController>() as SplineController;
		sc.AutoStart = true;
		sc.UseRigidBody = false;
		sc.UseScaling = true;
		sc.ExecuteMotion();

	}
	
	public override void EndLevel()
	{
		base.EndLevel();

		spaceship.SetActive(false);
	}
    
    protected override void Start()
	{
        base.Start();
		spaceship =  gameObject.transform.Find("Spaceship").gameObject;
	}
	
	// Update is called once per frame
	public override void UpdateLevel() {

	}

    #region Difficulty setting
    protected override void PrepareEasyLevel()
    {
        base.PrepareEasyLevel();
    }
    protected override void PrepareMediumLevel()
    {
        base.PrepareMediumLevel();
    }
    protected override void PrepareHardLevel()
    {
        base.PrepareHardLevel();
    }
    #endregion Difficulty setting
}
