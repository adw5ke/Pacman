using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetPinky : MonoBehaviour {

    /* pinky's target is four tiles in front of pacman unless pacman is facing up;
       if pacman is facing up, then the target is four tiles above and four tiles to the left of pacman */

    public Pacman pacman;
    public Ghost ghost;

    private bool hasMoved = true;    // the target should only move once in scatter mode
    private float xPosition = -7.5f; // temporary x position of the target in scatter mode
    private float yPosition = 10.5f; // temporary x position of the target in scatter mode
    private float zPosition = -7.0f; // determines draw order

    private void Update() {
        SetPosition();
    }

    // set the position of the target
    private void SetPosition() {
        // eaten mode - the target should be directly above the house
        if (this.ghost.ghostEaten.enabled) {
            this.transform.position = new Vector3(0.0f, 2.5f, this.zPosition);

        // chase mode - target's behavior is described above
        } else if (this.ghost.chase.enabled && !this.ghost.scatter.enabled && !this.ghost.ghostEaten.enabled) {
            if (this.pacman.movement.direction == Vector2.left) {
                this.transform.position = new Vector3(this.pacman.transform.position.x - 4.0f, this.pacman.transform.position.y, this.zPosition);
            } else if (this.pacman.movement.direction == Vector2.up) {
                this.transform.position = new Vector3(this.pacman.transform.position.x - 4.0f, this.pacman.transform.position.y + 4.0f, this.zPosition);
            } else if (this.pacman.movement.direction == Vector2.right) {
                this.transform.position = new Vector3(this.pacman.transform.position.x + 4.0f, this.pacman.transform.position.y, this.zPosition);
            } else if (this.pacman.movement.direction == Vector2.down) {
                this.transform.position = new Vector3(this.pacman.transform.position.x, this.pacman.transform.position.y - 4.0f, this.zPosition);
            }

            // reset the target for the next transition to scatter mode
            this.xPosition = -7.5f;
            this.yPosition = 10.5f;
            this.hasMoved = true;

        // scatter mode - force pinky to move counter-clockwise in the top-left corner
        } else if (this.ghost.scatter.enabled && !this.ghost.chase.enabled && !this.ghost.ghostEaten.enabled) {
            
            this.transform.position = new Vector3(xPosition, yPosition, this.zPosition);

            // move the target left
            if (inCircle(this.transform.position, this.ghost.transform.position, 5) && this.hasMoved) {
                this.xPosition = -11.5f;
                this.yPosition = 16.5f;
                this.hasMoved = false;
            }

        // home or frightened mode or pacman has died
        } else {
            this.transform.position = new Vector3(0.0f, 0.0f, this.zPosition);
        }
    }

    // check to see if a point is within a circle of a given radius
    // https://stackoverflow.com/questions/481144/equation-for-testing-if-a-point-is-inside-a-circle
    private bool inCircle(Vector3 origin, Vector3 point, float radius) {
        return (Mathf.Pow(radius, 2) - (Mathf.Pow((origin.x - point.x), 2) + Mathf.Pow((origin.y - point.y), 2))) >= 0;
    }
}
