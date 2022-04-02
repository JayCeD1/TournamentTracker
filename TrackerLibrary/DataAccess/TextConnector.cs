using System;
using System.Collections.Generic;
using System.Text;
using TrackerLibrary.models;
using TrackerLibrary.DataAccess.TextHelpers;
using System.Linq;

namespace TrackerLibrary.DataAccess
{
    public class TextConnector : IDataConnection
    {
        public void CompleteTournamentModel(TournamentModel model)
        {
            List<TournamentModel> tournaments = GlobalConfig.TournamentFile.fullFilePath().LoadFile().ConvertToTournamentModels();
          
            tournaments.Remove(model);
            tournaments.SaveToTournamentFile();
            //For updating the tournament matchups i.e rounds byes etc (updating the byes)
            TournamentLogic.UpdateTournamentResults(model);
        }

        public void CreatePerson(PersonModel model)
        {
            List<PersonModel> people = GlobalConfig.PeopleFile.fullFilePath().LoadFile().ConvertToPersonModels();

            int currentId = 1;
            if(people.Count >= 1)
            {
                currentId = people.OrderByDescending(x => x.Id).First().Id + 1;
            }
            model.Id = currentId;
            people.Add(model);
            people.SaveToPeopleFile();
        }

        public void CreatePrize(PrizeModel model)
        {
            //load the text file
            //convert text to list of prize model
            List<PrizeModel> prizes = GlobalConfig.PrizesFile.fullFilePath().LoadFile().ConvertToPrizeModels();

            //find the max id

            int currentId = 1;
            if(prizes.Count >= 1)
            {
                currentId = prizes.OrderByDescending(x => x.Id).First().Id + 1;
            }
            model.Id = currentId;

            //Add new record with new id
            prizes.Add(model);
            //convert prizes to list <string>
            //save the list<string> to the text file
            prizes.SaveToPrizeFile();

        }

        public void CreateTeam(TeamModel model)
        {
            List<TeamModel> teams = GlobalConfig.TeamFile.fullFilePath().LoadFile().ConvertToTeamModels();
            //find the max id

            int currentId = 1;
            if (teams.Count >= 1)
            {
                currentId = teams.OrderByDescending(x => x.Id).First().Id + 1;
            }
            model.Id = currentId;
            teams.Add(model);

            teams.SaveToTeamFile(GlobalConfig.TeamFile);

        }

        public void CreateTournament(TournamentModel model)
        {
            List<TournamentModel> tournaments = GlobalConfig.TournamentFile.fullFilePath().LoadFile().ConvertToTournamentModels();
            int currentId = 1;
            if(tournaments.Count > 0)
            {
                currentId = tournaments.OrderByDescending(x => x.Id).First().Id + 1;
            }
            model.Id = currentId;
            model.SaveRoundsToFile();
            tournaments.Add(model);
            tournaments.SaveToTournamentFile();
            //For updating the tournament matchups i.e rounds byes etc (updating the byes)
            TournamentLogic.UpdateTournamentResults(model);
        }

        public List<PersonModel> GetPerson_All()
        {
            return GlobalConfig.PeopleFile.fullFilePath().LoadFile().ConvertToPersonModels();
        }

        public List<TeamModel> GetTeam_All()
        {
            return GlobalConfig.TeamFile.fullFilePath().LoadFile().ConvertToTeamModels();
        }

        public List<TournamentModel> GetTournament_All()
        {
            return GlobalConfig.TournamentFile.fullFilePath().LoadFile().ConvertToTournamentModels();
        }

        public void UpdateMatchup(MatchupModel model)
        {
            model.UpdateMatchupToFile();
        }
    }
}
