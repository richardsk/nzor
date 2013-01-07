using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NZOR.Data.Entities.Consensus;
using NZOR.Data.Repositories.Consensus;
using System.Data.SqlClient;
using System.Data;

namespace NZOR.Data.Sql.Repositories.Consensus
{
    public class AnnotationRepository : Repository<Annotation>, IAnnotationRepository
    {
        private List<Annotation> _annotations;

        public AnnotationRepository(String connectionString)
            : base(connectionString)
        {
            _annotations = new List<Annotation>();
        }

        public List<Annotation> Annotations
        {
            get { return _annotations; }
        }

        public Annotation GetAnnotation(Guid annotationId)
        {
            Annotation annotation = null;

            List<SqlParameter> parameters = new List<SqlParameter>();

            parameters.Add(new SqlParameter("@annotationId", annotationId));

            using (DataTable tbl = Utility.GetSourceData(_connectionString, Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Consensus.Annotation-GET.sql"), parameters))
            {
                if (tbl.Rows.Count > 0)
                {
                    DataRow row = tbl.Rows[0];

                    annotation = new Annotation();

                    annotation.AnnotationId = row.Field<Guid>("AnnotationId");
                    annotation.AnnotationText = row.Field<string>("AnnotationText");
                    annotation.AnnotationType = row.Field<string>("AnnotationType");
                    annotation.NameId = row.Field<Guid?>("NameID");
                    annotation.ReferenceId = row.Field<Guid?>("ReferenceId");
                    annotation.ConceptId = row.Field<Guid?>("ConceptID");
                    annotation.AddedDate = row.Field<DateTime?>("AddedDate");
                    annotation.ModifiedDate = row.Field<DateTime?>("ModifiedDate");                    
                }
            }

            return annotation;
        }

        public List<Annotation> GetAnnotationsByName(Guid nameId)
        {
            List<Annotation> annotations = new List<Annotation>();

            List<SqlParameter> parameters = new List<SqlParameter>();

            parameters.Add(new SqlParameter("@nameId", nameId));

            using (DataTable tbl = Utility.GetSourceData(_connectionString, Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Consensus.Annotation-LISTByName.sql"), parameters))
            {
                foreach (DataRow row in tbl.Rows)
                {
                    Annotation ann = new Annotation();

                    ann.AnnotationId = row.Field<Guid>("AnnotationId");
                    ann.AnnotationText = row.Field<string>("AnnotationText");
                    ann.AnnotationType = row.Field<string>("AnnotationType");
                    ann.NameId = row.Field<Guid?>("NameID");
                    ann.ReferenceId = row.Field<Guid?>("ReferenceId");
                    ann.ConceptId = row.Field<Guid?>("ConceptID");
                    ann.AddedDate = row.Field<DateTime?>("AddedDate");
                    ann.ModifiedDate = row.Field<DateTime?>("ModifiedDate");

                    annotations.Add(ann);
                }
            }

            return annotations;
        }

        public List<Annotation> GetAnnotationsByConcept(Guid conceptId)
        {
            List<Annotation> annotations = new List<Annotation>();

            List<SqlParameter> parameters = new List<SqlParameter>();

            parameters.Add(new SqlParameter("@conceptID", conceptId));

            using (DataTable tbl = Utility.GetSourceData(_connectionString, Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Consensus.Annotation-LISTByConcept.sql"), parameters))
            {
                foreach (DataRow row in tbl.Rows)
                {
                    Annotation ann = new Annotation();

                    ann.AnnotationId = row.Field<Guid>("AnnotationId");
                    ann.AnnotationText = row.Field<string>("AnnotationText");
                    ann.AnnotationType = row.Field<string>("AnnotationType");
                    ann.NameId = row.Field<Guid?>("NameID");
                    ann.ReferenceId = row.Field<Guid?>("ReferenceId");
                    ann.ConceptId = row.Field<Guid?>("ConceptID");
                    ann.AddedDate = row.Field<DateTime?>("AddedDate");
                    ann.ModifiedDate = row.Field<DateTime?>("ModifiedDate");

                    annotations.Add(ann);
                }
            }

            return annotations;
        }

        public void Save()
        {
            String sql = String.Empty;

            foreach (Annotation ann in _annotations)
            {
                if (ann.State == Entities.Entity.EntityState.Added)
                {
                    sql = Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Consensus.Annotation-INSERT.sql");
                }
                else if (ann.State == Entities.Entity.EntityState.Modified)
                {
                    sql = Utility.GetSQL("NZOR.Data.Sql.Resources.Sql.Consensus.Annotation-UPDATE.sql");
                }

                using (SqlConnection cnn = new SqlConnection(_connectionString))
                {
                    cnn.Open();

                    using (SqlCommand cmd = cnn.CreateCommand())
                    {
                        cmd.CommandType = System.Data.CommandType.Text;
                        cmd.CommandText = sql;

                        cmd.Parameters.AddWithValue("@annotationID", ann.AnnotationId);
                        cmd.Parameters.AddWithValue("@AnnotationText", ann.AnnotationText);
                        cmd.Parameters.AddWithValue("@AnnotationType", (object)ann.AnnotationType ?? DBNull.Value);
                                                
                        cmd.Parameters.AddWithValue("@NameID", (object)ann.NameId ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ReferenceId", (object)ann.ReferenceId ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ConceptID", (object)ann.ConceptId ?? DBNull.Value);
                                                
                        cmd.Parameters.AddWithValue("@AddedDate", (object)ann.AddedDate ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ModifiedDate", (object)ann.ModifiedDate ?? DBNull.Value);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}
