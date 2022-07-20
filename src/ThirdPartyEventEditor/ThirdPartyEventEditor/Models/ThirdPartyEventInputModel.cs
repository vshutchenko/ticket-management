using System;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace ThirdPartyEventEditor.Models
{
    public class ThirdPartyEventInputModel
    {
        public int Id { get; set; }

        [Display(Name = "Name")]
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [Display(Name = "Description")]
        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Start date")]
        [Required(ErrorMessage = "Start date is required")]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "End date")]
        [Required(ErrorMessage = "End date is required")]
        public DateTime EndDate { get; set; }

        [Display(Name = "Poster")]
        [Required(ErrorMessage = "Poster image is required")]
        public HttpPostedFileBase PosterImage { get; set; }

        [Display(Name = "Current image")]
        public string CurrentImage { get; set; }
    }
}