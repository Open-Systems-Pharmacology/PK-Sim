using System.Collections.Generic;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;

namespace PKSim.IntegrationTests
{
   public abstract class concern_for_ParameterRateRepository : ContextForIntegration<IParameterRateRepository>
   {
   }

   public class When_retrieving_all_parameter_rates_from_the_repository : concern_for_ParameterRateRepository
   {
      private IEnumerable<ParameterRateMetaData> _result;

      protected override void Because()
      {
         _result = sut.All();
         _result = sut.All();
      }

      [Observation]
      public void should_return_at_least_one_element()
      {
         _result.Count().ShouldBeGreaterThan(0);
      }


      [Observation]
      public void should_have_set_the_default_flag_to_the_expected_parameters_for_some_know_parameters()
      {
         var lipophilicityParameter = _result.Find(x => x.BuildingBlockType == PKSimBuildingBlockType.Compound &&
                                                        x.ParameterName == CoreConstants.Parameters.LIPOPHILICITY);

         lipophilicityParameter.IsInput.ShouldBeTrue();
      }

      [Observation]
      public void should_expose_bile_salt_micelle_partition_coefficients_for_compound_dissolution()
      {
         var expectedParameters = new[]
         {
            "Partition coefficient (bile salt micelle/water) neutral",
            "Partition coefficient (bile salt micelle/water) ionized",
            "Partition coefficient (bile salt micelle/water) constant 1",
            "Partition coefficient (bile salt micelle/water) constant 2"
         };

         foreach (var parameterName in expectedParameters)
         {
            _result.Any(x => x.ContainerType == CoreConstants.ContainerType.COMPOUND && x.ParameterName == parameterName)
               .ShouldBeTrue($"Parameter '{parameterName}' should be available as compound rate parameter.");
         }
      }

      [Observation]
      public void should_expose_micellar_diffusion_coefficients_for_all_lumen_segments()
      {
         var expectedSegments = new[]
         {
            "Stomach",
            "Duodenum",
            "UpperJejunum",
            "LowerJejunum",
            "UpperIleum",
            "LowerIleum",
            "Caecum",
            "ColonAscendens",
            "ColonTransversum",
            "ColonDescendens",
            "ColonSigmoid",
            "Rectum"
         };

         var micellarDiffusionSegments = _result.Where(x =>
               x.ContainerType == CoreConstants.ContainerType.COMPARTMENT &&
               x.ParameterName == "Micellar diffusion coefficient in fasted state")
            .Select(x => x.ContainerName)
            .ToList();

         foreach (var segment in expectedSegments)
            micellarDiffusionSegments.ShouldContain(segment);
      }
   }
}
