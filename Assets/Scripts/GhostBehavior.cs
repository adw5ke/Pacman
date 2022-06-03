using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Ghost))]

public abstract class GhostBehavior : MonoBehaviour {

    /* base class for managing ghost behavior by enabling/disabling different behaviors */ 
    
    public Ghost ghost { get; private set; }

    // default duration
    public float duration;

    private void Awake() {
        this.ghost = GetComponent<Ghost>();
    }

    public void Enable() {
        Enable(this.duration);
    }

    public virtual void Enable(float duration) {
        this.enabled = true;

        // if you eat another power pellet while one is already active, cancel invoke
        CancelInvoke();
        Invoke(nameof(Disable), duration);
    }

    public virtual void Disable() {
        this.enabled = false;
        CancelInvoke();
    }
}
