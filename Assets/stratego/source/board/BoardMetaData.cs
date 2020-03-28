using UnityEngine;
using System.Collections.Generic;
using cakeslice;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class BoardMetaData : MonoBehaviour {

    [Header("Board Settings:")]
    public float neighborSearchDistance = 1f;

#if UNITY_EDITOR
    [CustomEditor(typeof(BoardMetaData))]
    public class BoardMetaEditor : Editor {

        public override void OnInspectorGUI() {
            this.serializedObject.UpdateIfRequiredOrScript();

            this.DrawDefaultInspector();

            BoardMetaData myScript = (BoardMetaData)target;
            GUILayout.Space(25);

            if(GUILayout.Button("Generate Board Data")) {
                this.func(myScript);
            }

            this.serializedObject.ApplyModifiedProperties();
        }

        private void func(BoardMetaData boardMetaData) {
            int cellId = 0;
            int i = 0;
            List<Slice> sliceList = new List<Slice>();

            foreach(Transform sliceTransform in boardMetaData.transform) {
                Slice slice = sliceTransform.GetComponent<Slice>();
                if(slice == null) {
                    throw new System.Exception("GameObject with Board component can not have children without the Slice component!");
                }
                sliceList.Add(slice);

                // Set the sliceId
                slice.setSliceId(i);
                i++;

                // Set the name of the slice.
                slice.name = "SLICE_" + slice.getSliceId();

                // Set the Slice.cameraTransfrom field.
                CameraTransfrom camTrans = sliceTransform.GetComponentInChildren<CameraTransfrom>();
                if(camTrans == null) {
                    GameObject o = new GameObject();
                    o.transform.parent = slice.transform;
                    camTrans = o.AddComponent<CameraTransfrom>();
                }
                camTrans.transform.SetSiblingIndex(0);
                camTrans.name = "CameraTransfrom";

                SerializedObject serializedObj = new SerializedObject(slice);
                serializedObj.FindProperty("cameraTransfrom").objectReferenceValue = camTrans;
                serializedObj.ApplyModifiedProperties();

                foreach(Transform child in sliceTransform.transform) {
                    if(child.GetComponent<CameraTransfrom>() != null) {
                        continue;
                    }

                    if(child.GetComponent<MeshFilter>()) {
                        // This must be a cell object, as it has a mesh, add a cell component if it doesn't have one.
                        if(!child.GetComponent<Cell>()) {
                            child.gameObject.AddComponent<Cell>();
                        }

                        // Set the Cell's cellIndex.
                        SerializedObject obj2 = new SerializedObject(child.GetComponent<Cell>());
                        obj2.FindProperty("cellIndex").intValue = cellId++;
                        obj2.ApplyModifiedProperties();

                        child.GetComponent<Outline>().enabled = false;
                    }
                }
            }

            boardMetaData.GetComponent<Board>().sliceArray = sliceList.ToArray();
        }
    }
#endif
}
