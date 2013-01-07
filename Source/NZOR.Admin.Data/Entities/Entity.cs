using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NZOR.Admin.Data.Entities
{
    public class Entity
    {
        public enum EntityState
        {
            Added,
            Modified,
            Deleted,
            Unchanged
        }

        private EntityState _entityState;

        public Entity()
        {
            _entityState = Entity.EntityState.Unchanged;
        }

        public EntityState State
        {
            get { return _entityState; }
            set { _entityState = value; }
        }
    }
}
