using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Board", menuName = "Stratego/BoardType", order = 1)]
public class BoardType : ScriptableObject {

    [SerializeField]
    private string stringId;
    [SerializeField]
    private GameObject prefab;
    [SerializeField]
    [Min(1)]
    private int maxPieces;

    [Header("Default Piece Counts")]
    public int oneCount;
    public int twoCount;
    public int threeCount;
    public int fourCount;
    public int fiveCount;
    public int sixCount;
    public int sevenCount;
    public int eightCount;
    public int nineCount;
    public int bombCount;
    public int spyCount;

    public Board instantiateBoardPrefab() {
        Board b = GameObject.Instantiate(this.prefab).GetComponent<Board>();
        return b;
    }

    public string getIdentifier() {
        return this.stringId;
    }

    public int getMaxPieces() {
        return this.maxPieces;
    }

    public static BoardType getBoardFromPlayerCount(int playerCount) {
        return References.list.boards[playerCount == 2 ? 0 : 1];
    }
}
