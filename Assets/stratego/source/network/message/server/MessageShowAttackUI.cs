using UnityEngine;

public class MessageShowAttackUI : AbstractMessageServer {

    public GameObject attackerPiece;
    public GameObject defenderPiece;
    // -1 if it was a tie.
    public int winnerId;

    public MessageShowAttackUI() {
    }

    public MessageShowAttackUI(Piece attacker, Piece defender, Team winner) {
        this.attackerPiece = attacker.gameObject;
        this.defenderPiece = defender.gameObject;
        this.winnerId = winner == null ? -1 : winner.getId();
    }

    public override short getID() {
        return 2005;
    }

    public override void processMessage(NetHandlerClient handler) {
        handler.showAttackUI(this);
    }
}
