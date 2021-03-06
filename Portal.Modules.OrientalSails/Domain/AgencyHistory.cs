using System;
using CMS.Core.Domain;

namespace Portal.Modules.OrientalSails.Domain
{
    public class AgencyHistory
    {
        public virtual int Id { get; set; }

        public virtual Agency Agency { get; set; }

        public virtual User Sale { get; set; }

        public virtual DateTime ApplyFrom { get; set; }
    }
}
