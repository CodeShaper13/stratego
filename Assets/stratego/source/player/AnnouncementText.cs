using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class AnnouncementText : MonoBehaviour {

    private float timer;
    private Text text;

    private void Awake() {
        this.text = this.GetComponent<Text>();
    }

    private void Update() {
        if(this.timer > 0) {
            this.timer -= Time.deltaTime;
            if(this.timer <= 0) {
                this.text.text = string.Empty;
            }
        }
    }

    public void setText(string text, float time) {
        this.timer = time;
        this.text.text = text;
    }
}
