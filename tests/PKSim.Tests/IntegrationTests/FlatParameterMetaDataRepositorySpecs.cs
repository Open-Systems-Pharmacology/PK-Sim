using System.Collections.Generic;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Container;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Infrastructure.ORM.Repositories;
using OSPSuite.Core.Domain;
using PKSim.Core.Repositories;

namespace PKSim.IntegrationTests
{
   //This tests ensure that the flags defined in the database for parameters are consistent
   public abstract class concern_for_FlatParameterMetaDataRepository : ContextForIntegration<IParametersInContainerRepository>
   {
      protected List<ParameterMetaData> _allParameters;

      public override void GlobalContext()
      {
         base.GlobalContext();
         sut = IoC.Resolve<IParametersInContainerRepository>();
         _allParameters = sut.All().ToList();
      }

      protected string ErrorMessageFor(IEnumerable<ParameterMetaData> parameters)
      {
         return parameters.Select(p => p.ToString()).ToString("\n");
      }
   }

   public class When_checking_the_database_consistency_regarding_parameter_flags : concern_for_FlatParameterMetaDataRepository
   {
      [Observation]
      public void all_hidden_parameters_should_be_read_only()
      {
         var parameters = _allParameters.Where(x => !x.Visible).Where(x => !x.ReadOnly).ToList();
         parameters.Any().ShouldBeFalse(ErrorMessageFor(parameters));
      }

      [Observation]
      public void all_read_only_parameters_should_not_be_variable_in_a_population()
      {
         var parameters = _allParameters.Where(x => x.ReadOnly).Where(x => x.CanBeVariedInPopulation).ToList();
         parameters.Any().ShouldBeFalse(ErrorMessageFor(parameters));
      }

      
      [Observation]
      public void all_parameters_variable_in_a_population_should_be_visible()
      {
         var parameters = _allParameters.Where(x => x.CanBeVariedInPopulation).Where(x => !x.Visible).ToList();
         parameters.Any().ShouldBeFalse(ErrorMessageFor(parameters));
      }

      [Observation]
      public void all_visible_and_editable_parameters_should_be_can_be_varied_except_mol_weight()
      {
         var parameters = _allParameters.Where(x => x.Visible)
            .Where(x => !x.ReadOnly)
            .Where(x => !x.CanBeVaried)
            .Where(x => x.ParameterName!=Constants.Parameters.MOL_WEIGHT).ToList();

         parameters.Any().ShouldBeFalse(ErrorMessageFor(parameters));
      }

      [Observation]
      public void all_readonly_parameter_should_be_marked_as_non_input()
      {
         var parameters = _allParameters.Where(x => x.ReadOnly)
            .Where(x => x.IsInput).ToList();

         parameters.Any().ShouldBeFalse(ErrorMessageFor(parameters));
      }
      [Observation]
      public void all_hidden_parameter_should_be_marked_as_non_input()
      {
         var parameters = _allParameters.Where(x => !x.Visible)
            .Where(x => x.IsInput).ToList();

         parameters.Any().ShouldBeFalse(ErrorMessageFor(parameters));
      }

   }
}