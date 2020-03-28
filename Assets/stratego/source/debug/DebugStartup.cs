using UnityEngine;
using UnityEngine.Networking;

public class DebugStartup : MonoBehaviour {

#if UNITY_EDITOR
    public bool loadSave = true;

    private void Start() {
        if(NetworkManager.singleton != null) {
            ScreenManager.singleton.showScreen(null);

            CustomNetworkManager manager = GameObject.FindObjectOfType<CustomNetworkManager>();

            if(this.loadSave) {
                GameSaver.loadGame(manager);
            } else {
                manager.startGameServer(new GameOptions(), null);
            }

            GameObject.Destroy(this.gameObject);
        }
    }
#endif
}
