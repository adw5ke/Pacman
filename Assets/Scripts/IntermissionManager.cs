using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntermissionManager : MonoBehaviour {
    public Pacman pacman;
    public Pacman bigPacman;
    public Ghost blinky;
    public Sound intermission;
    
    void Start() {
        StartCoroutine(IntermissionScene());
    }

    
    void Update() {
        // if(this.pacman.transform.position.x < -14) {
        //     Debug.Log("Reached the end");
        // }
    }

    private IEnumerator IntermissionScene() {
        // this.pacman.isKeysEnabled = false;
        this.pacman.ResetState();
        this.pacman.movement.speed = 8.0f;
        this.pacman.movement.SetDirection(Vector2.left);
        this.pacman.isKeysEnabled = false;
        this.bigPacman.gameObject.SetActive(false);
        this.blinky.movement.speed = 8.35f;
        this.blinky.movement.SetDirection(Vector2.left);
        yield return new WaitUntil(() => this.pacman.transform.position.x < -18);
        this.pacman.gameObject.SetActive(false);
        this.blinky.frightened.Enable(20.0f);
        this.blinky.movement.SetDirection(Vector2.zero);
        yield return new WaitForSeconds(1.0f);
        this.blinky.movement.speedMultiplier = 1.0f;
        this.blinky.movement.SetDirection(Vector2.right);
    }
}
