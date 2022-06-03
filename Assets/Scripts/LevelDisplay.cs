using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelDisplay : MonoBehaviour {

    public int level { get; private set; }

    private void Start() {
        this.level = FindObjectOfType<GameManager>().level;
    }

    private void Update() {
        this.level = FindObjectOfType<GameManager>().level;
        Display();
    }

    // display the appropriate fruits to indicate the level (adjust the fruit displayed and the position of the game object)
    // if level < 8      --> display fruits[0:level]
    // if 7 < level < 19 --> display fruits[level-7:level]
    // if 18 < level     --> display fruits[13:20]
    // also adjust the position of the display on the screen
    private void Display() {
        for(int i = 0; i < 20; i++) {
            if (this.level < 8) {
                if (i < this.level) {
                    this.transform.GetChild(i).gameObject.SetActive(true);
                } else {
                    this.transform.GetChild(i).gameObject.SetActive(false);
                }
                this.transform.position = new Vector3(5, -18, -2);
            } else if (7 < this.level && this.level < 19) {
                if (this.level - 8 < i && i < this.level) {
                    this.transform.GetChild(i).gameObject.SetActive(true);
                } else {
                    this.transform.GetChild(i).gameObject.SetActive(false);
                }
                this.transform.position = new Vector3(5 + (2 * (this.level - 7)), -18, -2);
            } else {
                if (11 < i && i < 19) {
                    this.transform.GetChild(i).gameObject.SetActive(true);
                } else {
                    this.transform.GetChild(i).gameObject.SetActive(false);
                }
                this.transform.position = new Vector3(29, -18, -2);
            }
        }
    }
}
