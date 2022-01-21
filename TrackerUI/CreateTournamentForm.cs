using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TrackerLibrary;
using TrackerLibrary.models;

namespace TrackerUI
{
    public partial class CreateTournamentForm : Form,IPrizeRequestor,ITeamRequestor
    {
        List<TeamModel> availableTeams = GlobalConfig.Connection.GetTeam_All();
        List<TeamModel> selectedTeams = new();
        List<PrizeModel> selectedPrizes = new();
        public CreateTournamentForm()
        {
            InitializeComponent();
            WireUpLists();
        }
        private void WireUpLists()
        {
            selectTeamDropDown.DataSource = null;
            selectTeamDropDown.DataSource = availableTeams;
            selectTeamDropDown.DisplayMember = "TeamName";

            tournamentTeamsListBox.DataSource = null;
            tournamentTeamsListBox.DataSource = selectedTeams;
            tournamentTeamsListBox.DisplayMember = "TeamName";

            prizesListBox.DataSource = null;
            prizesListBox.DataSource = selectedPrizes;
            prizesListBox.DisplayMember = "PlaceName";
        }

        private void deleteSelectedPlayerButton_Click(object sender, EventArgs e)
        {

        }

        private void selectTeamDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void addTeamButton_Click(object sender, EventArgs e)
        {
            //selectedTeams.Add((TeamModel)selectTeamDropDown.SelectedItem);

            TeamModel t = (TeamModel)selectTeamDropDown.SelectedItem;
            if (t != null)
            {
                availableTeams.Remove(t);
                selectedTeams.Add(t);

                WireUpLists();
            }
        }

        private void createPrizeButton_Click(object sender, EventArgs e)
        {
            //call the create prize form
            CreatePrizeForm frm = new(this);
            frm.Show();
        }

        public void PrizeComplete(PrizeModel model)
        {
            //get back from the form a prizeModel
            //Take the prizeModel and put it into our list of selected prizes
            selectedPrizes.Add(model);
            WireUpLists();        }

        public void TeamComplete(TeamModel model)
        {
            selectedTeams.Add(model);
            WireUpLists();
        }

        private void createNewTeamLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            CreateTeamForm frm = new(this);
            frm.Show();
        }
    }
}
