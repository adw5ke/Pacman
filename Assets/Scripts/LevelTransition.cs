using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelTransition : MonoBehaviour
{
    private GameManager mainGame;

    private void Start()
    {
        mainGame = GameObject.Find("GameManager").GetComponent<GameManager>();
        mainGame.NewRound();
        SceneManager.LoadScene(1);
    }
}
