using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NZOR.Publish.Model.Names;
using NZOR.Publish.Model.Concepts;
using NZOR.Publish.Model.Common;

namespace NZOR.Web.ViewModels
{
    public class NameViewModel
    {
        public Name Name { get; private set; }

        public Feedback Feedback { get; private set; }

        public NameViewModel(Name name)
        {
            Name = name;
            Feedback = new Feedback();
        }

        public List<Relationship> AcceptedConceptRelationships
        {
            get
            {
                return Name.Concepts.SelectMany(o => o.FromRelationships).Where(o => o.Type.Equals("is synonym of", StringComparison.OrdinalIgnoreCase) && o.IsActive).ToList();
            }
        }

        public List<Relationship> SynonymConceptRelationships
        {
            get
            {
                return Name.Concepts.SelectMany(o => o.ToRelationships).Where(o => o.Type.Equals("is synonym of", StringComparison.OrdinalIgnoreCase) && o.IsActive).ToList();
            }
        }
    }
}