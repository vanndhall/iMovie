using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace iMovie.Models
{
    public class BookingTable
    {
        public int ID { get; set; }
        public string seatno { get; set; }
        public string UserID { get; set; }
        public DateTime Datetopresent { get; set; }
        public int MovieDetailsId { get; set; }
        public int Amount { get; set; }

        [ForeignKey("MovieDetailsId")]
        public virtual MovieDetails moviedetails { get; set; }
    }
}
