using fNbt;
using UnityEngine.Networking;

public class ConnectedPlayer {

    private NetworkConnection connection;
    private string username;
    private Team team;
    private bool ready;
    private bool spectating;

    public ConnectedPlayer(NetworkConnection connection, string username) {
        this.connection = connection;
        this.username = username;
    }

    public ConnectedPlayer(NbtCompound tag) {
        this.username = tag.getString("username");
        this.team = Team.teamFromId(tag.getInt("teamId"));
        this.spectating = tag.getBool("isSpectating");
        this.ready = tag.getBool("isReady");
    }

    public void setConnection(NetworkConnection connection) {
        this.connection = connection;
    }

    /// <summary>
    /// Null if the Player is no longer connected.
    /// </summary>
    public NetworkConnection getConnection() {
        return this.connection;
    }

    /// <summary>
    /// Sends a message to this player.  If this player is not connected, no message will be sent.
    /// </summary>
    public void sendMessage(AbstractMessageServer msg) {
        if(this.connection != null) {
            this.connection.Send(msg.getID(), msg);
        }
    }

    public bool isEqualTo(ConnectedPlayer cp) {
        return this.connection.connectionId == cp.connection.connectionId;
    }

    public string getUsername() {
        return this.username;
    }

    public void setUsername(string username) {
        this.username = username;
    }

    public bool isReady() {
        return this.ready;
    }

    public void setReady(bool isReady) {
        this.ready = isReady;
    }

    public bool isSpectating() {
        return this.spectating;
    }

    /// <summary>
    /// Sets the Player to spectating.  The client is alerted to this as well.
    /// </summary>
    public void setSpectating() {
        this.spectating = true;
        this.sendMessage(new MessageStartSpectating());
    }

    /// <summary>
    /// Checks if the passed client is connected.
    /// </summary>
    public bool isConnected() {
        return this.connection != null;
    }

    public Team getTeam() {
        return this.team;
    }

    public void setTeam(Team team) {
        this.team = team;
    }

    /// <summary>
    /// Returns true if this player has a team assigned.
    /// </summary>
    /// <returns></returns>
    public bool hasTeam() {
        return this.team != null;
    }

    public NbtCompound writeToNbt() {
        NbtCompound tag = new NbtCompound();
        tag.setTag("username", this.username);
        tag.setTag("teamId", this.team.getId());
        tag.setTag("isSpectating", this.spectating);
        tag.setTag("isReady", this.ready);

        return tag;
    }
}
