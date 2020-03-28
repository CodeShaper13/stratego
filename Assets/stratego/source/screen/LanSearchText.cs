using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class LanSearchText : MonoBehaviour {

    private string cachedText;
    private Text textField;

    public bool cacheText;

    private void Awake() {
        this.textField = this.GetComponent<Text>();

        if(this.cacheText) {
            this.cachedText = this.textField.text;
        }
    }

    private void LateUpdate() {
        int i = (int)Time.time;
        int j = i % 4;
        string dots = string.Empty;
        for(int k = 0; k < j; k++) {
            dots += " .";
        }

        string s2;
        if(this.cacheText) {
            s2 = this.cachedText + dots;
        } else {
            s2 = this.textField.text;
            for(int l = 0; l < 3; l++) {
                s2 = s2.Trim('.').Trim(' ');
            }
            s2 += dots;
        }
        this.textField.text = s2;
    }
}
