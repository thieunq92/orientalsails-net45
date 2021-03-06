using System;
using System.Collections.Generic;
using CMS.Core.Domain;

namespace Portal.Modules.OrientalSails.Domain
{
    public class AgencyLocation
    {
        private IList<AgencyLocation> _child = new List<AgencyLocation>();

        public virtual int Id { get; set; }
        public virtual bool Deleted { get; set; }
        public virtual string Name { get; set; }
        public virtual AgencyLocation Parent { get; set; }

        public virtual IList<AgencyLocation> Child
        {
            get { return _child; }
            set { _child = value; }
        }
    }
}