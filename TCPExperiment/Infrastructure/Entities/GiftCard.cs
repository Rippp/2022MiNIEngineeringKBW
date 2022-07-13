using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Entities
{
    public class GiftCard
    {
        public GeishaType Type { get; set; }

        public GiftCard(GeishaType type)
        {
            Type = type;
        }
    }
}
