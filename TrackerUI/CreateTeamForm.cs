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
    public partial class CreateTeamForm : Form
    {

        private List<PersonModel> availableTeamMembers = GlobalConfig.Connection.GetPerson_All();
        private List<PersonModel> selectedTeamMembers = new();
        private ITeamRequestor callingForm;

        public CreateTeamForm(ITeamRequestor caller)
        {
            callingForm = caller;
            InitializeComponent();

            //CreateSampleData();
            WireUpLists();
        }
        private void CreateSampleData()
        {
            availableTeamMembers.Add(new PersonModel{FirstName = "tim",LastName="corey" });
            availableTeamMembers.Add(new PersonModel{FirstName = "jay",LastName="eren" });

            selectedTeamMembers.Add(new PersonModel { FirstName = "jane", LastName = "smith" });
            selectedTeamMembers.Add(new PersonModel { FirstName = "bill", LastName = "jones" });
        }
        private void WireUpLists()
        {
            selectTeamMemberDropDown.DataSource = null;
            selectTeamMemberDropDown.DataSource = availableTeamMembers;
            selectTeamMemberDropDown.DisplayMember = "FullName";

            teamMembersListBox.DataSource = null;
            teamMembersListBox.DataSource = selectedTeamMembers;
            teamMembersListBox.DisplayMember = "FullName";

        }

        private void headerLabel_Click(object sender, EventArgs e)
        {

        }

        private void addMemberButton_Click(object sender, EventArgs e)
        {
            PersonModel p = (PersonModel)selectTeamMemberDropDown.SelectedItem;
            if(p != null)
            {
                availableTeamMembers.Remove(p);
                selectedTeamMembers.Add(p);

                WireUpLists();
            }
           
        }

        private void createMemberButton_Click(object sender, EventArgs e)
        {
            if (ValidateForm())
            {
                PersonModel p = new PersonModel();
                p.FirstName = firstNameValue.Text;
                p.LastName = lastNameValue.Text;
                p.Email = emailValue.Text;
                p.Cellphone = cellPhoneValue.Text;

                p = GlobalConfig.Connection.CreatePerson(p);

                selectedTeamMembers.Add(p);
                WireUpLists();

                firstNameValue.Text = "";
                lastNameValue.Text = "";
                emailValue.Text = "";
                cellPhoneValue.Text = "";

               
            }
            else
            {
                MessageBox.Show("You need to fill in all of the fields");
            }
        }

        private bool ValidateForm()
        {
           if(firstNameValue.Text.Length == 0)
            {
                return false;
            }
            if (lastNameValue.Text.Length == 0)
            {
                return false;
            }
            if (emailValue.Text.Length == 0)
            {
                return false;
            }
            if (cellPhoneValue.Text.Length == 0)
            {
                return false;
            }
            return true;
        }

        private void removeSelectedMember_Click(object sender, EventArgs e)
        {
            PersonModel p = (PersonModel)teamMembersListBox.SelectedItem;

            if (p != null)
            {
                selectedTeamMembers.Remove(p);
                availableTeamMembers.Add(p);

                WireUpLists();
            }
            else
            {
                MessageBox.Show("Select an item to remove");
            }
           
        }

        private void createTeamButton_Click(object sender, EventArgs e)
        {
            TeamModel t = new();
            t.TeamName = teamNameValue.Text;
            t.TeamMembers = selectedTeamMembers;

            GlobalConfig.Connection.CreateTeam(t);

            callingForm.TeamComplete(t);//Do you resolve the TeamComplete method depending on the calling class(callingForm) since its an interface?
            this.Close();
        }
    }
}
