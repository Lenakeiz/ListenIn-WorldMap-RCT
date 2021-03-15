using UnityEngine;
using System.Collections;

public class LevelNineManager : ILevel {

	private GameObject[] donuts;

	public override void InitLevel()
	{
		base.InitLevel();

		SplineController sc = null;
		for (int i = 0; i < donuts.Length; i++) {
			sc = donuts[i].GetComponent<SplineController>() as SplineController;

            if (sc != null)
            {
                sc.AutoStart = true;
                sc.UseRigidBody = false;
                sc.ExecuteMotion();
            }
		}
	}

	public override void EndLevel()
	{
		base.EndLevel();

		for (int i = 0; i < donuts.Length; i++) {
			donuts[i].SetActive(false);
		}
	}

	protected override void Start()
	{
        base.Start();
        donuts = GameObject.FindGameObjectsWithTag("BouncerPeg");        
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
