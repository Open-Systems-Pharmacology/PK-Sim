using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Container;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Infrastructure;
using PKSim.Infrastructure.ProjectConverter;
using OSPSuite.Core.Domain;

namespace PKSim.IntegrationTests
{
   public class When_serializing_a_compound_with_an_alternative_defined_for_a_permeability : ContextForSerialization<Compound>
   {
      private Compound _compound;
      private Compound _deserializedCompound;

      protected override void Context()
      {
         base.Context();
         var repository = IoC.Resolve<ICompoundCalculationMethodCategoryRepository>();

         _compound = DomainFactoryForSpecs.CreateStandardCompound();
         var compoundAlternativeTask = IoC.Resolve<IParameterAlternativeFactory>();
         _compound.RemoveCalculationMethodFor(CoreConstants.Category.DistributionCellular);
         var category = repository.FindByName(CoreConstants.Category.DistributionCellular);
         _compound.AddCalculationMethod(category.AllItems().First(x => Equals(x.Name, ConverterConstants.CalculationMethod.BER)));
         var permGroup = _compound.ParameterAlternativeGroup(CoreConstants.Groups.COMPOUND_PERMEABILITY);
         var alternative = compoundAlternativeTask.CreateAlternativeFor(permGroup).WithName("toto");
         permGroup.AddAlternative(alternative);
      }

      protected override void Because()
      {
         _deserializedCompound = SerializeAndDeserialize(_compound);
      }

      [Observation]
      public void compound_should_contain_the_calculation_method()
      {
         _deserializedCompound.CalculationMethodFor(CoreConstants.Category.DistributionCellular).ShouldNotBeNull();
         _deserializedCompound.CalculationMethodFor(CoreConstants.Category.DistributionCellular).Name.ShouldBeEqualTo("Cellular partition coefficient method - Berezhkovskiy");
      }

      [Observation]
      public void should_be_able_to_deserialize_the_compound()
      {
         _deserializedCompound.ShouldNotBeNull();
      }
   }
}