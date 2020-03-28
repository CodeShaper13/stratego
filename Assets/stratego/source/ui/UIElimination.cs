using UnityEngine;
using UnityEngine.UI;

public class UIElimination : UIBase {

    [SerializeField]
    private Text reasonText;
    [SerializeField]
    private Text smallText;

    public void setReasonText(string text) {
        this.reasonText.text = text;
    }

    public void setSmallText(string text) {
        this.smallText.text = text;
    }

    public void callback_ok() {
        this.manager.closeCurrent();
    }
}
