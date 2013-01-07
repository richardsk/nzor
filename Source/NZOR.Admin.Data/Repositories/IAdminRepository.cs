using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NZOR.Admin.Data.Entities;

namespace NZOR.Admin.Data.Repositories
{
    public interface IAdminRepository
    {
        List<User> Users { get; }
     
        NZORStatistics GetStatistics();
        void UpdateStatistics();

        List<User> GetAllUsers();
        User GetUser(Guid userId);
        User GetUserByEmail(string email);
        User GetUserByApiKey(string apiKey);

        Setting GetSetting(string name);
        void SetSetting(string name, string value);

        List<NameRequest> GetPendingNameRequests();
        List<NameRequest> GetNameRequestsByApiKey(string apiKey);
        NameRequest GetNameRequestByFullName(string fullName);
        List<BrokeredName> GetBrokeredNamesForNameRequest(Guid nameRequestId);
        List<BrokeredName> GetNewBrokeredNames();
        void DeleteBrokeredName(Guid brokeredNameId);
        
        void Save();
    }
}
