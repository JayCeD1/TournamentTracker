
using System;
using System.Collections.Generic;
using System.Text;

namespace TrackerLibrary.models
{
    public class MatchupModel
    {
        public int Id { get; set; }
        public List<MatchupEntryModel> Entries { get; set; } = new List<MatchupEntryModel>();
        public TeamModel Winner { get; set; }
        public int MatchupRound { get; set; }
    }
}
