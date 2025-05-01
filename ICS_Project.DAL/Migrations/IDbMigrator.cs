using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICS_Project.DAL.Migrations
{
    public interface IDbMigrator
    {
        public void Migrate();
    }
}
