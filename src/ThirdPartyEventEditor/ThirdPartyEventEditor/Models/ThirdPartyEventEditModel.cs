using System;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace ThirdPartyEventEditor.Models
{
    public class ThirdPartyEventEditModel
    {
        public Guid Id { get; set; }

        [Display(Name = "Name")]
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [Display(Name = "Description")]
        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; }

        [Display(Name = "Start date")]  
        [Required(ErrorMessage = "Start date is required")]
        public DateTime StartDate { get; set; }

        [Display(Name = "End date")]     
        [Required(ErrorMessage = "End date is required")]
        public DateTime EndDate { get; set; }

        [Display(Name = "New poster image")]
        public HttpPostedFileBase NewPosterImage { get; set; }

        [Display(Name = "Current image")]
        public string CurrentImage { get; set; }
    }
}