using UnityEngine;
using UnityEngine.Networking;
using System;
using System.IO;

public class Main : MonoBehaviour {

    public static Main singleton;
    
    /// <summary> 0 = pick username, 1 = main, 2 = new game, 4 = join game </summary>
    public int state;
    private NetworkDiscovery discovery;

    private string username;
    public Tips tips;

    private void Awake() {
        Main.singleton = this;

        this.tips = new Tips();

        NetworkTransport.Init();

        this.discovery = GameObject.FindObjectOfType<NetworkDiscovery>();

        this.username = this.readUsernameFromFile();
        if(this.username == null) {
            this.state = 0;
            this.username = "Player" + UnityEngine.Random.Range(0, 999);
        }
        else {
            this.state = 1;
        }
    }

    private void OnGUI() {
        switch(this.state) {
            case 0:
                this.drawPickUsername();
                break;
            case 1:
                this.drawMain();
                break;
            case 2:
                this.drawNewMap();
                break;
            case 3:
                break;
            case 4:
                this.drawJoinGame();
                break;
        }
    }

    private void drawPickUsername() {
        GUI.Label(new Rect(20, 20, 400, 24), "Select a Username");
        this.username = GUI.TextField(new Rect(20, 60, 200, 24), this.username);
        if(!string.IsNullOrEmpty(this.username)) {
            if(GUI.Button(new Rect(20, 100, 100, 100), "OK")) {
                this.state = 1;

                // Save username to file.
                File.WriteAllText("username.txt", this.username);
            }
        }
    }

    private void drawMain() {
        float w = 250;
        float h = 80;

        if(GUI.Button(new Rect(20, 20, w, h), "New Game")) {
            this.state = 2;
        }

        bool flag = File.Exists("save.nbt");
        GUI.enabled = flag;
        if(GUI.Button(new Rect(20, 120, w, h), "Continue Game")) {
            // TODO load game
        }
        GUI.enabled = true;

        if(GUI.Button(new Rect(20, 220, w, h), "Join Game")) {
            this.discovery.Initialize();
            this.discovery.StartAsClient();
            this.state = 4;
        }
        if(GUI.Button(new Rect(20, 320, w, h), "Exit")) {
            Application.Quit();
        }
    }

    private void drawNewMap() {
        float w = 250;
        float h = 20;

        GUI.Label(new Rect(20, 20, w, h), "Player Count: " +  GameOptions.maxPlayers);
        int i = Mathf.RoundToInt(GUI.HorizontalSlider(new Rect(20, 40, w, h), GameOptions.maxPlayers, 2, 5));
        if(i == 5) {
            i = 6; // Cant have 5 players, only 4 and 6.
        }
        GameOptions.maxPlayers = i;

        GameOptions.amphibiousFives = GUI.Toggle(new Rect(20, 80, w, h), GameOptions.amphibiousFives, "Amphibious 5s");
        GameOptions.spyKillAll = GUI.Toggle(new Rect(20, 100, w, h), GameOptions.spyKillAll, "Spy Kill All");
        GameOptions.nineLongMove = GUI.Toggle(new Rect(20, 120, w, h), GameOptions.nineLongMove, "Nine Long Move");

        if(GUI.Button(new Rect(20, 200, w, h), "Start Game")) {
            this.state = -1;
            NetworkManager.singleton.StartHost();
        }
        this.drawBackBtn();
    }

    private static string BytesToString(byte[] bytes) {
        char[] chars = new char[bytes.Length / sizeof(char)];
        Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
        return new string(chars);
    }

    private void drawJoinGame() {
        GUI.Label(new Rect(20, 20, 100, 100), "Searching for a LAN game...");

        int xpos = 150;
        int ypos = 10;
        if(this.discovery.broadcastsReceived != null) {
            foreach(var addr in this.discovery.broadcastsReceived.Keys) {
                var value = this.discovery.broadcastsReceived[addr];
                if(GUI.Button(new Rect(xpos, ypos + 20, 200, 20), "Game at " + addr) && true) {
                    string dataString = BytesToString(value.broadcastData);
                    string[] items = dataString.Split(':');

                    if(items.Length == 3) { // && items[0] == "NetworkManager") { // TODO should we include this?
                        if(NetworkManager.singleton != null && NetworkManager.singleton.client == null) {
                            NetworkManager mng = NetworkManager.singleton;
                            mng.networkAddress = items[1];
                            mng.networkPort = Convert.ToInt32(items[2]);
                            mng.StartClient();

                            this.state = -1;
                        }
                    }
                }
                ypos += 24;
            }
        }

        if(this.drawBackBtn()) {
            this.discovery.StopBroadcast();
        }
    }

    private bool drawBackBtn() {
        bool flag = GUI.Button(new Rect(20, 400, 300, 80), "Back");
        if(flag) {
            this.state = 1;
        }
        return flag;
    }

    /// <summary>
    /// Reads the username from file, returning it, or null if the file can't be found.
    /// </summary>
    /// <returns></returns>
    private string readUsernameFromFile() {
        string path = "username.txt";
        if(File.Exists(path)) {
            string[] lines = File.ReadAllLines(path);
            if(lines.Length > 0) {
                string s1 = lines[0].Replace(' ', '_');
                if(string.IsNullOrEmpty(s1)) {
                    return null;
                } else {
                    return s1;
                }
            }
        }
        return null;
    }
}