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
    public partial class TournamentViewerForm : Form
    {
        private TournamentModel tournament;
        BindingList<int> rounds = new BindingList<int>();
        BindingList<MatchupModel> selectedMatchups = new();


        public TournamentViewerForm(TournamentModel tournamentModel)
        {
            InitializeComponent();
            tournament = tournamentModel;

            WireUpLists();

            LoadFormData();
            LoadRounds();
        }

        private void WireUpLists()
        {
            //refresh the drop down
            ///DataSource = null;
            roundDropDown.DataSource = rounds;

            //refresh listbox
            // matchupListBox.DataSource = null;
            matchupListBox.DataSource = selectedMatchups;
            matchupListBox.DisplayMember = "DisplayName";
        }
        private void LoadFormData()
        {
            TournamentName.Text = tournament.TournamentName;
        }
        private void LoadRounds()
        {
            rounds.Clear();

            rounds.Add(1);
            int currRound = 1;

            foreach (List<MatchupModel> matchups in tournament.Rounds)
            {
                if (matchups.First().MatchupRound > currRound)
                {
                    currRound = matchups.First().MatchupRound;
                    rounds.Add(currRound);
                }
            }
            LoadMatchups(1);
        }

        private void roundDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadMatchups((int)roundDropDown.SelectedItem);
        }
        private void LoadMatchups(int round)
        {
            foreach (List<MatchupModel> matchups in tournament.Rounds)
            {
                if (matchups.First().MatchupRound == round)
                {
                    matchupListBox.SelectedIndexChanged -= matchupListBox_SelectedIndexChanged;//for disabling event
                    selectedMatchups.Clear(); //this clear triggers the event on 121 wic causes error then line 76 runs. 
                    matchupListBox.SelectedIndexChanged += matchupListBox_SelectedIndexChanged; //reanable event

                    foreach (MatchupModel m in matchups)
                    {
                        if(m.Winner == null || !unplayedOnlyCheckBox.Checked)//if there's no winner or not checked
                        {
                            selectedMatchups.Add(m);//this as well triggers the event on 121 unlike this doesn't cuz error
                        }//if there's no winner or not checked
                    }

                }
            }
            if (selectedMatchups.Count > 0)
            {
                LoadMatchup(selectedMatchups.First()); 
            }
            DisplayMatchupInfo();
        }
        private void DisplayMatchupInfo()
        {
            bool isVisible = (selectedMatchups.Count > 0);
            teamOneName.Visible = isVisible;
            teamOneScoreLabel.Visible = isVisible;
            teamOneScoreValue.Visible = isVisible;

            teamTwoName.Visible = isVisible;
            teamTwoScoreLabel.Visible = isVisible;
            teamTwoScoreValue.Visible = isVisible;

            versusLabel.Visible = isVisible;
            scoreButton.Visible = isVisible;
        }

        private void LoadMatchup(MatchupModel m)
        {
            for (int i = 0; i < m.Entries.Count; i++)
            {
                if (i == 0)//first entry
                {
                    if (m.Entries[0].TeamCompeting != null)
                    {
                        teamOneName.Text = m.Entries[0].TeamCompeting.TeamName;
                        teamOneScoreValue.Text = m.Entries[0].Score.ToString();

                        teamTwoName.Text = "<Bye>";
                        teamTwoScoreValue.Text = "";
                    }
                    else
                    {
                        teamOneName.Text = "Not Yet Set";
                        teamOneScoreValue.Text = "";
                    }
                }
                if (i == 1)//second entry
                {
                    if (m.Entries[1].TeamCompeting != null)
                    {
                        teamTwoName.Text = m.Entries[1].TeamCompeting.TeamName;
                        teamTwoScoreValue.Text = m.Entries[1].Score.ToString();
                    }
                    else
                    {
                        teamTwoName.Text = "Not Yet Set";
                        teamTwoScoreValue.Text = "";
                    }
                }
            }
        }

        private void matchupListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadMatchup((MatchupModel)matchupListBox.SelectedItem);
        }
        private void unplayedOnlyCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            LoadMatchups((int)roundDropDown.SelectedItem);
        }
        private string ValidateData()
        {
            string output = "";
           // double teamOneScore = 0;
            double teamTwoScore = 0;

            bool scoreOneValid = double.TryParse(teamOneScoreValue.Text, out double teamOneScore);
            bool scoreTwoValid = double.TryParse(teamTwoScoreValue.Text, out teamTwoScore);

            if(!scoreOneValid)
            {
                output = "The score one value is not a valid number";
            }
            else if (!scoreTwoValid)
            {
                output = "The score two value is not a valid number";
            }
            else if (teamOneScore == 0 && teamTwoScore == 0)
            {
                output = "You did not enter a score for either teams";
            }
            else if(teamOneScore == teamTwoScore)
            {
                output = "We do not allow ties in this application";
            }
            return output;
        }
        private void scoreButton_Click(object sender, EventArgs e)
        {
            string errorMessage = ValidateData();
            if (errorMessage.Length > 0)
            {
                MessageBox.Show($"Input error: {errorMessage}");
                return;
            }
            MatchupModel m = (MatchupModel)matchupListBox.SelectedItem;
            double teamOneScore = 0;
            double teamTwoScore = 0;

            for (int i = 0; i < m.Entries.Count; i++)
            {
                if (i == 0)//first Matchupentry
                {
                    if (m.Entries[0].TeamCompeting != null)
                    {
                        bool scoreValid = double.TryParse(teamOneScoreValue.Text, out teamOneScore);

                        if (scoreValid)
                        {
                            m.Entries[0].Score = teamOneScore; //update model with the scores
                           
                        }
                        else
                        {
                            MessageBox.Show("Please enter valid score for team 1");
                            return;//proceed no further
                        }

                    }
                   
                }
                if (i == 1)//second Matchentry
                {
                    if (m.Entries[1].TeamCompeting != null)
                    {
                        bool scoreValid = double.TryParse(teamTwoScoreValue.Text, out teamTwoScore);

                        if (scoreValid)
                        {
                            m.Entries[1].Score = teamTwoScore; //update model with the scores
                        }
                        else
                        {
                            MessageBox.Show("Please enter valid score for team 2");
                            return;//proceed no further
                        }
                    }
                }
            }
            try
            {
                TournamentLogic.UpdateTournamentResults(tournament);//we are updating entire model thats why
                                                                    //refreshes the matchup list whenever the score button is clicked
            }
            catch (Exception ex)//for catching the exception that wud have thrown in 
            {

                MessageBox.Show($"The application had the following error: {ex.Message}");
                return;
            }
            LoadMatchups((int)roundDropDown.SelectedItem);

        }
    }
}
