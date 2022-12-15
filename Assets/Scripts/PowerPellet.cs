using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerPellet : Pellet {
    public float duration = 8.0f; // deprecated after power pellet duration was set based on the level

    // override pellet Eat() method
    protected override void Eat() {
        FindObjectOfType<GameManager>().PowerPelletEaten(this);
    }
}
