using System;
using UnityEngine;

public class Slice : MonoBehaviour {

    [SerializeField]
    private int sliceIndex;
    [SerializeField]
    private Transform spawn;

    private void Awake() {
        if(this.spawn == null) {
            throw new Exception("Spawn field must be set in the inspector!");
        }
    }

    public int getSliceIndex() {
        return this.sliceIndex;
    }

    public Transform getOrgin() {
        return this.spawn;
    }
}
