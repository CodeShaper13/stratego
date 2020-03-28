using UnityEngine;
using System.IO;

/// <summary>
/// Static class to manage the player's username.
/// </summary>
public static class Username {

    private static string username;

    /// <summary>
    /// Tries to read the username from the file, returning true if it is found, false if one needs to be picked.
    /// </summary>
    public static bool retrieveUsername() {
        #if UNITY_EDITOR
            Username.username = "EDITOR";
            return true;
        #endif
        Username.username = Username.readUsernameFromFile();
        if(Username.username == null) {
            Username.username = "player" + Random.Range(0, 999);
            return false;
        }
        else {
            return true;
        }
    }

    /// <summary>
    /// Returns the username.
    /// </summary>
    public static string getUsername() {
        return Username.username;
    }

    /// <summary>
    /// Sets the username and writes it to disk.
    /// </summary>
    public static void setUsername(string username) {
        Username.username = username.ToLower();

        File.WriteAllText("username.txt", Username.username);
    }

    /// <summary>
    /// Reads the username from file, returning it, or null if the file can't be found.
    /// </summary>
    /// <returns></returns>
    private static string readUsernameFromFile() {
        string path = "username.txt";
        if(File.Exists(path)) {
            string[] lines = File.ReadAllLines(path);
            if(lines.Length > 0) {
                string s1 = lines[0].Replace(' ', '_');
                if(string.IsNullOrEmpty(s1)) {
                    return null;
                }
                else {
                    return s1;
                }
            }
        }
        return null;
    }
}
