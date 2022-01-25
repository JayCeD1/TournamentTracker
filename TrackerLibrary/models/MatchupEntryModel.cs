using System;
using System.Collections.Generic;
using System.Text;

namespace TrackerLibrary.models
{
    public class MatchupEntryModel
    {
        public int Id { get; set; }
        /// <summary>
        /// Represents one team in the match up
        /// </summary>
        public TeamModel TeamCompeting { get; set; }

        /// <summary>
        /// Represents the score for this particular team
        /// </summary>
        public double Score { get; set; }

        /// <summary>
        /// Represents the match up that this team came 
        /// from as the winner
        /// </summary>
        public MatchupModel ParentMatchup { get; set; }
    }
}
