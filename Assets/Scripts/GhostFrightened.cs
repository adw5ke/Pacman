using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostFrightened : GhostBehavior {

    public SpriteRenderer body;
    public SpriteRenderer eyes;
    public SpriteRenderer blue;
    public SpriteRenderer white;

    public bool eaten { get; private set; }

    // override Enable() to account for multiple power pellets eaten
    public override void Enable(float duration) {
        base.Enable(duration);

        this.body.enabled = false;
        this.eyes.enabled = false;
        this.blue.enabled = true;
        this.white.enabled = false;

        // flash when half the time of the frightened stage has elapsed
        Invoke(nameof(Flash), duration / 2.0f);
    }

    public override void Disable() {
        base.Disable();

        this.body.enabled = true;
        this.eyes.enabled = true;
        this.blue.enabled = false;
        this.white.enabled = false;
    }

    // enable and disable appropriate sprites
    private void Eaten() {
        this.eaten = true;
        // this.ghost.SetPosition(this.ghost.home.inside.position);
        // this.ghost.home.Enable(this.duration);  // ghost must stay at home for at least 'duration'
 
        //this.body.enabled = false;
        //this.eyes.enabled = true;
        //this.blue.enabled = false;
        //this.white.enabled = false;

        // move this logic into GhostEaten and GameManager
    }

    private void Flash() {
        // if eaten, only the eyes will be turned on
        if (!this.eaten && !this.ghost.ghostEaten.enabled && this.ghost.frightened.enabled)  {
            this.blue.enabled = false;
            this.white.enabled = true;
            this.white.GetComponent<AnimatedSprite>().Restart();
        }
    }

    private void OnEnable() {
        this.blue.GetComponent<AnimatedSprite>().Restart();
        this.ghost.movement.SetDirection(-this.ghost.movement.direction); // reverse the ghosts' direction
        this.ghost.movement.speedMultiplier = 0.5f;
        this.eaten = false;
    }

    private void OnDisable() {
        this.ghost.movement.speedMultiplier = 1.0f;
        this.eaten = false;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        Node node = other.GetComponent<Node>();

        if (node != null && this.enabled) {
            // move the ghost in a random direction
            int index = Random.Range(0, node.availableDirections.Count);

            // don't allow the ghost to backtrack
            while (node.availableDirections[index] == -this.ghost.movement.direction) {
                index = Random.Range(0, node.availableDirections.Count);
            }

            this.ghost.movement.SetDirection(node.availableDirections[index]);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Pacman")) {
            if (this.enabled) {
                Eaten();
            }
        }
    }
}
