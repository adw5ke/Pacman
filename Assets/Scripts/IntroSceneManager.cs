using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class IntroSceneManager : MonoBehaviour {

    /* script to play the intro scene */

    // audio source is handled in the editor
    public GameObject ghosts;
    public GameObject lifeDisplay;
    public Image pacman;
    public Text playerOne;
    public Text ready;
    public Text oneUp;
    public Text highScoreText;

    private bool toggle = false; // toggle the 1up text

    // private void Awake() {
    //     DontDestroyOnLoad(this);
    // }

    private void Start() {
        InvokeRepeating("ToggleOneUp", 0.125f, 0.25f); ;
        this.highScoreText.text = HighScore.score.ToString().PadLeft(2, '0');
        StartCoroutine(IntroScene());
    }

    private IEnumerator IntroScene() {
        yield return new WaitForSeconds(2.3f);
        playerOne.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.1f);
        ghosts.SetActive(true);
        pacman.gameObject.SetActive(true);
        lifeDisplay.transform.GetChild(4).gameObject.SetActive(false);
        yield return new WaitForSeconds(1.6f);
        SceneManager.LoadScene(1);
    }

    private void ToggleOneUp() {
        oneUp.gameObject.SetActive(toggle);
        this.toggle = !this.toggle;
    }
}
