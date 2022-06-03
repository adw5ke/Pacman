using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostScatter : GhostBehavior {

    private void OnEnable() {
        if (!this.ghost.frightened.enabled) {
            this.ghost.movement.SetDirection(-this.ghost.movement.direction); // reverse the ghost's direction
        }
    }

    // transition from scatter to chase mode
    private void OnDisable() {
        this.ghost.chase.Enable();
    }

    private void OnTriggerEnter2D(Collider2D other) {

        Node node = other.GetComponent<Node>();

        if (node != null && this.enabled && !this.ghost.frightened.enabled) {

            Vector2 direction = Vector2.zero;
            float minDistance = float.MaxValue;

            // loop though all direcitons and find the direction that moves the closet to the target
            foreach (Vector2 availableDirection in node.availableDirections) {
                if (availableDirection != -this.ghost.movement.direction) {
                    // calculate the new position of the ghost if it were to move to this direction
                    Vector3 newPosition = this.transform.position + new Vector3(availableDirection.x, availableDirection.y);

                    // calculate the distance from the new position to the target and see if it is a minimum
                    // use square instead of square root b/c square root funtion is slow
                    float distance = (this.ghost.target.transform.position - newPosition).sqrMagnitude;

                    if (distance < minDistance) {
                        direction = availableDirection;
                        minDistance = distance;
                    }
                }
            }

            // set the ghost's direction
            this.ghost.movement.SetDirection(direction);
        }
    }
}
