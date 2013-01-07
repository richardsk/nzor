using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;

namespace OAIServer
{
    [Serializable()]    
    public class ExpiringListItem 
    {
        public Object Key = null;
        public Object Value = null;
        public DateTime Expiry = DateTime.MaxValue;

        public ExpiringListItem()
        {
        }

        public ExpiringListItem(Object key, Object val, DateTime expiry)
        {
            this.Key = key;
            this.Value = val;
            this.Expiry = expiry;
        }
    }

    [Serializable()]
    public class ExpiringList 
    {
        public String Name = "_x_";
        public List<ExpiringListItem> Items = new List<ExpiringListItem>();

        public ExpiringList()
        {
        }
   
        public ExpiringList(String name)
        {
            this.Name = name;
        }

        public void Add(ExpiringListItem eli)
        {
            Items.Add(eli);
        }

        public ExpiringListItem Get(Object key)
        {
            ExpiringListItem eli = null;

            foreach (ExpiringListItem e in Items)
            {
                if (e.Key.ToString() == key.ToString())
                {
                    eli = e;
                    break;
                }
            }

            return eli;
        }

        public static ExpiringList Load(String dir, String name)
        {
            System.IO.FileStream fs = new FileStream(Path.Combine(dir, name + ".exl"), FileMode.Open);
            ExpiringList el = null;

            try
            {
                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                el = (ExpiringList)bf.Deserialize(fs);
            }
            catch (Exception )
            {
            }

            fs.Close();

            return el;
        }

        public void Save(String dir)
        {
            //remove expired items
            for (int i = Items.Count - 1; i >= 0; i--)
            {
                ExpiringListItem eli = Items[i];
                if (eli.Expiry < DateTime.Now)
                {
                    Items.RemoveAt(i);
                }
            }

            FileStream fs = new FileStream(Path.Combine(dir, Name + ".exl"), FileMode.Create);

            try
            {
                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                bf.Serialize(fs, this);

            }
            catch (Exception )
            {
            }

            fs.Close();
        }

    }
}
