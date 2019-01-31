using UnityEngine;
using System;

public class Team {

    public static readonly Team RED = new Team("Red", 0, Color.red);
    public static readonly Team BLUE = new Team("Blue", 1, Color.blue);
    public static readonly Team GREEN = new Team("Green", 2, Color.green);
    public static readonly Team YELLOW = new Team("Yellow", 3, Color.yellow);
    public static readonly Team ORANGE = new Team("Orange", 3, new Color(1f, 0.5f, 0f));
    public static readonly Team PURPLE = new Team("Purple", 3, new Color(0.498f, 0, 0.498f));

    private static Team[] ALL_TEAMS = new Team[] { RED, BLUE, GREEN, YELLOW, ORANGE, PURPLE };

    private readonly string name;
    private readonly int id;
    private readonly Color color;

    public Team(string name, int id, Color color) {
        this.name = name;
        this.id = id;
        this.color = color;
    }

    public int getId() {
        return this.id;
    }

    public string getName() {
        return this.name;
    }

    public Color getColor() {
        return this.color;
    }

    public static Team teamFromEnum(EnumTeam team) {
        switch(team) {
            case EnumTeam.RED: return RED;
            case EnumTeam.BLUE: return BLUE;
            case EnumTeam.GREEN: return GREEN;
            case EnumTeam.YELLOW: return YELLOW;
            case EnumTeam.ORANGE: return ORANGE;
            case EnumTeam.PURPLE: return PURPLE;
        }
        throw new Exception("Invalid team enum");
    }

    public static Team teamFromId(int id) {
        return Team.ALL_TEAMS[id];
    }
}
