using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the list of tips that show up on the waiting screen.
/// </summary>
public class Tips {

    public static Tips instance;

    private List<string> tips;
    private int index;

    public static void initTips() {
        Tips.instance = new Tips();
    }

    private Tips() {
        string s = References.list.tips.text;
        this.tips = new List<string>(s.Split(new string[] { "\r\n", "\n" }, System.StringSplitOptions.RemoveEmptyEntries));
        this.tips.shuffle();

        this.index = Random.Range(0, this.tips.Count - 1);
    }

    /// <summary>
    /// Returns a random tip.
    /// </summary>
    public string getNextTip() {
        string s = this.tips[this.index];

        this.index++;
        if(this.index >= this.tips.Count) {
            this.index = 0;
        }

        return s;
    }
}
