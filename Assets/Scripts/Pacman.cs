using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Movement))]

public class Pacman : MonoBehaviour 
{
    public AnimatedSprite deathSequence;
    public new AnimatedSprite animation;

    public SpriteRenderer spriteRenderer { get; private set; }
    public new Collider2D collider { get; private set; }
    public Movement movement { get; private set; }

    private EdgeCollider2D front;

    public bool isKeysEnabled = true;   // control whether or not the player can move pacman

    private void Awake() {
        this.animation = GetComponent<AnimatedSprite>();
        this.spriteRenderer = GetComponent<SpriteRenderer>();
        this.collider = GetComponent<Collider2D>();
        this.movement = GetComponent<Movement>();
        this.front = GetComponent<EdgeCollider2D>();

        this.animation.advance = 0;
    }

    private void Update() {
        if(isKeysEnabled) {
            // set the new direction based on the current input
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) {
                this.movement.SetDirection(Vector2.up);
            } else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) {
                this.movement.SetDirection(Vector2.down);
            } else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) {
                this.movement.SetDirection(Vector2.left);
            } else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) {
                this.movement.SetDirection(Vector2.right);
            }

            // get the angle of the direction we are currently moving in
            float angle = Mathf.Atan2(this.movement.direction.y, this.movement.direction.x);

            // rotate pacman around the z axis by 'angle' degrees so that pacman faces whatever direction he is moving it
            this.transform.rotation = Quaternion.AngleAxis(angle * Mathf.Rad2Deg, Vector3.forward);
        }
    }

    public void ResetState() {
        this.enabled = true;
        this.spriteRenderer.enabled = true;
        this.collider.enabled = true;
        this.deathSequence.enabled = false;
        this.deathSequence.spriteRenderer.enabled = false;
        this.movement.ResetState();
        this.gameObject.SetActive(true);
        this.isKeysEnabled = true;
        this.animation.advance = 1;
        this.movement.speed = 8.0f;
    }

    public void DeathSequence() {
        this.enabled = false;
        this.spriteRenderer.enabled = false;
        this.collider.enabled = false;
        this.movement.enabled = false;
        this.deathSequence.enabled = true;
        this.deathSequence.spriteRenderer.enabled = true;
        this.deathSequence.Restart();
    }

    // rotate pacman to face up during the death sequence
    public void RotateUp() {
        // force direction up
        this.movement.SetDirection(Vector2.up, true);

        // rotate pacman up
        float angle = Mathf.Atan2(this.movement.direction.y, this.movement.direction.x);
        this.transform.rotation = Quaternion.AngleAxis(angle * Mathf.Rad2Deg, Vector3.forward);

        // set direction to zero
        this.movement.SetDirection(Vector2.zero, true);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.name == "Walls") {
            this.animation.advance = 0;
            // this.spriteRenderer.color = new Color(1f, 0f, 0f, 1f);
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.gameObject.name == "Walls") {
            this.animation.advance = 1;
            // this.spriteRenderer.color = new Color(0.14118f, 0.78039f, 0f, 1f);
        }
    }
}

