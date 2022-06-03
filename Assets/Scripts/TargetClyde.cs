using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetClyde : MonoBehaviour {
    
    /* Clyde's target is pacman unless Clyde is within 8 units of pacman;
       in that case, Clyde's target is the same as scatter mode (bottom left corner) */

    public Pacman pacman;
    public Ghost ghost;

    private float zPosition = -9.0f; // draw order - prioritize Clyde's target over Blinky's

    private void Update() {
        SetPosition();
    }

    private void SetPosition() {
        // eaten node
        if (this.ghost.ghostEaten.enabled) {
            this.transform.position = new Vector3(0.0f, 2.5f, this.zPosition);

        // chase mode - if Clyde is within 8 units of pacman, put the target at the bottom left corner
        } else if (this.ghost.chase.enabled && !this.ghost.scatter.enabled && !this.ghost.ghostEaten.enabled) {
            if (inCircle(this.pacman.transform.position, this.ghost.transform.position, 8.0f)) {
                this.transform.position = new Vector3(-13.5f, -17.5f, this.zPosition);

            // otherwise, the target is pacman
            } else {
                this.transform.position = new Vector3(this.pacman.transform.position.x, this.pacman.transform.position.y, this.zPosition);
            }

        // scatter mode - bottom left corner
        } else if (this.ghost.scatter.enabled && !this.ghost.chase.enabled && !this.ghost.ghostEaten.enabled) {
            this.transform.position = new Vector3(-13.5f, -17.5f, this.zPosition);

        } else {
            this.transform.position = new Vector3(0.0f, 0.0f, this.zPosition);
        }
    }

    // check to see if a point is within a circle of a given radius
    // used to see if clyde is within 8 units of pacman
    // source: https://stackoverflow.com/questions/481144/equation-for-testing-if-a-point-is-inside-a-circle
    private bool inCircle(Vector3 origin, Vector3 point, float radius) {
        return (Mathf.Pow(radius, 2) - (Mathf.Pow((origin.x - point.x), 2) + Mathf.Pow((origin.y - point.y), 2))) >= 0;
    }
}
