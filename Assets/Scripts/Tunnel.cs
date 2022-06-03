using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]

public class Tunnel : MonoBehaviour {
    public Ghost[] ghosts;

    // reduce the ghosts' speed inside of the tunnel
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ghosts")) {
            if (other.gameObject.name == "Blinky") {
                ghosts[0].movement.speedMultiplier = 0.5f;
            } else if (other.gameObject.name == "Inky") {
                ghosts[1].movement.speedMultiplier = 0.5f;
            } else if (other.gameObject.name == "Pinky") {
                ghosts[2].movement.speedMultiplier = 0.5f;
            } else if (other.gameObject.name == "Clyde") {
                ghosts[3].movement.speedMultiplier = 0.5f;
            }
        }
    }

    // restore ghosts' speed on exiting the tunnel
    private void OnTriggerExit2D(Collider2D other) {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ghosts")) {
            if (other.gameObject.name == "Blinky" && !ghosts[0].frightened.enabled) {
                ghosts[0].movement.speedMultiplier = 1.0f;
            } else if (other.gameObject.name == "Inky" && !ghosts[1].frightened.enabled) {
                ghosts[1].movement.speedMultiplier = 1.0f;
            } else if (other.gameObject.name == "Pinky" && !ghosts[2].frightened.enabled) {
                ghosts[2].movement.speedMultiplier = 1.0f;
            } else if (other.gameObject.name == "Clyde" && !ghosts[3].frightened.enabled) {
                ghosts[3].movement.speedMultiplier = 1.0f;
            }
        }
    }
}
