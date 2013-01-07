using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NZOR.Admin.Data.Sql.Repositories;
using System.Web.Http;
using NZOR.Publish.Model.Administration;
using NZOR.Publish.Model.Matching;
using NZOR.Data.Entities.Provider;
using NZOR.Data.Sql.Repositories.Provider;

namespace NZOR.Web.Service.APIs
{
    public class AdminController : ApiController
    {        
        private readonly AdminRepository _adminRepository;
        private readonly NameRepository _nameRepository;
        
        public AdminController(AdminRepository adminRepository, NameRepository nameRepository)
        {
            _adminRepository = adminRepository;
            _nameRepository = nameRepository;
        }
        
        public SettingResponse GetSetting(string name)
        {
            NZOR.Admin.Data.Entities.Setting s1 = _adminRepository.GetSetting(name);

            SettingResponse sr = new SettingResponse();
            sr.SettingId = s1.SettingId;
            sr.Name = s1.Name;
            sr.Value = s1.Value;

            return sr;
        }

        [HttpGet]
        public void SetSetting(string name, string value)
        {
            _adminRepository.SetSetting(name, value);
        }

        public List<NameRequestResponse> GetNameRequests(string apiKey)
        {
            List<NZOR.Admin.Data.Entities.NameRequest> nrList = _adminRepository.GetNameRequestsByApiKey(apiKey);
            List<NameRequestResponse> response = new List<NameRequestResponse>();

            foreach (NZOR.Admin.Data.Entities.NameRequest nr in nrList)
            {
                NameRequestResponse nrr = new NameRequestResponse();
                nrr.BatchMatchId = nr.BatchMatchId.ToString();
                nrr.FullName = nr.FullName;
                nrr.NameRequestId = nr.NameRequestId.ToString();
                nrr.RequestDate = nr.RequestDate.ToString();
                nrr.Status = nr.Status;

                List<NZOR.Admin.Data.Entities.BrokeredName> bnList = _adminRepository.GetBrokeredNamesForNameRequest(nr.NameRequestId);
                nrr.BrokeredNames = new List<BrokeredName>();
                foreach (NZOR.Admin.Data.Entities.BrokeredName bn in bnList)
                {
                    if (bn.NZORProviderNameId.HasValue)
                    {
                        Name pn = _nameRepository.GetName(bn.NZORProviderNameId.Value);
                        BrokeredName resultBn = new BrokeredName();
                        resultBn.DataSource = bn.WebUrl;
                        resultBn.FullName = pn.FullName;
                        resultBn.ProviderNameSource = pn.DataSourceName;
                        resultBn.ProviderNameStatus = pn.LinkStatus;
                        nrr.BrokeredNames.Add(resultBn);
                    }
                }

                response.Add(nrr);
            }

            return response;
        }
    }
}
