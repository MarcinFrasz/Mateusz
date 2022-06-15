using System.ComponentModel.DataAnnotations;

namespace DbManipulationApp.Models
{
    public class EditUserModel
    {

        public EditUserModel()
        {
            //Claims = new List<string>();
            Roles = new List<string>();
        }

        public string Id { get; set; }

        [Required]
        public string UserName { get; set; }
        [Phone]
        public string PhoneNumber { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }


      //  public List<string> Claims { get; set; }

        public IList<string> Roles { get; set; }
    }

}

