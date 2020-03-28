using System;
using System.Collections.Generic;
using UnityEngine;

public class Slice : MonoBehaviour {

    [SerializeField]
    [Header("The unique ID of the slice.")]
    private int sliceId = -1;
    [SerializeField]
    [Header("The transfrom that a Player's camera should use.")]
    private CameraTransfrom cameraTransfrom;

    private void Awake() {
        if(this.sliceId == -1) {
            throw new Exception("\"sliceId\" field can not be -1");
        }
        if(this.cameraTransfrom == null) {
            throw new Exception("\"cameraTransfrom\" field can not be empty!");
        }
    }

    /// <summary>
    /// Returns a list of all the Cells in this Slice.
    /// </summary>
    public List<Cell> getCells() {
        List<Cell> cells = new List<Cell>();
        foreach(Transform t in this.transform) {
            Cell c = t.GetComponent<Cell>();
            if(c != null) {
                cells.Add(c);
            }
        }
        return cells;
    }

    public void setSliceId(int id) {
        this.sliceId = id;
    }

    public int getSliceId() {
        return this.sliceId;
    }

    /// <summary>
    /// Returns the "center" of this base, where the camera should
    /// be moved to so it looks at this base.
    /// </summary>
    public CameraTransfrom getOrgin() {
        return this.cameraTransfrom;
    }
}
