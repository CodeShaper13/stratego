using System.Collections.Generic;
using UnityEngine;

public class Tips {

    private List<string> tips;
    private int index;

    public Tips() {
        string s = References.list.tips.text;
        this.tips = new List<string>(s.Split(new string[] { "\r\n", "\n" }, System.StringSplitOptions.RemoveEmptyEntries));
        this.tips.shuffle();

        this.index = Random.Range(0, this.tips.Count - 1);
    }

    public string getNextTip() {
        string s = this.tips[this.index];

        this.index++;
        if(this.index >= this.tips.Count) {
            this.index = 0;
        }

        return s;
    }
}
