using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Movement))]

public class Ghost : MonoBehaviour {
    // manage the different references to the ghosts and ghosts' movement
    public Movement movement { get; private set; }
    public GhostHome home { get; private set; }
    public GhostScatter scatter { get; private set; }
    public GhostChase chase { get; private set; }
    public GhostFrightened frightened { get; private set; }
    public GhostEaten ghostEaten { get; private set; }

    public GhostBehavior initialBehavior;
    public GameObject target;  // target object to chase

    public GameManager gameManager;     // used by GhostEaten.cs
    
    public int points = 200;            // used by the Game Manager
    public bool isExitingHouse = false; // TODO: implement this
    public bool isTransition = true;    // keeps track if a ghost is transitioning from eaten mode to the house

    // assign all of the references
    private void Awake() {
        this.gameManager = FindObjectOfType<GameManager>();
        this.movement = GetComponent<Movement>();
        this.home = GetComponent<GhostHome>();
        this.scatter = GetComponent<GhostScatter>();
        this.chase = GetComponent<GhostChase>();
        this.frightened = GetComponent<GhostFrightened>();
        this.ghostEaten = GetComponent<GhostEaten>();
    }

    private void Start() {
        ResetState();
    }

    // reset the state of all the ghost behaviors
    public void ResetState() {
        this.gameObject.SetActive(true);
        this.movement.ResetState();

        this.frightened.Disable();
        this.chase.Disable();
        this.scatter.Enable();
        this.ghostEaten.Disable();

        if (this.home != this.initialBehavior) {
            this.home.Disable();
        }

        if (this.initialBehavior != null) {
            this.initialBehavior.Enable();
        }

        this.home.terminate = false;
    }

    public void SetPosition(Vector3 position) {
        // keep the z-position the same since it determines draw depth
        position.z = this.transform.position.z;
        this.transform.position = position;
    }

    // handle collision between pacman and the ghosts
    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Pacman")) {
            // if the ghost is frightened, eat the ghost (handled by game manager)
            if (this.frightened.enabled) {
                this.gameManager.GhostEaten(this);
            } else if (!this.ghostEaten.enabled) {
                // otherwise, pacman was eaten by the ghost
                this.gameManager.PacmanEaten();
            }
        }
    }
}

