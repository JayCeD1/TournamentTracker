using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.models;

namespace TrackerLibrary.DataAccess.TextHelpers
{
    public static class TextConnectorProcessor
    {
        public static string fullFilePath(this string filename)
        {
            return $"C:\\Users\\Jessy\\source\\repos\\TournamentTracker\\{filename}";
        } 
        /// <summary>
        /// Is responsible for reading the contents of a file and converts them to List of string
        /// </summary>
        /// <param name="file">
        /// is an extension file that takes in the fullfilepath 
        /// </param>
        /// <returns>
        /// returns contents of a file in a List format of data-type string
        /// </returns>
        public static List<string> LoadFile(this string file)
        {
            if (!File.Exists(file))
            {
                return new List<string>();
            }
            return File.ReadAllLines(file).ToList();
        }
        public static List<PrizeModel> ConvertToPrizeModels(this List<string> lines)
        {
            List<PrizeModel> output = new List<PrizeModel>();

            foreach (string line in lines)
            {
                string[] cols = line.Split(',');
                PrizeModel p = new PrizeModel();
                p.Id = int.Parse(cols[0]);
                p.PlaceNumber = int.Parse(cols[1]);
                p.PlaceName = cols[2];
                p.PrizeAmount = decimal.Parse(cols[3]);
                p.PrizePercentage = double.Parse(cols[4]);
                output.Add(p);
            }
            return output;
        }
        public static List<PersonModel> ConvertToPersonModels(this List<string> lines)
        {
            List<PersonModel> output = new List<PersonModel>();

            foreach(string line in lines)
            {
                string[] cols = line.Split(',');
                PersonModel p = new PersonModel();
                p.Id = int.Parse(cols[0]);
                p.FirstName = cols[1];
                p.LastName = cols[2];
                p.Email = cols[3];
                p.Cellphone = cols[4];
                output.Add(p);
            }
            return output;
        }
        public static List<TeamModel> ConvertToTeamModels(this List<string> lines,string peopleFileName)
        {
            List<TeamModel> output = new();
            List<PersonModel> people = peopleFileName.fullFilePath().LoadFile().ConvertToPersonModels();

            foreach (string line in lines)
            {
                string[] cols = line.Split(',');
                TeamModel t = new();
                t.Id = int.Parse(cols[0]);
                t.TeamName = cols[1];

                string[] personIds = cols[2].Split('|');
                foreach (string id in personIds)
                {
                    t.TeamMembers.Add(people.Where(x => x.Id == int.Parse(id)).First());
                }
                output.Add(t);
            }
            return output;
        }
        public static void SaveToPrizeFile(this List<PrizeModel> models,string filename)
        {
            List<string> lines = new List<string>();

            foreach (PrizeModel p in models)
            {
                lines.Add($"{ p.Id },{ p.PlaceNumber },{ p.PlaceName },{ p.PrizeAmount },{ p.PrizePercentage }");
            }
            File.WriteAllLines(filename.fullFilePath(), lines);
        }
        public static void SaveToPeopleFile(this List<PersonModel>models, string filename)
        {
            List<string> lines = new();
            foreach (PersonModel p in models)
            {
                lines.Add($"{ p.Id },{ p.FirstName },{ p.LastName },{ p.Email },{ p.Cellphone } ");
            }
            File.WriteAllLines(filename.fullFilePath(), lines);
        }
        public static void SaveToTeamFile(this List<TeamModel> models, string filename)
        {
            List<string> lines = new List<string>();
            foreach (TeamModel t in models)
            {
                lines.Add($"{ t.Id },{ t.TeamName },{ ConvertPeopleListToString(t.TeamMembers) }");
            }
            File.WriteAllLines(filename.fullFilePath(), lines);
        }
        public static void SaveToTournamentFile(this List<TournamentModel> models, string filename)
        {
            List<string> lines = new();
            foreach (TournamentModel tm in models)
            {
                lines.Add($"{ tm.Id }," +
                            $"{ tm.TournamentName }," +
                            $"{ tm.EntryFee }," +
                            $"{ ConvertTeamListToString(tm.EnteredTeams) }," +
                            $"{ ConvertPrizeListToString(tm.Prizes) }," +
                            $"{ ConvertRoundsListToString(tm.Rounds) }");
            }
            File.WriteAllLines(filename.fullFilePath(), lines);
        }
        private static string ConvertPeopleListToString(List<PersonModel> people)
        {
            string output = "";

            if (people.Count == 0)
            {
                return output;
            }
            foreach (PersonModel p in people)
            {
                output += $"{p.Id}|";
            }
            output = output.Substring(0, output.Length - 1);

            return output;
        }
        private static string ConvertTeamListToString(List<TeamModel> teams)
        {
            string output = "";
            if(teams.Count == 0)
            {
                return "";
            }
            foreach (TeamModel t in teams)
            {
                output += $"{t.Id}|";
            }
            output = output.Substring(0, output.Length - 1);//for removing the last pipe from the output string.
            return output;
        } 
        private static string ConvertPrizeListToString(List<PrizeModel> prizes)
        {
            string output = "";
            if(prizes.Count == 0)
            {
                return output;
            }
            foreach (PrizeModel p in prizes)
            {
                output += $"{p.Id}|";
            }
            output = output.Substring(0, output.Length - 1);
            return output;
        }
        private static string ConvertRoundsListToString(List<List<MatchupModel>> rounds)
        {
            //{Rounds - id^id^id|id^id^id|id^id^id}
            string output = "";
            if (rounds.Count == 0)
            {
                return output;
            }
            foreach (List<MatchupModel> r in rounds)
            {
                output += $"{ConvertMatchupListToString(r)}|";
            }
            output = output.Substring(0, output.Length - 1);
            return output;
        }
        private static string ConvertMatchupListToString(List<MatchupModel> matchups)
        {
            string output = "";
            if (matchups.Count == 0)
            {
                return output;
            }
            foreach (MatchupModel m in matchups)
            {
                output += $"{m.Id}^";
            }
            output = output.Substring(0, output.Length - 1);
            return output;
        }
        private static string ConvertMatchupEntryListToString(List<MatchupEntryModel> entries)
        {
            string output = "";
            if (entries.Count == 0)
            {
                return output;
            }
            foreach (MatchupEntryModel e in entries)
            {
                output += $"{e.Id}|";
            }
            output = output.Substring(0, output.Length - 1);
            return output;
        }
        public static List<TournamentModel> ConvertToTournamentModels(this List<string> lines,string teamFileName,string peopleFileName,string prizeFileName)
        {
            //id,tournaName,EntryFee,{id|id|id-Entered Teams},{id|id|id-Prizes},{Rounds-id^id^id|id^id^id|id^id^id}
            List<TournamentModel> output = new();
            List<PrizeModel> prizes = prizeFileName.fullFilePath().LoadFile().ConvertToPrizeModels();
            List<TeamModel> teams = teamFileName.fullFilePath().LoadFile().ConvertToTeamModels(peopleFileName);
            List<MatchupModel> matchups = GlobalConfig.MatchupFile.fullFilePath().LoadFile().ConvertToMatchupModels();

            foreach (string line in lines)
            {
                string[] cols = line.Split(',');
                TournamentModel tm = new();
                tm.Id = int.Parse(cols[0]);
                tm.TournamentName = cols[1];
                tm.EntryFee = decimal.Parse(cols[2]);

                string[] teamIds = cols[3].Split('|');
                foreach (string id in teamIds)
                {
                    tm.EnteredTeams.Add(teams.Where(x => x.Id == int.Parse(id)).First());
                }

                string[] prizeIds = cols[4].Split('|');
                foreach (string id in prizeIds)
                {
                    tm.Prizes.Add(prizes.Where(x => x.Id == int.Parse(id)).First());
                }
                //capture round information
                string[] rounds = cols[5].Split("|");

                foreach (string round in rounds)
                {//gets an array of ids
                    List<MatchupModel> ms = new();

                    string[] msText = round.Split("^");
                    foreach (string matchupModelTextId in msText)
                    {//adds the match up model that matches the id
                        ms.Add(matchups.Where(x => x.Id == int.Parse(matchupModelTextId)).First());
                    }
                    tm.Rounds.Add(ms);
                }
                output.Add(tm);
            }

            return output;
        }
        public static void SaveRoundsToFile(this TournamentModel model, string matchupFile, string matchupEntryFile)
        {
            //loop through each round
            //loop through each match-up
            //Get the id of the new matchup and save the record
            //loop through each Entry, get the id, and save it
            foreach (List<MatchupModel> rounds in model.Rounds)
            {
                foreach (MatchupModel matchup in rounds)
                {
                    //load all the matchups from file
                    //get the top id and add 1
                    //store the id
                    //save the matchup record
                    matchup.SaveMatchupToFile(matchupFile,matchupEntryFile);

                   
                }
            }
        }
        public static List<MatchupEntryModel> ConvertToMatchupEntryModels(this List<string> lines)
        {
            //id = 0, TeamCompeting = 1, Score = 2, ParentMatchup = 3;
            List<MatchupEntryModel> output = new();

            foreach (string  line in lines)
            {
                string[] cols = line.Split(",");
                MatchupEntryModel me = new();
                me.Id = int.Parse(cols[0]);
                if (cols[1].Length == 0)
                {
                    me.TeamCompeting = null;
                }
                else
                {
                    me.TeamCompeting = LookupTeamById(int.Parse(cols[1]));
                }
                me.Score = double.Parse(cols[2]);

                int parentId = 0;
                if(int.TryParse(cols[3], out parentId))
                {
                    me.ParentMatchup = LookupMatchupById(parentId);
                }
                else
                {
                    me.ParentMatchup = null;
                }
                output.Add(me);
            }
            return output;
        }
        private static List<MatchupEntryModel> ConvertStringToMatchupEntryModel(string input)
        {
            string[] ids = input.Split('|');
            List<MatchupEntryModel> output = new();
            List<string> entries = GlobalConfig.MatchupEntryFile.fullFilePath().LoadFile();
            List<string> matchingEntries = new();

            foreach (string id in ids)
            {
                foreach (string  entry in entries)
                {
                    string[] cols = entry.Split(",");

                    if(cols[0] == id)
                    {
                        matchingEntries.Add(entry);
                    }
                }
            }

            output = matchingEntries.ConvertToMatchupEntryModels();
            return output;
        }
        private static TeamModel LookupTeamById(int id)
        {
            List<string> teams = GlobalConfig.TeamFile.fullFilePath().LoadFile();
            foreach (string team in teams)
            {
                string[] cols = team.Split(",");
                
                if(cols[0] == id.ToString())
                {
                    List<string> matchingTeams = new();
                    matchingTeams.Add(team);
                    return matchingTeams.ConvertToTeamModels(GlobalConfig.PeopleFile).First();
                }
            }
            return null;
        }
        private static MatchupModel LookupMatchupById(int id)
        {
            List<string> matchups = GlobalConfig.MatchupFile.fullFilePath().LoadFile();

            foreach (string matchup in matchups)
            {
                string[] cols = matchup.Split(",");

                if(cols[0] == id.ToString())
                {
                    List<string> matchingMatchups = new();
                    matchingMatchups.Add(matchup);
                    return matchingMatchups.ConvertToMatchupModels().First();
                }
            }
            return null;
        }
        public static List<MatchupModel> ConvertToMatchupModels(this List<string> lines)
        {
            //id=1,entries=1(pipe delimited by id),winner=2,matchupRound=3
            List<MatchupModel> output = new List<MatchupModel>();

            foreach (string line in lines)
            {
                string[] cols = line.Split(',');
                MatchupModel p = new MatchupModel();
                p.Id = int.Parse(cols[0]);
                p.Entries = ConvertStringToMatchupEntryModel(cols[1]);
                
                if (cols[2].Length == 0)
                {
                    p.Winner = null; 
                }
                else
                {
                    p.Winner = LookupTeamById(int.Parse(cols[2]));
                }
                p.MatchupRound = int.Parse(cols[3]);
                output.Add(p);
            }
            return output;
        }
        public static void SaveMatchupToFile(this MatchupModel matchup,string matchupFile,string matchupEntryFile)
        {
            List<MatchupModel> matchups = GlobalConfig.MatchupFile.fullFilePath().LoadFile().ConvertToMatchupModels();
            int currentId = 1;
            if (matchups.Count >= 1)
            {
                currentId = matchups.OrderByDescending(x => x.Id).First().Id + 1;
            }
            matchup.Id = currentId;
            matchups.Add(matchup);

            foreach (MatchupEntryModel entry in matchup.Entries)
            {
                //save matchup entries to file
                entry.SaveEntryToFile(matchupEntryFile);
            }

            //save matchups to file
            List<string> lines = new();
            foreach (MatchupModel m in matchups)
            {//id=1,entries=1(pipe delimited by id),winner=2,matchupRound=3
                string winner = "";
                if (m.Winner != null)
                {
                    winner = m.Winner.Id.ToString();
                }
                lines.Add($"{ m.Id },{ ConvertMatchupEntryListToString(m.Entries) },{ winner },{ m.MatchupRound }");
            }
            File.WriteAllLines(GlobalConfig.MatchupFile.fullFilePath(), lines);
        }
        public static void SaveEntryToFile(this MatchupEntryModel entry, string matchupEntryFile)
        {
            //saving one entry to file requires loading all entries, ordering in descending to get the last id and adding 1 to it
            //then adding that one to last pile of the entries
            List<MatchupEntryModel> entries = GlobalConfig.MatchupEntryFile.fullFilePath().LoadFile().ConvertToMatchupEntryModels();//loads matchup entries
            int currentId = 1;
            if (entries.Count >= 1)
            {
                currentId = entries.OrderByDescending(x => x.Id).First().Id + 1;
            }
            entry.Id = currentId;
            entries.Add(entry);

            List<string> lines = new List<string>();
            foreach (MatchupEntryModel e in entries)
            {//id = 0, TeamCompeting = 1, Score = 2, ParentMatchup = 3;
                string parent = "";
                if(e.ParentMatchup != null)
                {
                    parent = e.ParentMatchup.Id.ToString();
                }
                string teamCompeting = "";
                if (e.TeamCompeting != null)
                {
                    teamCompeting = e.TeamCompeting.Id.ToString();
                }
                lines.Add($"{ e.Id },{ teamCompeting },{ e.Score },{ parent }");
            }
            File.WriteAllLines(GlobalConfig.MatchupEntryFile.fullFilePath(), lines);
         
        }
       
    }
}
