using UnityEngine;

public class Attack {

    private Piece attacker;
    private Piece defender;
    private Cell defenderCell;
    private ConnectedPlayer initiator;

    public Attack(Piece attacker, Piece defender, ConnectedPlayer initiator) {
        this.attacker = attacker;
        this.defender = defender;
        this.defenderCell = this.defender.getCell();
        this.initiator = initiator;

        this.initiator.sendMessage(new MessageSetPieceVisible(this.defender, true));
    }

    /// <summary>
    /// Updates the state of the attack.  If true is returned, the attack is over
    /// and it should be the next Player's turn.
    /// </summary>
    public bool update() {
        this.func(this.attacker, this.defender);
        this.func(this.defender, this.attacker);

        if(Vector3.Distance(this.attacker.transform.position, this.defender.transform.position) <= 0.1f) {
            // Piece are close enough.

            // Find out the outcome of the attack.
            bool defenderDestroyed = false;
            EnumAttackOutcome outcome = this.attacker.getAttackOutcome(this.defender);
            if(outcome == EnumAttackOutcome.TIE) {
                this.destroyPiece(this.attacker);
                this.destroyPiece(this.defender);
                defenderDestroyed = true;
            }
            else if(outcome == EnumAttackOutcome.LOSE) {
                this.destroyPiece(this.attacker);

                this.defender.setDestination(this.defenderCell);
            }
            else if(outcome == EnumAttackOutcome.WIN) {
                this.destroyPiece(this.defender);
                defenderDestroyed = true;

                this.attacker.setDestination(this.defenderCell);
            }

            if(!defenderDestroyed) {
                this.initiator.sendMessage(new MessageSetPieceVisible(this.defender, false));
            }

            // Have the surviving piece, if any, move back ot it's cell.

            return true;
        }

        return false;
    }

    private void destroyPiece(Piece piece) {
        GameObject.Destroy(piece.gameObject);
    }

    private void func(Piece p, Piece target) {
        p.transform.position = Vector3.Lerp(p.transform.position, target.transform.position, 0.025f);
    }
}
