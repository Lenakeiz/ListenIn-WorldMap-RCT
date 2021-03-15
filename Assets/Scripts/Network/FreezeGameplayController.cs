using UnityEngine;
using System.Collections;

public class FreezeGameplayController : MonoBehaviour
{
    [SerializeField] private GameObject freezeGameUi;
    private bool continueGame = true;

    public void FreezeGameplay(bool shallContinue)
    {
        if (continueGame == shallContinue) return;

        Time.timeScale = shallContinue ? 1 : 0;
        freezeGameUi.SetActive(!shallContinue);
        continueGame = shallContinue;
    }
}
