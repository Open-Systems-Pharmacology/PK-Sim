using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Infrastructure.ProjectConverter;
using PKSim.Infrastructure.ProjectConverter.v5_3;
using PKSim.IntegrationTests;
using OSPSuite.Core.Domain;

namespace PKSim.ProjectConverter.v5_3
{
   public class When_converting_the_simple_project_5_2_1 : ContextWithLoadedProject<Converter52To531>
   {
      private IParameter _liverVolume;
      private IParameter _fractionUnbound;

      public override void GlobalContext()
      {
         base.GlobalContext();
         LoadProject("SimpleProject_521");
         var individual = First<Individual>();
         var compound = First<Compound>();
         _liverVolume = individual.Organism.EntityAt<IParameter>(CoreConstants.Organ.Liver, Constants.Parameters.VOLUME);
         _fractionUnbound = compound.Parameter(CoreConstants.Parameter.FRACTION_UNBOUND_PLASMA_REFERENCE_VALUE);
      }

      [Observation]
      public void should_have_updated_the_dimension_info_on_each_parameter()
      {
         _liverVolume.Dimension.Name.ShouldBeEqualTo(Constants.Dimension.VOLUME);
         _fractionUnbound.Dimension.Name.ShouldBeEqualTo(CoreConstants.Dimension.Fraction);
      }

      [Observation]
      public void should_have_the_building_block_info_on_each_parameter()
      {
         _liverVolume.BuildingBlockType.ShouldBeEqualTo(PKSimBuildingBlockType.Individual);
         _fractionUnbound.BuildingBlockType.ShouldBeEqualTo(PKSimBuildingBlockType.Compound);
      }

      [Observation]
      public void should_have_converted_the_model_in_the_simulation()
      {
         var simulation = First<Simulation>();
         simulation.Model.ShouldNotBeNull();
      }

      [Observation]
      public void should_have_converted_the_simulation_output_in_the_simulation()
      {
         var simulation = First<Simulation>();
         simulation.OutputSchema.ShouldNotBeNull();
      }

      [Observation]
      public void should_have_added_the_missing_solver_parameters()
      {
         var simulation = First<Simulation>();
         simulation.Solver.HMin.ShouldNotBeNull();
         simulation.Solver.HMax.ShouldNotBeNull();
         simulation.Solver.MxStep.ShouldNotBeNull();
         simulation.Solver.H0.ShouldNotBeNull();
      }

      [Observation]
      public void should_have_set_the_has_halogen_parameter_to_visible()
      {
         var compound = First<Compound>();
         compound.Parameter(CoreConstants.Parameter.HAS_HALOGENS).Visible.ShouldBeTrue();

         var simulation = First<Simulation>();
         var simCompound = simulation.BuildingBlock<Compound>();
         simCompound.Parameter(CoreConstants.Parameter.HAS_HALOGENS).Visible.ShouldBeTrue();

         var simParameter = simulation.Model.Root.Container(compound.Name).Parameter(CoreConstants.Parameter.HAS_HALOGENS);
         simParameter.Visible.ShouldBeTrue();
      }

      [Observation]
      public void should_have_moved_the_drug_mass_parameter_under_the_global_molecule_container()
      {
         var simulation = First<Simulation>();
         var compound = First<Compound>();
         simulation.TotalDrugMassFor(compound.Name).ShouldNotBeNull();
      }


      [Observation]
      public void should_have_removed_the_drug_mass_parameter_from_the_application_container()
      {
         var simulation = First<Simulation>();
         var applications = simulation.Model.Root.Container(Constants.APPLICATIONS);
         applications.Parameter(ConverterConstants.Parameter.TotalDrugMass).ShouldBeNull();
         applications.Parameter(CoreConstants.Parameter.TotalDrugMass).ShouldBeNull();
      }

      [Observation]
      public void should_have_added_the_tag_molecule_to_all_drug_mass_parameters()
      {
         var simulation = First<Simulation>();
         var allDrugMassParameters = simulation.ApplicationsContainer.GetAllChildren<IParameter>(x => x.IsNamed(Constants.DRUG_MASS));
         allDrugMassParameters.Each(x=>x.Tags.Contains(CoreConstants.Tags.MOLECULE).ShouldBeTrue());
      }

      [Observation]
      public void should_have_changed_the_path_to_total_drug_mass_in_all_observers()
      {
         var simulation = First<Simulation>();
         var compound = First<Compound>();
         var observer = simulation.All<IObserver>().FindByName(CoreConstants.Observer.FRACTION_EXCRETED_TO_BILE);
         var objectPath = observer.Formula.ObjectPaths.First(x => x.Alias == ConverterConstants.Parameter.TotalDrugMass);
         objectPath.ShouldOnlyContainInOrder(simulation.Name, compound.Name, CoreConstants.Parameter.TotalDrugMass);
      }
   }


}	