using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]

public class Pellet : MonoBehaviour {
    
    public int points = 10;

    // method is able to be overwritten by power pellet
    protected virtual void Eat() {
        // eat the current pellet; handled by the game manager
        FindObjectOfType<GameManager>().PelletEaten(this);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.layer == LayerMask.NameToLayer("Pacman") && other is CircleCollider2D) {
            Eat();
        }
    }
}
