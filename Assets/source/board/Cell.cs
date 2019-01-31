using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Cell : NetworkBehaviour {

    [SyncVar]
    public int cellIndex;
    [SyncVar]
    public bool isWater;
    [SyncVar]
    /// <summary> If this square is in a Team's base, this is the Team's ID.  -1 for non team squares. </summary>
    public int teamID;

    /// <summary> A list of all the adjacent cells that this one connects too. </summary>
    public List<Cell> neighbors;

    private void Start() {
        // Color the Cell.
        Material m = this.func();
        if(m != null) {
            this.GetComponent<MeshRenderer>().material = m;
        }

        // Compute the neighbors.
        List<Cell> list = new List<Cell>();
        foreach(Cell cell in GameObject.FindObjectsOfType<Cell>()) {
            if(cell == this) {
                continue;
            }

            Vector2 v1 = new Vector2(this.transform.position.x, this.transform.position.z);
            Vector2 v2 = new Vector2(cell.transform.position.x, cell.transform.position.z);
            if(Vector2.Distance(v1, v2) <= 0.7f) {
                list.Add(cell);
            }
        }

        this.neighbors = list;
    }

    private Material func() {
        if(this.teamID == Team.RED.getId()) {
            return References.list.groundRed;
        }
        else if(this.teamID == Team.BLUE.getId()) {
            return References.list.groudBlue;
        }
        else if(this.teamID == Team.GREEN.getId()) {
            return References.list.groundGreen;
        }
        else if(this.teamID == Team.YELLOW.getId()) {
            return References.list.groundYellow;
        }
        else if(this.teamID == Team.ORANGE.getId()) {
            return References.list.groundOrange;
        }
        else if(this.teamID == Team.PURPLE.getId()) {
            return References.list.groundPurple;
        }
        return null;
    }

    private void OnDrawGizmos() {
        if(this.isWater) {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(this.getPos(), 0.25f);
        }
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
    /// Returns true if the passed cell is neighbors with this one.
    /// </summary>
    public bool isAdjacent(Cell cell) {
        return this.neighbors.Contains(cell);
    }

    /// <summary>
    /// Returns the position of the cell.
    /// </summary>
    public Vector3 getPos() {
        return this.transform.position;
    }

    /// <summary>
    /// Returns the piece that is in this cell.  Null is returned if it is empty.
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
