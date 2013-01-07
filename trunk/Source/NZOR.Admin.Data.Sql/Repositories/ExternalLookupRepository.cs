using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

using NZOR.Admin.Data.Sql.Helpers;
using NZOR.Admin.Data.Entities;
using NZOR.Admin.Data.Repositories;

namespace NZOR.Admin.Data.Sql.Repositories
{
    public class ExternalLookupRepository : Repository<ExternalLookupService>, IExternalLookupRepository
    {
        public ExternalLookupRepository(String connectionString)
            : base(connectionString)
        {
        }

        public ExternalLookupService GetLookupService(Guid id)
        {
            ExternalLookupService service = null;

            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(new SqlParameter("@externalLookupServiceId", id));

            NZOR.Admin.Data.Repositories.IScheduledTaskRepository str = new ScheduledTaskRepository(_connectionString);

            using (DataTable tbl = Utility.GetSourceData(_connectionString, Utility.GetSQL("NZOR.Admin.Data.Sql.Resources.Sql.ExternalLookupService-GET.sql"), sqlParameters))
            {
                if (tbl.Rows.Count > 0)
                {
                    DataRow row = tbl.Rows[0];
                    service = new ExternalLookupService();

                    service.ExternalLookupServiceId = row.Field<Guid>("ExternalLookupServiceId");
                    service.Name = row.Field<String>("Name");
                    service.Description = row.IsNull("Description") ? null : row.Field<String>("Description");
                    service.DataFormat = row.Field<String>("DataFormat");
                    service.NameLookupEndpoint = row.IsNull("NameLookupEndpoint") ? null : row.Field<String>("NameLookupEndpoint");
                    service.ConceptLookupEndpoint = row.IsNull("ConceptLookupEndpoint") ? null : row.Field<String>("ConceptLookupEndpoint");
                    service.ReferenceLookupEndpoint = row.IsNull("ReferenceLookupEndpoint") ? null : row.Field<String>("ReferenceLookupEndpoint");
                    service.IDLookupEndpoint = row.IsNull("IDLookupEndpoint") ? null : row.Field<String>("IDLookupEndpoint");
                    service.SpaceCharacterSubstitute = row.IsNull("SpaceCharacterSubstitute") ? null : row.Field<String>("SpaceCharacterSubstitute");
                    service.IconFilename = row.IsNull("IconFilename") ? null : row.Field<String>("IconFilename");
                    service.LookupServiceClassName = row.IsNull("LookupServiceClassName") ? null : row.Field<String>("LookupServiceClassName");
                }
            }

            return service;            
        }

        public List<ExternalLookupService> ListLookupServices()
        {
            List<ExternalLookupService> services = new List<ExternalLookupService>();
            
            NZOR.Admin.Data.Repositories.IScheduledTaskRepository str = new ScheduledTaskRepository(_connectionString);

            using (DataTable tbl = Utility.GetSourceData(_connectionString, Utility.GetSQL("NZOR.Admin.Data.Sql.Resources.Sql.ExternalLookupService-LIST.sql")))
            {
                foreach (DataRow row in tbl.Rows)
                {
                    ExternalLookupService els = new ExternalLookupService();

                    els.ExternalLookupServiceId = row.Field<Guid>("ExternalLookupServiceId");
                    els.Name = row.Field<String>("Name");
                    els.Description = row.IsNull("Description") ? null : row.Field<String>("Description");
                    els.DataFormat = row.Field<String>("DataFormat");
                    els.NameLookupEndpoint = row.IsNull("NameLookupEndpoint") ? null : row.Field<String>("NameLookupEndpoint");
                    els.ConceptLookupEndpoint = row.IsNull("ConceptLookupEndpoint") ? null : row.Field<String>("ConceptLookupEndpoint");
                    els.ReferenceLookupEndpoint = row.IsNull("ReferenceLookupEndpoint") ? null : row.Field<String>("ReferenceLookupEndpoint");
                    els.IDLookupEndpoint = row.IsNull("IDLookupEndpoint") ? null : row.Field<String>("IDLookupEndpoint");
                    els.SpaceCharacterSubstitute = row.IsNull("SpaceCharacterSubstitute") ? null : row.Field<String>("SpaceCharacterSubstitute");
                    els.IconFilename = row.IsNull("IconFilename") ? null : row.Field<String>("IconFilename");
                    els.LookupServiceClassName = row.IsNull("LookupServiceClassName") ? null : row.Field<String>("LookupServiceClassName");
                    services.Add(els);
                }
            }

            return services;    
        }
    }
}
