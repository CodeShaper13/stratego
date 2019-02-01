using System.Collections.Generic;
using UnityEngine;

public class AvailibleTeams {

    private List<Team> freeTeams;
    private List<Team> usedTeams;

    public Dictionary<Team, int> teamToSliceMapping;

    public AvailibleTeams(int maxPlayers) {
        this.freeTeams = new List<Team>(3);
        this.usedTeams = new List<Team>();

        this.freeTeams.Add(Team.RED);
        this.freeTeams.Add(Team.BLUE);

        if(maxPlayers >= 3) {
            this.freeTeams.Add(Team.YELLOW);
        }
        if(maxPlayers >= 4) {
            this.freeTeams.Add(Team.GREEN);
        }
        if(maxPlayers == 6) {
            this.freeTeams.Add(Team.ORANGE);
            this.freeTeams.Add(Team.PURPLE);
        }


        //this.freeTeams.shuffle();


        this.teamToSliceMapping = new Dictionary<Team, int>();

        if(maxPlayers == 2) {
            Debug.Log("PROBLEM!!! In availibleTeams.cs");
        }
        else if(maxPlayers == 3) {
            for(int i = 0; i < 3; i++) {
                this.teamToSliceMapping.Add(this.freeTeams[i], i * 2);
            }
        }
        else if(maxPlayers == 4) {
            this.teamToSliceMapping.Add(this.freeTeams[0], 0);
            this.teamToSliceMapping.Add(this.freeTeams[1], 1);
            this.teamToSliceMapping.Add(this.freeTeams[2], 3);
            this.teamToSliceMapping.Add(this.freeTeams[3], 4);
        }
        else if(maxPlayers == 6) {
            for(int i = 0; i < 6; i++) {
                this.teamToSliceMapping.Add(this.freeTeams[i], i);
            }
        }
    }

    public bool moreRoom() {
        return this.freeTeams.Count > 0;
    }

    public Team getRandomTeam() {
        Team team = this.freeTeams[0];
        this.freeTeams.RemoveAt(0);

        this.usedTeams.Add(team);

        return team;
    }

    public class Clazz {

        public int sliceIndex;
        public Team team;

        public Clazz(Team team, int i) {
            this.team = team;
            this.sliceIndex = i;
        }
    }
}
