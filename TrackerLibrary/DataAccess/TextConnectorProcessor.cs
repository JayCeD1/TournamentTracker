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
        public static void SaveToPrizeFile(this List<PrizeModel>models,string filename)
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
        private static string ConvertPeopleListToString(List<PersonModel> people)
        {
            string output = "";

            if(people.Count == 0)
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
        public static void SaveToTournamentFile(this List<TournamentModel> models, string filename)
        {
            List<string> lines = new();
            foreach (TournamentModel tm in models)
            {
                lines.Add($"{ tm.Id }," +
                            $"{ tm.TournamentName }," +
                            $"{ tm.EntryFee }," +
                            $"{ ConvertTeamListToString(tm.EnteredTeams) }," +
                            $"{ ConvertPrizeListToString(tm.Prizes) }" +
                            $"{ ConvertRoundsListToString(tm.Rounds) }");
            }
            File.WriteAllLines(filename.fullFilePath(), lines);
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
                output += $"{p.Id}";
            }
            output = output.Substring(0, output.Length - 1);
            return output;
        }
        private static string ConvertRoundsListToString(List<List<MatchupModel>> rounds)
        {
            string output = "";
            if (rounds.Count == 0)
            {
                return output;
            }
            foreach (List<MatchupModel> r in rounds)
            {
                output += $"{ ConvertMatchupListToString(r) }|";
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
        public static List<TournamentModel> ConvertToTournamentModels(this List<string> lines,string teamFileName,string peopleFileName,string prizeFileName)
        {
            //id,tournaName,EntryFee,{id|id|id-Entered Teams},{id|id|id-Prizes},{Rounds-id^id^id|id^id^id|id^id^id}
            List<TournamentModel> output = new();
            List<PrizeModel> prizes = prizeFileName.fullFilePath().LoadFile().ConvertToPrizeModels();
            List<TeamModel> teams = teamFileName.fullFilePath().LoadFile().ConvertToTeamModels(peopleFileName);

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
                //TODO - capture round information
                output.Add(tm);
            }

            return output;
        }
    }
}
