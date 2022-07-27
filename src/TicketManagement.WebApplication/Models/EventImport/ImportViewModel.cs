using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TicketManagement.WebApplication.Models.EventImport
{
    public class ImportViewModel
    {
        public string Layout { get; set; } = string.Empty;

        public string Venue { get; set; } = string.Empty;

        [Display(Name = "Layout")]
        public SelectList? Layouts { get; set; }

        [Display(Name = "Venue")]
        public SelectList? Venues { get; set; }

        [Required(ErrorMessage = "Select a file for import.")]
        [Display(Name = "File with events")]
        public IFormFile? EventsJson { get; set; }
    }
}
