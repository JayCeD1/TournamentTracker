using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.models;

namespace TrackerLibrary
{
    public static class TournamentLogic
    {
        //Order our list randoomly of teams
        //check if its big enough, if not include byes 2^2^2^2 = 2^4;
        //create our first round of matchups
        //create every round after that 8 - 4 - 2 - 1 match up
        public static void CreateRounds(TournamentModel model)
        {
            List<TeamModel> randomizedTeams = RandomizeTeamOrder(model.EnteredTeams);
            int rounds = FindNumberOfRounds(model.EnteredTeams.Count);
            int byes = NumberOfByes(rounds, model.EnteredTeams.Count);
            model.Rounds.Add(CreateFirstRound(byes,randomizedTeams));
            CreateOtherRounds(model, rounds); //the details of this function won't b run if no. of rounds less than 2
        }
        private static void CreateOtherRounds(TournamentModel model,int rounds)
        {
            int round = 2;
            List<MatchupModel> previousRound = model.Rounds[0];
            List<MatchupModel> currRound = new();
            MatchupModel currMatchup = new();
            while(round <= rounds)
            {
                foreach (MatchupModel match in previousRound)
                {
                    currMatchup.Entries.Add(new MatchupEntryModel { ParentMatchup = match });
                    if(currMatchup.Entries.Count > 1) //if mo than 1 i.e 2 add the match-up ...match-up-entry z added here!
                    {
                        currMatchup.MatchupRound = round;
                        currRound.Add(currMatchup);
                        currMatchup = new();
                    }
                }
                model.Rounds.Add(currRound); //adds a list of match-up-models to the tournament round
                previousRound = currRound; //updates the new previous round

                currRound = new(); //resets the curRound var for reuse
                round += 1;
            }
        }
        private static List<MatchupModel> CreateFirstRound(int byes, List<TeamModel> teams)
        {
            List<MatchupModel> output = new();
            MatchupModel curr = new();
            foreach (TeamModel team in teams)
            {
                curr.Entries.Add(new MatchupEntryModel { TeamCompeting = team });
                if(byes > 0 || curr.Entries.Count > 1) //if a bye exists, then this match-up z done o if der r 2 teams
                {
                    curr.MatchupRound = 1;
                    output.Add(curr); //add the match up if d above condition met.
                    curr = new(); //?create a new match-up-entry-model on each iteration. //reusing the variable after populating it.restarting d model
                }
                if(byes > 0)
                {
                    byes -= 1;
                }
            }
            return output;
        }
        private static int NumberOfByes(int rounds,int numberOfTeams)
        {
            int output = 0;
            int totalTeams = 1;
            for (int i = 1; i <= rounds; i++)
            {
                totalTeams *= 2;
            }
            output = totalTeams - numberOfTeams;
            return output;
        }
        private static int FindNumberOfRounds(int teamCount)
        {
            int output = 1;
            int val = 2;

            while (val < teamCount)
            {
                output += 1;
                val *= 2;
            }
            return output;
        }
        private static List<TeamModel> RandomizeTeamOrder(List<TeamModel> teams)
        {
            //cards.OrderBy(a => Guid.NewGuid()).ToList();
            return teams.OrderBy(x => Guid.NewGuid()).ToList();
        }
    }
}
