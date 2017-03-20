using OSPSuite.Utility;
using PKSim.Core.Model;
using PKSim.Core.Repositories;

namespace PKSim.Core.Mappers
{
   public interface ICompoundToCompoundPropertiesMapper : IMapper<Compound, CompoundProperties>
   {
   }

   public class CompoundToCompoundPropertiesMapper : ICompoundToCompoundPropertiesMapper
   {
      private readonly ICompoundCalculationMethodCategoryRepository _categoryRepository;

      public CompoundToCompoundPropertiesMapper(ICompoundCalculationMethodCategoryRepository categoryRepository)
      {
         _categoryRepository = categoryRepository;
      }

      public CompoundProperties MapFrom(Compound compound)
      {
         var compoundProperties = new CompoundProperties {Compound = compound};

         foreach (var category in _categoryRepository.All())
         {
            var compoundCalculationMethod = compound.CalculationMethodFor(category) ?? category.DefaultItem;
            compoundProperties.AddCalculationMethod(compoundCalculationMethod);
         }

         //update default mapping for alternatives in compound
         foreach (var alternativeGroup in compound.AllParameterAlternativeGroups())
         {
            var compoundGroupSelection = new CompoundGroupSelection {AlternativeName = alternativeGroup.DefaultAlternative.Name, GroupName = alternativeGroup.Name};
            compoundProperties.AddCompoundGroupSelection(compoundGroupSelection);
         }

         return compoundProperties;
      }
   }
}