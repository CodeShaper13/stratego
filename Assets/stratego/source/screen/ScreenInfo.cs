using UnityEngine;
using UnityEngine.UI;

public class ScreenInfo : ScreenBase {

    [SerializeField]
    private Text msgText;
    [SerializeField]
    private Text btnText;
    private LanSearchText lst;

    public override void onAwake() {
        this.lst = this.msgText.gameObject.GetComponent<LanSearchText>();
        this.lst.enabled = false;
    }

    public void setMessage(string msg, string btnText, bool appendDots = false) {
        this.msgText.text = msg;
        this.btnText.text = btnText;
        this.lst.enabled = appendDots;
    }

    public override bool showPhotographBackground() {
        return true;
    }

    /*
    // Will this close the connection ok? 
     
    public override void onEscape() {
        this.screenManager.showScreen(this.screenManager.screenMainMenu);
    }
    */
}
