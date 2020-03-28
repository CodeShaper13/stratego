using UnityEngine;
using UnityEngine.Networking;
using System;
using UnityEngine.UI;

public class ScreenJoinGame : ScreenBase {

    [SerializeField]
    private GameObject buttonPrefab;
    [SerializeField]
    private Transform holder; 

    private NetworkDiscovery discovery;
    private BtnHolder[] buttonHolders;

    public override void onAwake() {
        this.discovery = this.getNetworkManager().getDiscovery();

        this.buttonHolders = new BtnHolder[10];
        for(int i = 0; i < 10; i++) {
            GameObject obj = GameObject.Instantiate(this.buttonPrefab, this.holder);
            obj.transform.localPosition = new Vector3(0, i * 50, 0);
            BtnHolder btnHolder = new BtnHolder(obj);
            this.buttonHolders[i] = btnHolder;

            obj.GetComponent<Button>().onClick.AddListener(delegate {
                this.callback_btnClick(btnHolder);
                this.playBtnSound();
            });
        }
    }

    public override void onUiOpen() {
        this.discovery.Initialize();
        this.discovery.StartAsClient();
    }

    public override void onUiClose() {
        this.discovery.StopBroadcast();
    }

    public override bool showPhotographBackground() {
        return true;
    }

    public override void onEscape() {
        this.discovery.StopBroadcast();
        this.screenManager.showScreen(this.screenManager.screenMainMenu);
    }

    public override void onUpdate() {
        if(this.discovery.broadcastsReceived != null) {

            int i = 0;
            foreach(string addr in this.discovery.broadcastsReceived.Keys) {
                NetworkBroadcastResult value = this.discovery.broadcastsReceived[addr];

                string dataString = BytesToString(value.broadcastData);
                string[] items = dataString.Split(':');

                try {
                    int port = Convert.ToInt32(items[2]);

                    BtnHolder holder = this.buttonHolders[i];
                    holder.obj.SetActive(true);
                    holder.text.text =
                        "IP Address(" + addr + ")  Port(" + port + ")";

                    holder.address = value.serverAddress;
                    holder.port = port;

                    i++;
                } catch(FormatException e) {
                    print("Error parsing " + items[2] + " to a number");
                }                
            }

            // Gray out the unused buttons.
            for(int j = i; j < 10; j++) {
                this.buttonHolders[j].obj.SetActive(false);
            }
        }
    }

    public void callback_back() {
        this.onEscape();
    }

    private void callback_btnClick(BtnHolder btnHolder) {
        if(NetworkManager.singleton != null && NetworkManager.singleton.client == null) {
            this.screenManager.showScreen(this.screenManager.screenInfo);
            this.screenManager.screenInfo.setMessage("Connecting", "Cancel", true);

            NetworkManager mng = NetworkManager.singleton;
            mng.networkAddress = btnHolder.address;
            mng.networkPort = btnHolder.port;
            mng.StartClient();
        }
    }

    private static string BytesToString(byte[] bytes) {
        char[] chars = new char[bytes.Length / sizeof(char)];
        Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
        return new string(chars);
    }

    [Serializable]
    private class BtnHolder {

        public Text text;
        public GameObject obj;
        public string address;
        public int port;

        public BtnHolder(GameObject btn) {
            this.text = btn.GetComponentInChildren<Text>();
            this.obj = btn;
        }
    }
}
