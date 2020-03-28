using UnityEngine;

public class MessageSetTeam : AbstractMessageServer {

    public int teamId;
    public Vector3 cameraPosition;
    public Quaternion cameraRotation;

    public MessageSetTeam() { }

    public MessageSetTeam(int teamId, Vector3 pos, Quaternion rot) {
        this.teamId = teamId;
        this.cameraPosition = pos;
        this.cameraRotation = rot;
    }

    public override short getID() {
        return 2008;
    }

    public override void processMessage(NetHandlerClient handler) {
        handler.setTeam(this);
    }
}
