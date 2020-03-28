using fNbt;
using System.Collections.Generic;

public class AvailibleTeams {

    private List<Team> freeTeams;

    public AvailibleTeams(NbtCompound tag) {
        this.freeTeams = new List<Team>();
        int[] teamIds = tag.getIntArray("freeTeams");
        foreach(int id in teamIds) {
            this.freeTeams.Add(Team.teamFromId(id));
        }
    }

    public AvailibleTeams(int maxPlayers, Slice[] sliceArray) {
        this.freeTeams = new List<Team>(this.getTeamsBasedOnPlayerCount(maxPlayers));
        this.freeTeams.shuffle<Team>();
    }

    /// <summary>
    /// Returns true if there are more open teams for players to join.
    /// </summary>
    public bool moreRoom() {
        return this.freeTeams.Count > 0;
    }

    /// <summary>
    /// Returns a random open team, or null if there are no more open teams.
    /// </summary>
    public Team getTeam() {
        if(this.freeTeams.Count == 0) {
            return null; // No free teams.
        } else {
            Team team = this.freeTeams[0];
            this.freeTeams.RemoveAt(0);

            return team;
        }
    }

    public void writeToNbt(NbtCompound tag) {
        int[] teamIds = new int[this.freeTeams.Count];
        for(int i = 0; i < this.freeTeams.Count; i++) {
            teamIds[i] = this.freeTeams[i].getId();
        }
        tag.setTag("freeTeams", teamIds);
    }

    private Team[] getTeamsBasedOnPlayerCount(int maxPlayers) {
        if(maxPlayers == 2) {
            return new Team[] { Team.RED, Team.BLUE };
        } else if(maxPlayers == 3) {
            return new Team[] { Team.RED, Team.YELLOW, Team.PURPLE };
        } else if(maxPlayers == 4) {
            return new Team[] { Team.RED, Team.BLUE, Team.GREEN, Team.PURPLE };
        } else if(maxPlayers == 5) {
            return new Team[] { Team.RED, Team.BLUE, Team.YELLOW, Team.GREEN, Team.PURPLE };
        } else { // 6
            return Team.ALL_TEAMS;
        }
    }
}
