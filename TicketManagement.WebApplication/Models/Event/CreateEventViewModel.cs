using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TicketManagement.WebApplication.Models.Event
{
    public class CreateEventViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [Display(Name = "Name")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required")]
        [Display(Name = "Description")]
        public string Description { get; set; } = string.Empty;

        public string Layout { get; set; } = string.Empty;
        public string Venue { get; set; } = string.Empty;

        public SelectList? Layouts { get; set; }

        public SelectList? Venues { get; set; }

        [Display(Name = "Start date")]
        public DateTime StartDate { get; set; }

        [Display(Name = "End date")]
        public DateTime EndDate { get; set; }

        [Display(Name = "Image link")]
        public string ImageUrl { get; set; } = string.Empty;
    }
}
