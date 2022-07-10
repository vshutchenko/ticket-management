using System.ComponentModel.DataAnnotations;

namespace TicketManagement.WebApplication.Models.Event
{
    public class EventViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [Display(Name = "Name")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [Display(Name = "Description")]
        public string? Description { get; set; }

        public int LayoutId { get; set; }

        [Display(Name = "Start date")]
        public DateTime StartDate { get; set; } = DateTime.Now;

        [Display(Name = "End date")]
        public DateTime EndDate { get; set; } = DateTime.Now;

        [Display(Name = "Image link")]
        public string? ImageUrl { get; set; }

        [Display(Name = "Published")]
        public bool Published { get; set; }
    }
}
