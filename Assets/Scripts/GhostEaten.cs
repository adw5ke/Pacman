using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostEaten : GhostBehavior {

    // TODO: remove all debugging code

    public SpriteRenderer body;
    public SpriteRenderer eyes;
    public SpriteRenderer blue;
    public SpriteRenderer white;

    private float speedMultiplier = 3.0f;
    private int ghostLayer = 7;
    private int phaseLayer = 12;

    public Transform inside;
    public Transform outside; // unused
    public Transform InsideLeft;
    public Transform InsideRight;

    // duration should not be considered during eaten mode
    public override void Enable(float duration) {
        this.ghost.chase.enabled = false;
        this.ghost.scatter.enabled = false;
        this.ghost.frightened.enabled = false;
        this.ghost.ghostEaten.enabled = true;

        this.body.enabled = false;
        this.eyes.enabled = true;
        this.blue.enabled = false;
        this.white.enabled = false;

        this.ghost.movement.speedMultiplier = this.speedMultiplier;
        this.ghost.gameObject.layer = this.phaseLayer;     // allow the ghost to pass through pacman
    }

    public override void Disable() {
        base.Disable();
        if (!this.ghost.frightened.enabled) {
            this.body.enabled = true;
            this.eyes.enabled = true;
            this.blue.enabled = false;
            this.white.enabled = false;
        }
        this.ghost.gameObject.layer = this.ghostLayer;
    }

    private void OnEnable() {
        this.ghost.isTransition = true;
    }

    private void OnDisable() {
        // TODO: set duration
        if(this.ghost.name == "Blinky") {
            this.ghost.home.Enable(0.0f);    
        } else {
            this.ghost.home.Enable(8.0f);
        }

        // this.ghost.ghostEaten.enabled = false;
        this.ghost.movement.speedMultiplier = 1.0f;

        // signal to the game manager that ghost eaten has been disabled
        this.ghost.gameManager.SendMessage("DisableGhostEaten");
    }

    private void Update() {
        // transition the ghost into the house if it is eaten
        // target should be set to directly above the house
        if(inCircle(this.ghost.target.transform.position, this.ghost.transform.position, 0.20f) && this.ghost.isTransition) {
            StartCoroutine(TransitionToHouse());
            this.ghost.isTransition = false;
        }
    }

    private IEnumerator TransitionToHouse() {
        // move the ghost into the house once it has reached the outside of the house

        this.ghost.movement.rigidbody.isKinematic = true;
        this.ghost.gameObject.layer = this.ghostLayer; // reset the ghost's layer

        this.ghost.transform.position = new Vector3(0.0f, 2.5f, -1.0f);  // align the ghost's position
        this.ghost.movement.SetDirection(Vector2.down, true);            // move the ghost into the house

        yield return new WaitUntil(() => inCircle(this.inside.position, this.ghost.transform.position, 0.20f));
        this.ghost.movement.rigidbody.isKinematic = false;

        this.ghost.transform.position = new Vector3(0.0f, -0.5f, -1.0f); // set the ghost's position at the center of the house

        if (this.ghost.name == "Inky" || this.ghost.name == "Clyde") {
            if (this.ghost.name == "Inky") {
                // send Inky to the left side of the house
                this.ghost.movement.SetDirection(Vector2.left, true);
                yield return new WaitUntil(() => inCircle(this.InsideLeft.position, this.ghost.transform.position, 0.25f));

                // center Inky 
                this.ghost.transform.position = new Vector3(-2.0f, -0.5f, -1.0f);
            } else {
                // send Clyde to the right side of the house
                this.ghost.movement.SetDirection(Vector2.right, true);
                yield return new WaitUntil(() => inCircle(this.InsideRight.position, this.ghost.transform.position, 0.25f));

                // center Clyde
                this.ghost.transform.position = new Vector3(2.0f, -0.5f, -1.0f);
            }
        }

        // resume ghost home behavior
        this.ghost.movement.SetDirection(Vector2.up, true);
        this.Disable();
    }

    // force the ghost to move back to the house; the target should be directly in front of the house
    private void OnTriggerEnter2D(Collider2D other) {
        // message for debugging purposes
        string message = this.ghost.name + ": D->";
        Node node = other.GetComponent<Node>();

        if (node != null && this.enabled) {

            // save the ghost's direction
            Vector2 tempDirection = this.ghost.movement.direction;

            // setting the ghost's position to the node's position ensure that we are able to 
            // set the next direction every time (instead of queuing it)
            this.ghost.transform.position = node.transform.position;
            this.ghost.movement.SetDirection(Vector2.zero);

            Vector2 direction = Vector2.zero;
            float minDistance = float.MaxValue;

            message += convert(this.ghost.movement.direction) + " | AD->(";
            // foreach(Vector2 V in node.availableDirections)
            // {
            //     message += convert(V) + ",";
            // }
            // message += ") | L->{ ";

            // find the direction that moves closest to the target
            foreach (Vector2 availableDirection in node.availableDirections) {
                // don't allow backtracking
                if (availableDirection != -tempDirection) {
                    // Vector3 newPosition = this.transform.position + new Vector3(availableDirection.x, availableDirection.y);

                    Vector3 newPosition = node.adjacentNodePositions[availableDirection];
                    float distance = (this.ghost.target.transform.position - newPosition).sqrMagnitude;

                    // Debug.Log(this.transform.position + " + " + new Vector3(availableDirection.x, availableDirection.y) + " = " + newPosition);
                    // Debug.Log(this.ghost.target.position + " - " + newPosition + " -> sqrMag = " + distance);

                    if (distance < minDistance) {
                        direction = availableDirection;
                        minDistance = distance;
                    }
                    // message += convert(availableDirection) + ": " + distance + ", ";
                }
            }
            // message += "} | ND->" + convert(direction) + " | T->";
            // message += this.ghost.target.transform.position + " | G->" + this.ghost.transform.position + " | RM->" + convert(-this.ghost.movement.direction);
            this.ghost.movement.SetDirection(direction);
            // message += " | CONFIRM->" + convert(this.ghost.movement.direction);
            // Debug.Log(message);
        }
    }

    // utility method: check to see if a point is within a circle of a given radius
    // source: https://stackoverflow.com/questions/481144/equation-for-testing-if-a-point-is-inside-a-circle
    private bool inCircle(Vector3 origin, Vector3 point, float radius) {
        return (Mathf.Pow(radius, 2) - (Mathf.Pow((origin.x - point.x), 2) + Mathf.Pow((origin.y - point.y), 2))) >= 0;
    }

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
