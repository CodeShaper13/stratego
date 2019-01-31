using System.Collections.Generic;

public class AvailibleTeams {

    private List<Team> freeTeams;
    private List<Team> usedTeams;

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

        //this.shuffleList<Team>(this.freeTeams);
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

    private void shuffleList<T>(List<T> list) {
        int n = list.Count;
        System.Random rnd = new System.Random();
        while(n > 1) {
            int k = (rnd.Next(0, n) % n);
            n--;
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}
