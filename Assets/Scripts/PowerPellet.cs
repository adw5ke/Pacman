using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerPellet : Pellet {
    public float duration = 8.0f;

    // override pellet Eat() method
    protected override void Eat() {
        FindObjectOfType<GameManager>().PowerPelletEaten(this);
    }
}
