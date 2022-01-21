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
        private const string PrizesFile = "PrizesModel.csv";
        private const string PeopleFile = "PersonModel.csv";
        private const string TeamFile = "TeamModel.csv"; //we save the team name

        public PersonModel CreatePerson(PersonModel model)
        {
            List<PersonModel> people = PeopleFile.fullFilePath().LoadFile().ConvertToPersonModels();

            int currentId = 1;
            if(people.Count >= 1)
            {
                currentId = people.OrderByDescending(x => x.Id).First().Id + 1;
            }
            model.Id = currentId;
            people.Add(model);
            people.SaveToPeopleFile(PeopleFile);
            return model; 
        }

        public PrizeModel CreatePrize(PrizeModel model)
        {
            //load the text file
            //convert text to list of prize model
            List<PrizeModel> prizes = PrizesFile.fullFilePath().LoadFile().ConvertToPrizeModels();

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
            prizes.SaveToPrizeFile(PrizesFile);

            return model;
        }

        public TeamModel CreateTeam(TeamModel model)
        {
            List<TeamModel> teams = TeamFile.fullFilePath().LoadFile().ConvertToTeamModels(PeopleFile);
            //find the max id

            int currentId = 1;
            if (teams.Count >= 1)
            {
                currentId = teams.OrderByDescending(x => x.Id).First().Id + 1;
            }
            model.Id = currentId;
            teams.Add(model);

            teams.SaveToTeamFile(TeamFile);

            return model;
        }

        public List<PersonModel> GetPerson_All()
        {
            return PeopleFile.fullFilePath().LoadFile().ConvertToPersonModels();
        }

        public List<TeamModel> GetTeam_All()
        {
            throw new NotImplementedException();
        }
    }
}
