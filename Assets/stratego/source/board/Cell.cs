using cakeslice;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Note:  None of the Data is synced, as this class does not extend NetworkedBehaviour.
/// </summary>
[RequireComponent(typeof(Outline))]
public class Cell : MonoBehaviour {

    /// <summary> The index of this cell, every cell has a unique one. </summary>
    [SerializeField]
    public int cellIndex;
    public bool isWater;
    /// <summary> If ture, then the local Player own this Cell.  Only true for clients and NOT synced. </summary>
    public bool ownedByLocalPlayer;

    /// <summary> A list of all the adjacent cells that this one connects too. </summary>
    private List<Cell> neighbors;

    private void Awake() {
        this.neighbors = this.computeNeighbors();
        if(this.isWater) {
            this.GetComponent<MeshRenderer>().material = References.list.groundWater;
        }
    }

    private void Start() {
        // Spread out the Cells so it looks better.
        this.transform.localPosition = this.transform.localPosition * 1.1f;

        // Set the Cell's material.
    }

    /// <summary>
    /// Finds all of the neighbors of this Cell and returns them in a list.
    /// </summary>
    private List<Cell> computeNeighbors() {
        BoardMetaData b = this.GetComponentInParent<BoardMetaData>();
        List<Cell> neighbors = new List<Cell>();
        foreach(Cell cell in GameObject.FindObjectsOfType<Cell>()) {
            if(cell == this) {
                continue;
            }

            Vector2 v1 = new Vector2(this.transform.position.x, this.transform.position.z);
            Vector2 v2 = new Vector2(cell.transform.position.x, cell.transform.position.z);
            if(Vector2.Distance(v1, v2) <= b.neighborSearchDistance) {
                neighbors.Add(cell);
            }
        }
        return neighbors;
    }

    private void OnDrawGizmos() {
        if(this.isWater) {
            Gizmos.color = Color.blue;
        }
        Gizmos.DrawSphere(this.getPos(), 0.15f);
    }

    private void OnDrawGizmosSelected() {
        if(this.neighbors != null) {
            Gizmos.color = Color.yellow;
            foreach(Cell cell in this.neighbors) {
                Gizmos.DrawLine(this.getPos(), cell.getPos());
            }
        }
    }

    /// <summary>
    /// Marks the cell as owned by the local Player.
    /// This also colors the Cell with the Player's color.
    /// </summary>
    public void setOwnedByLocal(Team team) {
        this.ownedByLocalPlayer = true;

        Color cellColor = References.getMaterialFromTeam(team).color * 0.4f;
        this.GetComponent<MeshRenderer>().material.color = cellColor;
    }

    /// <summary>
    /// Returns true if the passed cell is neighbors with this one.
    /// </summary>
    public bool isAdjacent(Cell cell) {
        return this.neighbors.Contains(cell);
    }

    /// <summary>
    /// Returns the position of the cell as a Vector3.
    /// </summary>
    public Vector3 getPos() {
        return this.transform.position;
    }

    /// <summary>
    /// Returns a list of the Cell's neighbors.
    /// </summary>
    public List<Cell> getNeighbors() {
        return this.neighbors;
    }

    /// <summary>
    /// Returns the Piece that is in this cell.  Null is returned if the cell is empty.
    /// </summary>
    public Piece getCurrentPiece() {
        RaycastHit hit;
        if(Physics.Raycast(this.transform.position + Vector3.up, Vector3.down, out hit, float.PositiveInfinity, Layers.PIECE)) {
            return hit.transform.GetComponent<Piece>();
        } else {
            return null;
        }
    }
}
