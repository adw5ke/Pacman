using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour {

    public LayerMask obstacleLayer;
    public LayerMask nodeLayer;
    public List<Vector2> availableDirections { get; private set; }
    public Dictionary<Vector2, Vector3> adjacentNodePositions { get; private set; }

    // hard coded dictionary of nodes
    // each key-value pair is the position of the node and a list of available directions at that node
    // TODO: delete this (this was for debugging)
    private IDictionary<Vector2, List<Vector2>> dictionary = new Dictionary<Vector2, List<Vector2>>() {
        {new Vector2(-12.5f,12.5f),  new List<Vector2> {Vector2.right, Vector2.down} },
        {new Vector2(-12.5f,8.5f),   new List<Vector2> {Vector2.up,    Vector2.right, Vector2.down} },
        {new Vector2(-12.5f,5.5f),   new List<Vector2> {Vector2.up,    Vector2.right} },
        {new Vector2(-12.5f,-6.5f),  new List<Vector2> {Vector2.right, Vector2.down} },
        {new Vector2(-12.5f,-9.5f),  new List<Vector2> {Vector2.up,    Vector2.right} },
        {new Vector2(-12.5f,-12.5f), new List<Vector2> {Vector2.right, Vector2.down} },
        {new Vector2(-12.5f,-15.5f), new List<Vector2> {Vector2.up,    Vector2.right} },

        {new Vector2(-10.5f,-9.5f),  new List<Vector2> {Vector2.left, Vector2.down} },
        {new Vector2(-10.5f,-12.5f), new List<Vector2> {Vector2.left, Vector2.up, Vector2.right} },

        {new Vector2(-7.5f,12.5f),  new List<Vector2> {Vector2.left, Vector2.right, Vector2.down} },
        {new Vector2(-7.5f,8.5f),   new List<Vector2> {Vector2.left, Vector2.up,    Vector2.right, Vector2.down} },
        {new Vector2(-7.5f,5.5f),   new List<Vector2> {Vector2.left, Vector2.up,    Vector2.down} },
        {new Vector2(-7.5f,-0.5f),  new List<Vector2> {Vector2.left, Vector2.up,    Vector2.down,  Vector2.left} },
        {new Vector2(-7.5f,-6.5f),  new List<Vector2> {Vector2.left, Vector2.up,    Vector2.down,  Vector2.left} },
        {new Vector2(-7.5f,-9.5f),  new List<Vector2> {Vector2.up,   Vector2.right, Vector2.down} },
        {new Vector2(-7.5f,-12.5f), new List<Vector2> {Vector2.left, Vector2.up} },

        {new Vector2(-4.5f,8.5f),   new List<Vector2> {Vector2.left,  Vector2.right, Vector2.down} },
        {new Vector2(-4.5f,5.5f),   new List<Vector2> {Vector2.up,    Vector2.right} },
        {new Vector2(-4.5f,2.5f),   new List<Vector2> {Vector2.right, Vector2.down } },
        {new Vector2(-4.5f,-0.5f),  new List<Vector2> {Vector2.left,  Vector2.up,    Vector2.down} },
        {new Vector2(-4.5f,-3.5f),  new List<Vector2> {Vector2.up,    Vector2.right, Vector2.down} },
        {new Vector2(-4.5f,-6.5f),  new List<Vector2> {Vector2.left,  Vector2.up,    Vector2.right} },
        {new Vector2(-4.5f,-9.5f),  new List<Vector2> {Vector2.left,  Vector2.right, Vector2.down} },
        {new Vector2(-4.5f,-12.5f), new List<Vector2> {Vector2.up,    Vector2.right} },

        {new Vector2(-1.5f,12.5f),  new List<Vector2> {Vector2.left, Vector2.down} },
        {new Vector2(-1.5f,8.5f),   new List<Vector2> {Vector2.left, Vector2.up, Vector2.right} },
        {new Vector2(-1.5f,5.5f),   new List<Vector2> {Vector2.left, Vector2.down} },
        {new Vector2(-1.5f,2.5f),   new List<Vector2> {Vector2.left, Vector2.up, Vector2.right} },
        {new Vector2(-1.5f,-6.5f),  new List<Vector2> {Vector2.left, Vector2.down} },
        {new Vector2(-1.5f,-9.5f),  new List<Vector2> {Vector2.left, Vector2.up, Vector2.right} },
        {new Vector2(-1.5f,-12.5f), new List<Vector2> {Vector2.left, Vector2.down} },
        {new Vector2(-1.5f,-15.5f), new List<Vector2> {Vector2.left, Vector2.up, Vector2.right} },

        {new Vector2(12.5f,12.5f),  new List<Vector2> {Vector2.left, Vector2.down} },
        {new Vector2(12.5f,8.5f),   new List<Vector2> {Vector2.left, Vector2.up, Vector2.down} },
        {new Vector2(12.5f,5.5f),   new List<Vector2> {Vector2.left, Vector2.up} },
        {new Vector2(12.5f,-6.5f),  new List<Vector2> {Vector2.left, Vector2.down} },
        {new Vector2(12.5f,-9.5f),  new List<Vector2> {Vector2.left, Vector2.up} },
        {new Vector2(12.5f,-12.5f), new List<Vector2> {Vector2.left, Vector2.down} },
        {new Vector2(12.5f,-15.5f), new List<Vector2> {Vector2.left, Vector2.up} },

        {new Vector2(10.5f,-9.5f),  new List<Vector2> {Vector2.right, Vector2.down} },
        {new Vector2(10.5f,-12.5f), new List<Vector2> {Vector2.left, Vector2.up, Vector2.right} },

        {new Vector2(7.5f,12.5f),  new List<Vector2> {Vector2.left, Vector2.right, Vector2.down} },
        {new Vector2(7.5f,8.5f),   new List<Vector2> {Vector2.left, Vector2.up,    Vector2.right, Vector2.down} },
        {new Vector2(7.5f,5.5f),   new List<Vector2> {Vector2.up,   Vector2.right, Vector2.down} },
        {new Vector2(7.5f,-0.5f),  new List<Vector2> {Vector2.left, Vector2.up,    Vector2.right, Vector2.down} },
        {new Vector2(7.5f,-6.5f),  new List<Vector2> {Vector2.left, Vector2.up,    Vector2.right, Vector2.down} },
        {new Vector2(7.5f,-9.5f),  new List<Vector2> {Vector2.left, Vector2.up,    Vector2.down} },
        {new Vector2(7.5f,-12.5f), new List<Vector2> {Vector2.up,   Vector2.right} },

        {new Vector2(4.5f,8.5f),   new List<Vector2> {Vector2.left, Vector2.right, Vector2.down} },
        {new Vector2(4.5f,5.5f),   new List<Vector2> {Vector2.left, Vector2.up} },
        {new Vector2(4.5f,2.5f),   new List<Vector2> {Vector2.left, Vector2.down} },
        {new Vector2(4.5f,-0.5f),  new List<Vector2> {Vector2.up,   Vector2.right, Vector2.down} },
        {new Vector2(4.5f,-3.5f),  new List<Vector2> {Vector2.left, Vector2.up,    Vector2.down} },
        {new Vector2(4.5f,-6.5f),  new List<Vector2> {Vector2.left, Vector2.up,    Vector2.right} },
        {new Vector2(4.5f,-9.5f),  new List<Vector2> {Vector2.left, Vector2.right, Vector2.down} },
        {new Vector2(4.5f,-12.5f), new List<Vector2> {Vector2.left, Vector2.up} },

        {new Vector2(1.5f,12.5f),  new List<Vector2> {Vector2.right, Vector2.down} },
        {new Vector2(1.5f,8.5f),   new List<Vector2> {Vector2.left,  Vector2.up, Vector2.right} },
        {new Vector2(1.5f,5.5f),   new List<Vector2> {Vector2.right, Vector2.down} },
        {new Vector2(1.5f,2.5f),   new List<Vector2> {Vector2.left,  Vector2.up, Vector2.right} },
        {new Vector2(1.5f,-6.5f),  new List<Vector2> {Vector2.right, Vector2.down} },
        {new Vector2(1.5f,-9.5f),  new List<Vector2> {Vector2.left,  Vector2.up, Vector2.right} },
        {new Vector2(1.5f,-12.5f), new List<Vector2> {Vector2.right, Vector2.down} },
        {new Vector2(1.5f,-15.5f), new List<Vector2> {Vector2.left,  Vector2.up, Vector2.right} },
    };



    private void Start() {
        this.availableDirections = new List<Vector2>();
        this.adjacentNodePositions = new Dictionary<Vector2, Vector3>();
        CheckAvailableDirection(Vector2.up);
        CheckAvailableDirection(Vector2.down);
        CheckAvailableDirection(Vector2.left);
        CheckAvailableDirection(Vector2.right);
        SpecialNode();
    }

    // populate availableDirections with the available directions from each node
    // populate adjacentNodePositions with the positions of all nodes adjacent to the current and the direction to get to each node
    private void CheckAvailableDirection(Vector2 direction) {
        // box cast on the walls to determine available directions
        RaycastHit2D hitWall = Physics2D.BoxCast(this.transform.position, Vector2.one * 0.5f, 0.0f, direction, 1.5f, this.obstacleLayer);

        // if no collider is hit ,then there is no obstacle (wall) in that direction
        if (hitWall.collider == null) {
            this.availableDirections.Add(direction);

            if (this.transform.position != new Vector3(-10.5f, -0.5f, 0.0f) && this.transform.position != new Vector3(10.5f, -0.5f, 0.0f)) {
                // box cast on the nodes to determine the positions of the adjacent nodes
                RaycastHit2D hitNode = Physics2D.BoxCast(this.transform.position, Vector2.one * 0.5f, 0.0f, direction, 12.0f, this.nodeLayer);
                this.adjacentNodePositions.Add(direction, hitNode.transform.position);
            }
        }
    }

    // add special values for the two nodes in the tunnels
    private void SpecialNode() {
        if (this.transform.position == new Vector3(-10.5f, -0.5f, 0.0f)) {
            this.adjacentNodePositions.Add(Vector2.left, Vector3.zero);
            this.adjacentNodePositions.Add(Vector2.right, new Vector3(1.0f, 1.0f, 0.0f));
            this.availableDirections.Add(Vector2.left);
            this.availableDirections.Add(Vector2.right);
        } else if (this.transform.position == new Vector3(10.5f, -0.5f, 0.0f)) {
            this.adjacentNodePositions.Add(Vector2.right, Vector3.zero);
            this.adjacentNodePositions.Add(Vector2.left, new Vector3(1.0f, 1.0f, 0.0f));
            this.availableDirections.Add(Vector2.left);
            this.availableDirections.Add(Vector2.right);
        }
    }
}
