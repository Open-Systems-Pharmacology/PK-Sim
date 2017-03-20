using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Infrastructure.ORM.FlatObjects;

namespace PKSim.Infrastructure.ORM.Mappers
{
   public interface IFlatPVVListToPVVCategoryListMapper
   {
      IEnumerable<ParameterValueVersionCategory> MapFrom(IEnumerable<FlatParameterValueVersion> flatPVVCategories, string speciesName);
   }

   public class FlatPVVListToPVVCategoryListMapper : IFlatPVVListToPVVCategoryListMapper
   {
      private readonly IParameterValueVersionRepository _parameterValueVersionRepository;

      public FlatPVVListToPVVCategoryListMapper(IParameterValueVersionRepository parameterValueVersionRepository)
      {
         _parameterValueVersionRepository = parameterValueVersionRepository;
      }

      public IEnumerable<ParameterValueVersionCategory> MapFrom(IEnumerable<FlatParameterValueVersion> flatParameterValueVersions, string speciesName)
      {
         var pvvCategories = new List<ParameterValueVersionCategory>();
         var flatPvvForSpecies = from flatPVV in flatParameterValueVersions
                                 where flatPVV.Species == speciesName
                                 select flatPVV;

         flatPvvForSpecies = flatPvvForSpecies.ToList();
         var pvvCategoryNames = (from flatPVV in flatPvvForSpecies
                                 select flatPVV.Category).Distinct();

         foreach (var pvvCategoryName in pvvCategoryNames)
         {
            var pvvCategory = mapCategoryFrom(flatPvvForSpecies, pvvCategoryName);
            pvvCategories.Add(pvvCategory);
         }

         return pvvCategories;
      }

      private ParameterValueVersionCategory mapCategoryFrom(IEnumerable<FlatParameterValueVersion> flatPvvCategories, string categoryName)
      {
         var pvvsForCategory = from flatPVV in flatPvvCategories
                               where flatPVV.Category == categoryName
                               select flatPVV;

         var pvvCategory = new ParameterValueVersionCategory {Name = categoryName};

         pvvsForCategory.Each(flatPvv => pvvCategory.Add(_parameterValueVersionRepository.FindBy(flatPvv.Id)));
         return pvvCategory;
      }
   }
}