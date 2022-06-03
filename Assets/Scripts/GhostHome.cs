using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostHome : GhostBehavior {

    public Transform inside;  // transform inside of the house
    public Transform outside; // transform outside of the house

    private void OnEnable() {
        StopAllCoroutines();
    }

    private void OnDisable() {
        // check for active self to prevent error when object is destroyed
        if (this.gameObject.activeSelf) {
            StartCoroutine(ExitTransition());
        }
    }

    // ghosts move up and down in the house
    private void OnCollisionEnter2D(Collision2D collision) {
        // reverse direction everytime the ghost hits a wall
        if (this.enabled && collision.gameObject.layer == LayerMask.NameToLayer("Walls")) {
            this.ghost.movement.SetDirection(-this.ghost.movement.direction);
        }
    }

    // ghost exits the house
    private IEnumerator ExitTransition() {
        this.ghost.isExitingHouse = true;
        // turn off movement while we manually animate the position
        // force the movement up through the house wall
        this.ghost.movement.SetDirection(Vector2.up, true);

        // turn off collision during transition
        this.ghost.movement.rigidbody.isKinematic = true;
        this.ghost.movement.enabled = false;

        // current position of the ghost
        Vector3 position = this.transform.position;

        // duration of transition
        float duration = 0.5f;

        // time elasped
        float elapsed = 0.0f;

        // animate to the starting point
        while (elapsed < duration) {
            // interpolate from current position and inside position by the percentage of time
            // once the percentage is 1 (elapsed / duration), the ghost was fully transitioned
            this.ghost.SetPosition(Vector3.Lerp(position, this.inside.position, elapsed / duration));
            elapsed += Time.deltaTime;

            // wait a frame, then continue
            yield return null;
        }

        // start a brand new animation
        elapsed = 0.0f;

        // animate ghost exiting the house
        while (elapsed < duration) {
            this.ghost.SetPosition(Vector3.Lerp(this.inside.position, this.outside.position, elapsed / duration));
            elapsed += Time.deltaTime;
            yield return null;
        }

        // pick a random direction left or right and re-enable movement once the ghost leaves the house
        this.ghost.movement.SetDirection(new Vector2(Random.value < 0.5f ? -1.0f : 1.0f, 0.0f), true);
        this.ghost.movement.rigidbody.isKinematic = false;
        this.ghost.movement.enabled = true;
        this.ghost.isExitingHouse = false;
        // this.ghost.ghostEaten.Disable(); // TODO: is this correct?
    }
}
