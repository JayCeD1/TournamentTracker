using System;
using System.Collections.Generic;
using System.Text;

namespace TrackerLibrary.models
{
    public class PersonModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Cellphone { get; set; }
        public string FullName
        {
            get { 
                return $"{FirstName} {LastName}"; 
            }
        }

    }
}
