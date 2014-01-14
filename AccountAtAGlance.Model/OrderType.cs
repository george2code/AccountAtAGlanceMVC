using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountAtAGlance.Model
{
    public class OrderType
    {
        public OrderType()
        {
            Orders = new HashSet<Order>();
        }

        // Primitive properties
        public int Id { get; set; }
        public string Type { get; set; }

        // Navigation properties
        public virtual ICollection<Order> Orders { get; set; }
    }
}
