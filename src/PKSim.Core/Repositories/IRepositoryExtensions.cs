using System;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility.Collections;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Repositories
{
   public static class RepositoryExtensions
   {
      public static T SelectFirst<T>(this IRepository<T> repository, Func<T, bool> specification)
      {
         return repository.All().FirstOrDefault(specification);
      }

      public static IEnumerable<T> Where<T>(this IRepository<T> repository, Func<T, bool> specification)
      {
         return repository.All().Where(specification);
      }

      public static bool ExistsByName<T>(this IRepository<T> repository, string name) where T : class, IWithName
      {
         return repository.All().ExistsByName(name);
      }

      public static T FindByName<T>(this IRepository<T> repository, string name) where T : class, IWithName
      {
         return repository.All().FindByName(name);
      }

      public static T FindById<T>(this IRepository<T> repository, string id) where T : class, IWithId
      {
         return repository.All().FindById(id);
      }

      public static int Count<T>(this IRepository<T> repository)
      {
         return repository.All().Count();
      }

      public static IEnumerable<string> AllNames<T>(this IRepository<T> repository) where T : class, IObjectBase
      {
         return repository.All().AllNames();
      }
   }
}