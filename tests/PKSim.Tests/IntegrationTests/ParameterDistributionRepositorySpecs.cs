using System.Collections.Generic;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;

namespace PKSim.IntegrationTests
{
   public abstract class concern_for_ParameterDistributionRepository : ContextForIntegration<IParameterDistributionRepository>
   {
   }

   public class When_retrieving_all_parameter_distributions_from_the_repository : concern_for_ParameterDistributionRepository
   {
      private IEnumerable<ParameterDistributionMetaData> _result;

      protected override void Because()
      {
         _result = sut.All();
      }

      [Observation]
      public void should_return_at_least_one_parameter()
      {
         _result.Count().ShouldBeGreaterThan(0);
      }

      [Observation]
      public void all_parameters_should_have_at_least_one_value_with_GA_40()
      {
         var groupByPVV = _result.GroupBy(x => x.ParameterValueVersion);
         var errorList = new List<string>();
         foreach (var groupPVV in groupByPVV)
         {
            var groupByPopulation = groupPVV.GroupBy(x => x.Population);
            foreach (var groupPop in groupByPopulation)
            {
               var groupByGender = groupPop.GroupBy(x => x.Gender);
               foreach (var groupGender in groupByGender)
               {
                  var groupByContainer = groupGender.GroupBy(x => x.ContainerId);
                  foreach (var groupContainer in groupByContainer)
                  {
                     var groupByParameter = groupContainer.GroupBy(x => x.ParameterName);
                     foreach (var groupParameter in groupByParameter)
                     {
                       var allGA = groupParameter.Select(x => x.GestationalAge).ToList();
                       var metaData = groupParameter.First();
                       if (!allGA.Contains(CoreConstants.NOT_PRETERM_GESTATIONAL_AGE_IN_WEEKS))
                       {
                        errorList.Add(string.Format("Gestional age 40 not found for parameter {0} in {1} - {2} - {3}",
                                                          metaData.ParameterName, metaData.Population, metaData.Gender, metaData.ContainerName));
                        }
                     }
                  }
               }
            }
         }
         errorList.Count().ShouldBeEqualTo(0, errorList.ToString("\n"));
      }
   }
}