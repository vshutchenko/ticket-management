using System;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace ThirdPartyEventEditor.Models
{
    public class ThirdPartyEventCreateModel
    {
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

        [Display(Name = "Poster")]
        [Required(ErrorMessage = "Poster image is required")]
        public HttpPostedFileBase PosterImage { get; set; }
    }
}