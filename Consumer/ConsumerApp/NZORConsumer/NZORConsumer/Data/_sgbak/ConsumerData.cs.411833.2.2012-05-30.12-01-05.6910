using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace NZORConsumer.Data
{
    public class ConsumerData
    {
        public static void CreateDefaultHarvest(NZORConsumer.Model.ConsumerEntities ce, out string message)
        {
            message = "";
            try
            {
                //do we need to?
                if (ce.Harvests.Count() == 0)
                {
                    Model.HarvestScope hs = new Model.HarvestScope();
                    ce.HarvestScopes.AddObject(hs);
                    ce.SaveChanges();

                    hs = ce.HarvestScopes.First();

                    Model.Harvest h = new Model.Harvest();
                    h.HarvestScopeId = hs.HarvestScopeId;
                    h.ServiceUrl = ConfigurationManager.AppSettings["DefaultNZORServiceUrl"];
                    h.IntervalDays = 2;

                    ce.Harvests.AddObject(h);
                    ce.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                message = "Error : " + ex.Message;
            }
        }

        public static int NameCount(NZORConsumer.Model.ConsumerEntities ce, out string message)
        {
            message = "";
            try
            {
                return ce.Names.Count();
            }
            catch (Exception ex)
            {
                message = "Error : " + ex.Message;
            }
            return 0;
        }

        public static List<Model.Name> SearchNames(Model.ConsumerEntities ce, string searchText, int maxRows, out int totalCount, out string message)
        {
            List<Model.Name> results = new List<Model.Name>();
            totalCount = 0;
            message = "";

            try
            {
                IQueryable<Model.Name> names = ce.Names.Where(n => n.FullName.IndexOf(searchText) != -1);
                totalCount = names.Count();
                results.AddRange(names.Take(maxRows).OrderBy(n => n.FullName));
            }
            catch (Exception ex)
            {
                message = "Error : " + ex.Message;
            }

            return results;
        }
    }
}
