using System;
using System.ComponentModel.DataAnnotations;

namespace ThirdPartyEventEditor.Models
{
    public class ThirdPartyEvent
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy hh:mm}")]
        public DateTime StartDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy hh:mm}")]
        public DateTime EndDate { get; set; }

        public string PosterImage { get; set; }
    }
}