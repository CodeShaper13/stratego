using UnityEngine;
using UnityEngine.UI;

public class WaitingUI : MonoBehaviour {

    public Text textTip;
    public Text playersRemainingText;

    private void Awake() {
        this.callback_nextTip();
    }

    public void callback_nextTip() {
        this.textTip.text = Main.singleton.tips.getNextTip();
    }

    public void callback_disconnect() {
        // TODO
    }
}
