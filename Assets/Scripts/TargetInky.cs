using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetInky : MonoBehaviour {

    /* Inky's target's position can be found as follows: 
       find the tile that is two units in front of pacman
       if pacman is facin up, then find the tile that is two units above and two to the left of pacman instead
       draw a vector from this tile to blinky, then rotate the vector 180 degrees
       then endpoint of the vector is the position of the target */

    public Pacman pacman;
    public Ghost blinky;
    public Ghost ghost;

    private float zPosition = -7.0f;

    private void Update() {
        SetPosition();
    }

    private void SetPosition() {
        Vector3 intermediate = new Vector3(); // the intermediate point (2 units in front of pacman)

        // eaten mode - the target should be directly above the house
        if (this.ghost.ghostEaten.enabled) {
            this.transform.position = new Vector3(0.0f, 2.5f, this.zPosition);

        // chase mode - target's behavior is described above
        } else if (this.ghost.chase.enabled && !this.ghost.scatter.enabled && !this.ghost.ghostEaten.enabled) {
            if (this.pacman.movement.direction == Vector2.left) {
                intermediate = new Vector3(this.pacman.transform.position.x - 2.0f, this.pacman.transform.position.y, this.zPosition);
            } else if (this.pacman.movement.direction == Vector2.up) {
                intermediate = new Vector3(this.pacman.transform.position.x - 2.0f, this.pacman.transform.position.y + 2.0f, this.zPosition);
            } else if (this.pacman.movement.direction == Vector2.right) {
                intermediate = new Vector3(this.pacman.transform.position.x + 2.0f, this.pacman.transform.position.y, this.zPosition);
            } else if (this.pacman.movement.direction == Vector2.down) {
                intermediate = new Vector3(this.pacman.transform.position.x, this.pacman.transform.position.y - 2.0f, this.zPosition);
            }

            // create a vector and rotate the it 180 degrees around the intermediate point
            Vector3 vector = rotateAroundOrigin(intermediate, this.blinky.transform.position, 180 * Mathf.Deg2Rad);

            // set the target's new position
            this.transform.position = new Vector3(vector.x, vector.y, this.zPosition);


        // scatter mode - target should be in the bottom right corner
        // Inky can move around the bottom section either clockwise or counter-clockwise    
        } else if (this.ghost.scatter.enabled && !this.ghost.chase.enabled && !this.ghost.ghostEaten.enabled) {
            this.transform.position = new Vector3(13.5f, -17.5f, this.zPosition);

        } else {
            this.transform.position = new Vector3(0.0f, 0.0f, this.zPosition);
        }
    }

    // rotate 'point' 'angle' degrees around the 'origin'
    // source?: https://stackoverflow.com/questions/34372480/rotate-point-about-another-point-in-degrees-python
    private Vector3 rotateAroundOrigin(Vector3 origin, Vector3 point, float angle) {
        Vector3 result = new Vector3();
        result.x = origin.x + Mathf.Cos(angle) * (point.x - origin.x) - Mathf.Sin(angle) * (point.y - origin.y);
        result.y = origin.y + Mathf.Sin(angle) * (point.x - origin.x) + Mathf.Cos(angle) * (point.y - origin.y);
        result.z = point.z;
        return result;
    }
}
