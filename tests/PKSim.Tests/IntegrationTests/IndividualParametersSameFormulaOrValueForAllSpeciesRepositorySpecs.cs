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
      private readonly IFlatContainerRepository _flatContainerRepository = IoC.Resolve<IFlatContainerRepository>();

      private ParameterMetaData parameterMetaDataFor(string containerPath, string parameterName)
      {
         var containerId = _flatContainerRepository.ContainerFrom(containerPath).Id;

         return new ParameterMetaData()
         {
            ContainerId = containerId,
            ParameterName = parameterName
         };
      }

      private (bool isSameFormula, bool isSameValue) isSameFormulaOrValue(string containerPath, string parameterName)
      {
         return sut.IsSameFormulaOrValue(parameterMetaDataFor(containerPath, parameterName));
      }

      private bool isSameFormula(string containerPath, string parameterName)
      {
         return sut.IsSameFormula(parameterMetaDataFor(containerPath, parameterName));
      }

      private bool isSameValue(string containerPath, string parameterName)
      {
         return sut.IsSameValue(parameterMetaDataFor(containerPath, parameterName));
      }

      protected static IEnumerable<object[]> TestData()
      {
         yield return new object[]
         {
            "Organism", "Weight", (isSameFormula:true, isSameValue:false)
         };

         yield return new object[]
         {
            "Organism", "Height", (isSameFormula:false, isSameValue:false)
         };

         yield return new object[]
         {
            "Organism|Bone", "Volume", (isSameFormula:false, isSameValue:false)
         };

         yield return new object[]
         {
            "Organism|Lumen|Duodenum", "Length", (isSameFormula:false, isSameValue:false)
         };

         yield return new object[]
         {
            "Organism|Bone", "Organ volume mouse", (isSameFormula:false, isSameValue:true)
         };
      }

      [Observation]
      [TestCaseSource(nameof(TestData))]
      public void should_return_correct_values(string containerPath, string parameterName, 
         (bool isSameFormula, bool isSameValue) expectedValues)
      {
         var (sameFormula, sameValue) = isSameFormulaOrValue(containerPath, parameterName);

         sameFormula.ShouldBeEqualTo(expectedValues.isSameFormula,"Formula equality not as expected");
         sameValue.ShouldBeEqualTo(expectedValues.isSameValue, "Value equality not as expected");

         sameFormula.ShouldBeEqualTo(isSameFormula(containerPath, parameterName));
         sameValue.ShouldBeEqualTo(isSameValue(containerPath, parameterName));
      }
   }
}
