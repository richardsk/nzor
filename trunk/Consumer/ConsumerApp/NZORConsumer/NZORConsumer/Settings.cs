using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NZORConsumer
{
    public class Settings
    {
        private static string _apiKey = null;

        public static string APIKey
        {
            get
            {
                if (_apiKey != null) return _apiKey;

                Model.ConsumerEntities ce = new Model.ConsumerEntities();
                Model.Setting apiSetting = ce.Settings.Where(s => s.Name == "APIKey").FirstOrDefault();
                if (apiSetting != null) _apiKey = apiSetting.Value;
                return _apiKey ?? null;
            }
            set
            {
                Model.ConsumerEntities ce = new Model.ConsumerEntities();
                Model.Setting apiSetting = ce.Settings.Where(s => s.Name == "APIKey").FirstOrDefault();
                if (apiSetting == null)
                {
                    apiSetting = new Model.Setting();
                    apiSetting.Name = "APIKey";
                    ce.Settings.AddObject(apiSetting);
                }
                apiSetting.Value = value;
                ce.SaveChanges();

                _apiKey = value;
            }
        }
    }
}
