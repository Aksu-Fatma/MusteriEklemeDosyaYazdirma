using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AksuProje
{
    public class Notes
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public DateTime CeraetedAt { get; set; }
        public required string Text { get; set; }

       
    }
}
