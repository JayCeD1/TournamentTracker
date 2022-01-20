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
            throw new NotImplementedException();
        }

        public List<PersonModel> GetPerson_All()
        {
            return PeopleFile.fullFilePath().LoadFile().ConvertToPersonModels();
        }
    }
}
