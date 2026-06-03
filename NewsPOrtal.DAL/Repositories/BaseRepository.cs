using NewsPOrtal.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Linq.Expressions;

namespace NewsPortal.DAL.Repositories
{
    public abstract class BaseRepository
    {
        protected readonly DbContext Context;

        public BaseRepository(DbContext context)
        {
            Context = context;


        }

    }
}
