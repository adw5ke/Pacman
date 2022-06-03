using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]

public class Passage : MonoBehaviour {

	/* teleport pacman or ghosts to the opposite tunnel */

    public Transform connection;

    // assign the object's position to the connection's position
    private void OnTriggerEnter2D(Collider2D other) {
        Vector3 position = this.connection.position;
        position.z = other.transform.position.z;
        other.transform.position = position;
    }
}
