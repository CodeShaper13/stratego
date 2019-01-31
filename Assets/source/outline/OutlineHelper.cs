using cakeslice;
using UnityEngine;

public class OutlineHelper {

    private bool outline1;
    private bool outline2;
    private bool outline3;

    private readonly Outline outline;

    public OutlineHelper(GameObject obj) {
        this.outline = obj.GetComponent<Outline>();
    }

    public bool isOutlineVisible(EnumOutlineParam type) {
        switch(type) {
            case EnumOutlineParam.SELECTED:
                return this.outline1;
            case EnumOutlineParam.ACTION_OPTION:
                return this.outline2;
            case EnumOutlineParam.RED:
                return this.outline3;
            case EnumOutlineParam.ANY:
                return this.outline1 || this.outline2 || this.outline3;
        }
        return false;
    }

    public void updateOutline(bool visible, EnumOutlineParam type) {
        switch(type) {
            case EnumOutlineParam.ALL:
                this.outline1 = visible;
                this.outline2 = visible;
                this.outline3 = visible;
                break;
            case EnumOutlineParam.SELECTED:
                this.outline1 = visible;
                break;
            case EnumOutlineParam.ACTION_OPTION:
                this.outline2 = visible;
                break;
            case EnumOutlineParam.RED:
                this.outline3 = visible;
                break;
        }

        // Update the one you can see.
        if(this.outline1) {
            this.outline.color = 0;
            this.outline.enabled = true;
        }
        else if(this.outline2) {
            this.outline.color = 1;
            this.outline.enabled = true;
        }
        else if(this.outline3) {
            this.outline.color = 2;
            this.outline.enabled = true;
        }
        else {
            this.outline.enabled = false;
        }
    }
}