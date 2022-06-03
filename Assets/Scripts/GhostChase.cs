using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostChase : GhostBehavior {

    // TODO: remove all debugging code

    private void OnEnable() {
        if(!this.ghost.frightened.enabled) {
            // reverse the ghost's direction
            this.ghost.movement.SetDirection(-this.ghost.movement.direction); 
        }
    }

    // on chase disable, switch to scatter
    private void OnDisable() {
        this.ghost.scatter.Enable();
    }

    // on trigger enter for nodes
    private void OnTriggerEnter2D(Collider2D other) {
        Node node = other.GetComponent<Node>();
        // string debugMessage = "CH: D->"+ convert(this.ghost.movement.direction) + "|";

        // chase is enabled and the ghost are not frightened
        if (node != null && this.enabled && !this.ghost.frightened.enabled) {
            // remove the direction that makes the ghost backtrack
            // Vector2 temp = GetDirectionToBeRemoved(node.Regenerate(), this.ghost.movement.direction);
            // Debug.Log(convert(this.ghost.movement.direction) + " <- current | removed -> " + convert(temp));
            // List<Vector2> availableDirections = RemoveOppositeDirection(node.Regenerate(), this.ghost.movement.direction);
            // List<Vector2> availableDirections = node.Regenerate();

            // debugMessage += "AD->(";
            // foreach (Vector2 V in node.availableDirections)
            // {
            //     debugMessage += convert(V) + ",";
            // }
            // debugMessage += ")|L->{";

            Vector2 direction = Vector2.zero;
            float minDistance = float.MaxValue;

            // loop though all direcitons and find the direction that moves the closet to the target
            foreach (Vector2 availableDirection in node.availableDirections) {

                // do not allow the ghost to backtrack
                if (availableDirection != -this.ghost.movement.direction) {
                    // calculate the new position of the ghost if it were to move to this direction
                    Vector3 newPosition = this.transform.position + new Vector3(availableDirection.x, availableDirection.y);

                    // calculate the distance from the new position to the target and see if it is a minimum
                    // use square instead of square root b/c square root funtion is slow
                    float distance = (this.ghost.target.transform.position - newPosition).sqrMagnitude;

                    // debugMessage += convert(availableDirection) + ":" + distance + ",";

                    if (distance < minDistance) {
                        direction = availableDirection;
                        minDistance = distance;
                    }
                }
            }

            // debugMessage += "}|ND->" + convert(direction) + "|";
            // debugMessage += "T->" + this.ghost.target.position + "|G->" + this.ghost.transform.position + "|";
            // debugMessage += "RM->" + convert(temp);

            // assign the movement
            this.ghost.movement.SetDirection(direction);

            // if(this.ghost.name == "Blinky")
            // {
                // Debug.Log(debugMessage);
            // }
        }
    }

    // debug utility method, convert vector to a string
    private string convert(Vector2 direction) {
        string result = "zero";
        if (direction == Vector2.left) {
            result = "left";
        } else if (direction == Vector2.up) {
            result = "up";
        } else if (direction == Vector2.right) {
            result = "right";
        } else if (direction == Vector2.down) {
            result = "down";
        } else if (direction == Vector2.zero) {
            result = "null";
        }
        return result;
    }
}
