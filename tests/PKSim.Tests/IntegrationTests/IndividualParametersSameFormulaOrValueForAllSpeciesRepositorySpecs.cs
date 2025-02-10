using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Container;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Infrastructure.ORM.Repositories;

namespace PKSim.IntegrationTests
{
   public abstract class concern_for_IndividualParametersSameFormulaOrValueForAllSpeciesRepository : ContextForIntegration<IIndividualParametersSameFormulaOrValueForAllSpeciesRepository>
   {
   }

   public class When_retrieving_parameters_with_the_same_formula_or_value_for_all_species_from_the_repository : concern_for_IndividualParametersSameFormulaOrValueForAllSpeciesRepository
   {
      private IEnumerable<IndividualParameterSameFormulaOrValueForAllSpecies> _result;

      protected override void Because()
      {
         _result = sut.All();
      }

      [Observation]
      public void should_return_at_least_one_parameter()
      {
         _result.Count().ShouldBeGreaterThan(0);
      }
   }

   public class When_testing_if_a_parameter_has_the_same_formula_or_value_for_all_species : concern_for_IndividualParametersSameFormulaOrValueForAllSpeciesRepository
   {
      private IFlatContainerRepository _flatContainerRepository;

      protected override void Context()
      {
         base.Context();
         _flatContainerRepository = IoC.Resolve<IFlatContainerRepository>();
      }

      private ParameterMetaData parameterMetaDataFor(string containerPath, string parameterName)
      {
         var containerId = _flatContainerRepository.ContainerFrom(containerPath).Id;

         return new ParameterMetaData
         {
            ContainerId = containerId,
            ParameterName = parameterName
         };
      }

      private bool isSameFormulaOrValue(string containerPath, string parameterName)
      {
         var parameterMetaData = parameterMetaDataFor(containerPath, parameterName);
         return sut.IsSameFormulaOrValue(parameterMetaData);
      }

      protected static IEnumerable<object[]> TestData()
      {
         yield return new object[]
         {
            "Organism", "Weight", true
         };

         yield return new object[]
         {
            "Organism", "Height", false
         };

         yield return new object[]
         {
            "Organism|Bone", "Volume", false
         };

         yield return new object[]
         {
            "Organism|Lumen|Duodenum", "Length", false
         };

         yield return new object[]
         {
            "Organism|Bone", "Organ volume mouse", true
         };

         //This is a special parameter that is defined as same in the table but in fact it is a table parameter and needs to be treated as species specific
         yield return new object[]
         {
            "Organism", "Ontogeny factor (alpha1-acid glycoprotein) table", false
         };
      }

      [Observation]
      [TestCaseSource(nameof(TestData))]
      public void should_return_correct_values(string containerPath, string parameterName, bool isSame)
      {
         var same = isSameFormulaOrValue(containerPath, parameterName);
         same.ShouldBeEqualTo(isSame, $"Formula equality not as expected for {containerPath} {parameterName}");
      }
   }
}