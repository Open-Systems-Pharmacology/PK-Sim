using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Container;
using FakeItEasy;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Infrastructure.ProjectConverter;
using PKSim.Infrastructure.ProjectConverter.v6_0;
using PKSim.IntegrationTests;
using PKSim.Presentation;
using OSPSuite.Core.Converter.v6_0;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Extensions;

namespace PKSim.ProjectConverter.V6_0
{
   public class When_converting_the_SimulationSettings_562_project : ContextWithLoadedProject<Converter562To601>
   {
      private Simulation _simulation;
      private Individual _individual;
      private SimpleProtocol _protocol;
      private Simulation _simulation2;

      public override void GlobalContext()
      {
         base.GlobalContext();
         LoadProject("SimulationSettings_562");
         _simulation = First<Simulation>();
         _simulation2 = FindByName<Simulation>("S2");
         _individual = First<Individual>();
         _protocol = First<SimpleProtocol>();
      }

      [Observation]
      public void should_have_converted_the_simulation_settings_of_the_simulation()
      {
         _simulation.SimulationSettings.ShouldNotBeNull();
      }

      [Observation]
      public void should_have_updated_the_reference_to_neighbors_in_all_neighborhoods_in_the_individual_building_block_of_the_simulation()
      {
         _simulation2.Individual.Neighborhoods.GetChildren<INeighborhood>().First().FirstNeighbor.ShouldNotBeNull();
      }

      [Observation]
      public void should_have_converted_the_solver_settings_of_the_simulation()
      {
         _simulation.Solver.ShouldNotBeNull();
         _simulation.Solver.HMax.ShouldBeEqualTo(120);
      }

      [Observation]
      public void should_have_converted_the_output_schema_of_the_simulation()
      {
         _simulation.OutputSchema.ShouldNotBeNull();
      }

      [Observation]
      public void should_have_set_the_dimension_of_relative_expression_in_individual_molecules_to_dimensionsless()
      {
         _individual.AllMolecules().SelectMany(x => x.AllExpressionsContainers())
            .Select(x => x.RelativeExpressionParameter)
            .Each(p => p.Dimension.ShouldBeEqualTo(Constants.Dimension.NO_DIMENSION));
      }

      [Observation]
      public void should_have_converted_the_favorites()
      {
         _project.Favorites.Any().ShouldBeTrue();
      }

      [Observation]
      public void should_have_updated_the_visibility_of_the_input_dose_parameter()
      {
         _protocol.Dose.Visible.ShouldBeTrue();
      }

      [Observation]
      public void should_have_changed_the_build_mode_of_property_parameter_in_the_compound_from_property_to_global()
      {
         var root = _simulation.Model.Root;
         foreach (var compoundName in _simulation.CompoundNames)
         {
            foreach (var parameterName in ConverterConstants.Parameter.AllCompoundGlobalParameters)
            {
               var parameter = root.EntityAt<IParameter>(compoundName, parameterName);
               parameter.BuildMode.ShouldBeEqualTo(ParameterBuildMode.Global);
            }
         }
      }
   }

   public class When_converting_some_user_settings_using_the_old_default_abs_tol_and_rel_tol : ContextForIntegration<Converter562To601>
   {
      private IUserSettings _userSettings;

      protected override void Context()
      {
         sut = new Converter562To601(IoC.Resolve<Converter56To601>(), IoC.Resolve<ICalculationMethodsUpdater>());
         _userSettings = A.Fake<IUserSettings>();
         _userSettings.AbsTol = ConverterConstants.OLD_DEFAULT_ABS_TOL;
         _userSettings.RelTol = ConverterConstants.OLD_DEFAULT_REL_TOL;
      }

      protected override void Because()
      {
         sut.Convert(_userSettings,ProjectVersions.V5_2_2);
      }

      [Observation]
      public void should_update_the_default_user_settings()
      {
         _userSettings.AbsTol.ShouldBeEqualTo(CoreConstants.DEFAULT_ABS_TOL);
         _userSettings.RelTol.ShouldBeEqualTo(CoreConstants.DEFAULT_REL_TOL);
      }
   }

   public class When_converting_some_user_settings_using_user_defined_abs_tol_or_rel_tol : ContextForIntegration<Converter562To601>
   {
      private IUserSettings _userSettings;

      protected override void Context()
      {
         sut = new Converter562To601(IoC.Resolve<Converter56To601>(), IoC.Resolve<ICalculationMethodsUpdater>());
         _userSettings = A.Fake<IUserSettings>();
         _userSettings.AbsTol = 10;
         _userSettings.RelTol = ConverterConstants.OLD_DEFAULT_REL_TOL;
      }

      protected override void Because()
      {
         sut.Convert(_userSettings, ProjectVersions.V5_1_4);
      }

      [Observation]
      public void should_update_the_user_settings_that_was_not_changed()
      {
         _userSettings.AbsTol.ShouldBeEqualTo(10);
         _userSettings.RelTol.ShouldBeEqualTo(CoreConstants.DEFAULT_REL_TOL);
      }
   }
}