using System.Collections;
using CMS.Core.Domain;

namespace Portal.Modules.OrientalSails.Domain
{
    public class Costing
    {
        public virtual int Id { get; set; }
        public virtual CostType Type { get; set; }
        public virtual CostingTable Table { get; set; }
        public virtual double Adult { get; set; }
        public virtual double Child { get; set; }
        public virtual double Baby { get; set; }
    }

    public enum CostingType
    {
        Ticket,
        Meal,
        Kayaing,
        Service,
        Rockclimbing
    }
}
