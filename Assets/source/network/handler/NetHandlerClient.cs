using System;
using UnityEngine;
using UnityEngine.Networking;

public class NetHandlerClient : NetHandlerBase {

    private Player player;

    public NetHandlerClient(Player localPlayer) {
        this.player = localPlayer;
    }

    protected override void registerHandlers() {
        this.registerMsg<MessageShowText>();
        this.registerMsg<MessageChangTeurn>();
        this.registerMsg<MessageSetPieceVisible>();
        this.registerMsg<MessageStartSpectating>();
    }

    public void startSpectating(MessageStartSpectating messageStartSpectating) {
        this.player.isSpectating = true;

        // Reveal the values of all the other pieces.
        foreach(Piece piece in GameObject.FindObjectsOfType<Piece>()) {
            piece.showActualValue(true);
        }
    }

    public void tellTurn(MessageChangTeurn msg) {
        string s;
        if(msg.i == this.player.controllingTeamID) {
            s = "Your Turn";
            this.player.isMyTurn = true;
            this.player.movedThisTurn = false;
        } else {
            s = Team.teamFromId(msg.i).getName() + "'s Turn";
            this.player.isMyTurn = false;
        }

        this.player.showText(s, 0f, true);
    }

    public void showText(MessageShowText msg) {
        this.player.showText(msg.message, msg.duration, msg.inCorner);
    }

    public void revealPieceValue(MessageSetPieceVisible msg) {
        Piece piece = msg.piece.GetComponent<Piece>();
        piece.showActualValue(msg.visibleValue);
    }

    private void registerMsg<T>() where T : AbstractMessageClient, new() {
        NetworkManager.singleton.client.RegisterHandler
            (new T().getID(), delegate (NetworkMessage netMsg) {
                T msg = netMsg.ReadMessage<T>();
                msg.processMessage(this);
            });
    }
}
