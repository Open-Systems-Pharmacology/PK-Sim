using OSPSuite.Utility.Collections;

namespace PKSim.Infrastructure.ProjectConverter
{
   public interface IObjectConverterFinder
   {
      IObjectConverter FindConverterFor(int version);
   }

   public class ObjectConverterFinder : IObjectConverterFinder
   {
      private readonly IRepository<IObjectConverter> _objectConverters;

      public ObjectConverterFinder(IRepository<IObjectConverter> objectConverters)
      {
         _objectConverters = objectConverters;
      }

      public IObjectConverter FindConverterFor(int version)
      {
         foreach (var objectConverter in _objectConverters.All())
         {
            if (objectConverter.IsSatisfiedBy(version))
               return objectConverter;
         }

         return new NullConverter();
      }
   }
}