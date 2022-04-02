using System;
using System.Collections.Generic;
using System.Configuration;
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
            model.Rounds.Add(CreateFirstRound(byes,randomizedTeams));//add a bye to 1st rounders
            CreateOtherRounds(model, rounds); //the details of this function won't b run if no. of rounds less than 2
            
        }
        private static int CheckCurentRound(this TournamentModel model)
        {
            int output = 1;
            foreach (List<MatchupModel> round in model.Rounds)
            {
                if (round.All(x => x.Winner != null))
                {
                    output += 1;
                }
                else
                {
                    return output;
                }
            }
            //Tournament is complete
            CompleteTournament(model);
            return output - 1;
        }

        private static void CompleteTournament(TournamentModel model)
        {
            GlobalConfig.Connection.CompleteTournamentModel(model);
            TeamModel winner = model.Rounds.Last().First().Winner;
            TeamModel runnerUp = model.Rounds.Last().First().Entries.Where(x => x.TeamCompeting != winner).First().TeamCompeting;

            decimal winnerPrize = 0;
            decimal runnerUpPrize = 0;

            PrizeModel firstPlacePrize = model.Prizes.Where(x => x.PlaceNumber == 1).FirstOrDefault();
            PrizeModel secondPlacePrize = model.Prizes.Where(x => x.PlaceNumber == 2).FirstOrDefault();

            if (model.Prizes.Count > 0)
            {
                decimal totalIncome = model.EnteredTeams.Count * model.EntryFee;

                if (firstPlacePrize != null)
                {
                    winnerPrize = firstPlacePrize.CalculatePrizePayout(totalIncome);

                }
                if (secondPlacePrize != null)
                {
                    runnerUpPrize = secondPlacePrize.CalculatePrizePayout(totalIncome);

                }
            }
            //send email to all tournament users
            string subject;
            StringBuilder body = new();
            
                subject = $"In {model.TournamentName}, {winner.TeamName} has won. ";
                body.AppendLine("<h1>we have a WINNER</h1>");
                body.Append("<p>Congratulations to our winner on this great tournament </p>");
                body.Append("</br>");
                
                if(winnerPrize > 0)
                {
                    body.AppendLine($"<p>{winner.TeamName} will receive ${winnerPrize}</p>");
                    
                }
                 if (runnerUpPrize > 0)
                {
                    body.AppendLine($"<p>{runnerUp.TeamName} will receive ${runnerUpPrize}</p>");
                    
                }
                body.AppendLine("<p>Thanks for a great tournament everyone!</p>");
                body.AppendLine("Tournament Tracker");

            List<string> bcc = new();
            foreach (TeamModel team in model.EnteredTeams)
            {
                foreach (PersonModel p in team.TeamMembers)
                {
                    if (p.Email.Length > 0)
                    {
                        bcc.Add(p.Email); 
                    }
                }
            }

            EmailLogic.SendEmail(new List<string>(),bcc, subject, body.ToString());
            //completeTournament
            model.CompleteTournament();
        }
        private static decimal CalculatePrizePayout(this PrizeModel prize,decimal totalIncome)
        {
            decimal output = 0;
            if(prize.PrizeAmount > 0)
            {
                output = prize.PrizeAmount;
            }
            else
            {
                output = Decimal.Multiply(totalIncome, Convert.ToDecimal(prize.PrizePercentage / 100));
            }
            return output;
        }
        public static void UpdateTournamentResults(TournamentModel model)
        {
            int startingRound = model.CheckCurentRound();
            List<MatchupModel> toScore = new();

            foreach (List <MatchupModel> rounds in model.Rounds)
            {
                foreach (MatchupModel rm in rounds)
                {//if any entry has a score or an entry has a bye and no winner yet
                    if(rm.Winner == null && (rm.Entries.Any(x => x.Score != 0) || rm.Entries.Count == 1)) //prevent re-scoring already scored teams
                    {
                        toScore.Add(rm); //matchups to be scored
                    }
                }
            }
            MarkWinnerInMatchups(toScore);
            AdvanceWinners(toScore,model);
            // GlobalConfig.Connection.UpdateMatchup(model: m);
            toScore.ForEach(x => GlobalConfig.Connection.UpdateMatchup(x));

            int endingRound = model.CheckCurentRound();

            if(endingRound > startingRound)
            {
                //alert users via email
                model.AlertUsersToNewRound();
            }
        }
        public static void AlertUsersToNewRound(this TournamentModel model)
        {
            int currentRoundNumber = model.CheckCurentRound();
            List<MatchupModel> currentRound = model.Rounds.Where(x => x.First().MatchupRound == currentRoundNumber).First();
            foreach (MatchupModel matchup in currentRound)
            {
                foreach (MatchupEntryModel me in matchup.Entries)
                {
                    foreach (PersonModel p in me.TeamCompeting.TeamMembers)
                    {
                        AlertPersonToNewRound(p,me.TeamCompeting.TeamName,matchup.Entries.Where(x => x.TeamCompeting != me.TeamCompeting).FirstOrDefault());
                    }
                }
            }
        }
        private static void AlertPersonToNewRound(PersonModel p, string teamName, MatchupEntryModel competitor)
        {
            if(p.Email.Length == 0)
            {
                return;
            }
            string to = "";
            string subject = "";

            StringBuilder body = new();
            if (competitor != null)
            {
                subject = $"You have a new match up with {competitor.TeamCompeting.TeamName}. ";
                body.AppendLine("<h1>You have a new matchup</h1>");
                body.Append("<strong>competitor: </competitor>");
                body.Append(competitor.TeamCompeting.TeamName);
                body.AppendLine();
                body.AppendLine();
                body.AppendLine("Have a nice day!");
                body.AppendLine("Tournament Tracker");
            }
            else
            {
                subject = "You have a bye week this round";

                body.AppendLine("Enjoy your round off");
                body.AppendLine("Tournament Tracker");
            }
           
            EmailLogic.SendEmail(to, subject, body.ToString());
        }
        private static void AdvanceWinners(List<MatchupModel> models, TournamentModel tournament)
        {
            //update the next round matchups
            foreach (MatchupModel m in models)
            {
                foreach (List<MatchupModel> rounds in tournament.Rounds) //loop thru the rounds
                {
                    foreach (MatchupModel rm in rounds)
                    {
                        foreach (MatchupEntryModel me in rm.Entries)
                        {
                            if (me.ParentMatchup != null) //all matchups greater than the 1st round have a parentmatchup
                            {//if below checks any parent match up id corresponds to any after updating the score
                                if (me.ParentMatchup.Id == m.Id) //check if the selected matchup id matches any in me.parent match up
                                {
                                    me.TeamCompeting = m.Winner;
                                    GlobalConfig.Connection.UpdateMatchup(model: rm);
                                }
                            }
                        }
                    }
                } 
            }
        }
        private static void MarkWinnerInMatchups(List<MatchupModel> models)
        {
            string greaterWins = ConfigurationManager.AppSettings["greaterWins"];
            //0 means false or low score wins
            foreach (MatchupModel m in models)
            { 
                //checks for bye entry
                if(m.Entries.Count == 1)
                {
                    m.Winner = m.Entries[0].TeamCompeting;
                    continue;
                }
                if (greaterWins == "0")
                {
                    if(m.Entries[0].Score < m.Entries[1].Score)
                    {
                        m.Winner = m.Entries[0].TeamCompeting;
                    }
                    else if(m.Entries[1].Score < m.Entries[0].Score)
                    {
                        m.Winner = m.Entries[0].TeamCompeting;
                    }
                    else
                    {
                        throw new Exception("We don't allow ties in this application");
                    }
                }
                else
                {
                    //1 means true or high score wins
                    if (m.Entries[0].Score > m.Entries[1].Score)
                    {
                        m.Winner = m.Entries[0].TeamCompeting;
                    }
                    else if (m.Entries[1].Score > m.Entries[0].Score)
                    {
                        m.Winner = m.Entries[0].TeamCompeting;
                    }
                    else
                    {
                        throw new Exception("We don't allow ties in this application");
                    }
                } 
            }
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
                    {//i.e this z where we add the actual matchups for every mo than 2 entries
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
