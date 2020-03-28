using UnityEngine;

public static class TextColorer {

    /// <summary>
    /// Returns the letter as a colored string.
    /// </summary>
    public static string getColoredText(Team team, string text) {
        Color c = References.getMaterialFromTeam(team).color;
        return "<color=#" + ColorUtility.ToHtmlStringRGBA(c) + ">" + text + "</color>";
    }
}
