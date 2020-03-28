/// <summary>
/// A message sent to all Clients, telling them that it is someone else's turn.
/// </summary>
public class MessageChangeTurn : AbstractMessageServer {

    public int teamIdOfTurn;

    public MessageChangeTurn() { }

    public MessageChangeTurn(Team team) {
        this.teamIdOfTurn = team.getId();
    }

    public override short getID() {
        return 2002;
    }

    public override void processMessage(NetHandlerClient handler) {
        handler.tellTurn(this);
    }
}
