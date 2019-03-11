using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ElectionPortal.Models
{
    public class CreatePasscode
    {

        [Display(Name = "Number of passcodes")]
        public int Number { get; set; }

        public CreatePasscode()
        {

        }
    }

    public class SearchPasscode
    {
        [Required]
        [Display(Name = "Passcode")]
        public string Passcode { get; set; }

        [Display(Name = "Reg Number")]
        public long RegNumber { get; set; }

        [Display(Name = "Used")]
        public bool IsUsed { get; set; }

        public SearchPasscode()
        {

        }
    }
       
    public class VotePanel
    {
        public List<Post> PostsCollection { get; set; }

        public List<Candidate> CandidatesCollection { get; set; }
        
        public VotePanel()
        {

        }
    }

    public class LoginPanel
    {
        [Required]
        [Display(Name = "Registration number")]
        public long RegNumber { get; set; }

        public LoginPanel()
        {

        }
    }
}