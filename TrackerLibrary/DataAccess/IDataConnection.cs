using System;
using System.Collections.Generic;
using System.Text;
using TrackerLibrary.models;

namespace TrackerLibrary.DataAccess
{
    public interface IDataConnection
    {
        PrizeModel CreatePrize(PrizeModel model);
        PersonModel CreatePerson(PersonModel model);
        void CreateTournament(TournamentModel model);
        List<PersonModel> GetPerson_All();
        List<TeamModel> GetTeam_All();
        TeamModel CreateTeam(TeamModel model);
    }
}
