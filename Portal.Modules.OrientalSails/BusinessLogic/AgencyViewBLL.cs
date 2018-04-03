using Portal.Modules.OrientalSails.Domain;
using Portal.Modules.OrientalSails.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Portal.Modules.OrientalSails.BusinessLogic
{
    public class AgencyViewBLL
    {
        public AgencyRepository AgencyRepository { get; set; }
        public AgencyContractRepository AgencyContractRepository { get; set; }
        public AgencyViewBLL()
        {
            AgencyRepository = new AgencyRepository();
            AgencyContractRepository = new AgencyContractRepository();
        }

        public void Dispose()
        {
            if (AgencyRepository != null)
            {
                AgencyRepository.Dispose();
                AgencyRepository = null;
            }

            if (AgencyContractRepository != null) {
                AgencyContractRepository.Dispose();
                AgencyContractRepository = null;
            }
        }

        public Agency AgencyGetById(int agencyId)
        {
            return AgencyRepository.AgencyGetById(agencyId);
        }

        public void AgencyContractSaveOrUpdate(AgencyContract agencyContract)
        {
            AgencyContractRepository.SaveOrUpdate(agencyContract);
        }

        public IList<AgencyContract> AgencyContractGetAllByAgency(int agencyId)
        {
            return AgencyContractRepository.AgencyContractGetAllByAgency(agencyId);
        }

        public AgencyContract AgencyContractGetById(int agencyContractId)
        {
            return AgencyContractRepository.AgencyContractGetById(agencyContractId);
        }
    }
}