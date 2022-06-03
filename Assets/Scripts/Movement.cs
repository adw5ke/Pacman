using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]

public class Movement : MonoBehaviour {

    /* movement class for all game objects (pacman and ghosts) */ 

    public float speed = 8.0f;
    public float speedMultiplier = 1.0f;
    public Vector2 initialDirection;

    // check which layer for collision
    public LayerMask obstacleLayer;

    // hide inherent rigidbody
    public new Rigidbody2D rigidbody { get; private set; }

    // current direction of object
    public Vector2 direction { get; private set; }

    // queue the next movement
    public Vector2 nextDirection { get; private set; }

    public Vector3 startingPosition { get; private set; }

    private void Awake() {
        this.rigidbody = GetComponent<Rigidbody2D>();
        this.startingPosition = this.transform.position;
    }

    private void Start() {
        ResetState();
    }

    // reset state to default values (for the ghosts)
    public void ResetState() {
        this.speedMultiplier = 1.0f;
        this.direction = this.initialDirection;
        this.nextDirection = Vector2.zero;
        this.transform.position = this.startingPosition;
        this.rigidbody.isKinematic = false;  // disable ghosts' ability to move through the wall
        this.enabled = true;
    }

    // continually try to move in the next direction (next direction queued)
    private void Update() {
        if (this.nextDirection != Vector2.zero) {
            // queue the next direction
            SetDirection(this.nextDirection);
        }
    }

    // called automatically at a fixed time interval
    // to correct for differences in frame rate between different machines
    private void FixedUpdate() {
        Vector2 position = this.rigidbody.position;
        Vector2 translation = this.direction * this.speed * this.speedMultiplier * Time.fixedDeltaTime;

        this.rigidbody.MovePosition(position + translation);
    }

    // set the next direction
    public void SetDirection(Vector2 direction, bool forced = false) {
        // forced direction is for the ghosts, not pacman
        // only set the direction if it is a valid move
        if (forced || !Occupied(direction)) {
            this.direction = direction;
            this.nextDirection = Vector2.zero;
        } else {
            // queue up the next direction if the tile is occupied (by a wall)
            this.nextDirection = direction;
        }
    }

    // utility method: determine if we can move in a direction without hitting a wall
    public bool Occupied(Vector2 direction) {
        // box cast from the current object's position, scale by 0.75 to avoid scraping collision
        RaycastHit2D hit = Physics2D.BoxCast(this.transform.position, Vector2.one * 0.75f, 0.0f, direction, 1.5f, this.obstacleLayer);

        // if there is a hit, there will be collider, otherwise null
        return hit.collider != null;
    }
}
