using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iMovie.Models
{
    public class MovieDetails
    {
        public int ID { get; set; }
        public string Movie_Name { get; set; }
        public string Movie_Description { get; set; }
        public DateTime DateAndTime { get; set; }
        public string MoviePicture { get; set; }

        public virtual ICollection<BookingTable> booking { get; set; }
    }
}
