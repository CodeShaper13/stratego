using UnityEngine;
using UnityEngine.Networking;

public class DebugStartup : MonoBehaviour {

    private void Update() {
        if(NetworkManager.singleton != null) {
            Main.singleton.state = -1;
            NetworkManager.singleton.StartHost();
            GameObject.Destroy(this.gameObject);
        }
    }
}
