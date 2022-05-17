using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Utility.Container;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Infrastructure;
using PKSim.Infrastructure.ProjectConverter;

namespace PKSim.IntegrationTests
{
   public class When_creating_a_parameter_start_value_building_block_for_a_simulation_whose_parameter_where_changed_from_default : ContextForSimulationIntegration<IPKSimParameterStartValuesCreator>
   {
      private IBuildConfigurationTask _buildConfigurationTask;
      private IEntityPathResolver _entityPathResolver;
      private IObjectPath _parameterPath;
      private IParameterStartValuesBuildingBlock _psv;
      private Individual _individual;
      private Compound _compound;
      private Protocol _protocol;
      private IndividualMolecule _enzyme;
      private PartialProcess _metabolizationProcess;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _buildConfigurationTask = IoC.Resolve<IBuildConfigurationTask>();
         _entityPathResolver = IoC.Resolve<IEntityPathResolver>();
         var enzymeFactory = IoC.Resolve<IIndividualEnzymeFactory>();
         var compoundProcessRepository = IoC.Resolve<ICompoundProcessRepository>();
         var cloneManager = IoC.Resolve<ICloneManager>();
         _compound = DomainFactoryForSpecs.CreateStandardCompound();
         _individual = DomainFactoryForSpecs.CreateStandardIndividual();
         _protocol = DomainFactoryForSpecs.CreateStandardIVBolusProtocol();
         _enzyme = enzymeFactory.AddMoleculeTo(_individual, "CYP").DowncastTo<IndividualEnzyme>();
         _metabolizationProcess = cloneManager.Clone(compoundProcessRepository
            .ProcessByName(CoreConstantsForSpecs.Process.METABOLIZATION_SPECIFIC_FIRST_ORDER).DowncastTo<PartialProcess>());
         _metabolizationProcess.Name = "My Partial Process";
         _metabolizationProcess.Parameter(ConverterConstants.Parameters.CLspec).Value = 15;
         _compound.AddProcess(_metabolizationProcess);

         _simulation = DomainFactoryForSpecs.CreateModelLessSimulationWith(_individual, _compound, _protocol)
            .DowncastTo<IndividualSimulation>();

         _simulation.CompoundPropertiesList.First()
            .Processes
            .MetabolizationSelection
            .AddPartialProcessSelection(new EnzymaticProcessSelection {ProcessName = _metabolizationProcess.Name, MoleculeName = _enzyme.Name});

         DomainFactoryForSpecs.AddModelToSimulation(_simulation);
      }

      protected override void Because()
      {
         var compoundName = _simulation.CompoundNames.First();
         var parameter = _simulation.Model.Root.EntityAt<IParameter>(compoundName, CoreConstantsForSpecs.Parameters.BLOOD_PLASMA_CONCENTRATION_RATIO);
         parameter.Value = 10;
         _parameterPath = _entityPathResolver.ObjectPathFor(parameter);
         _psv = _buildConfigurationTask.CreateFor(_simulation, shouldValidate: true, createAgingDataInSimulation: false).ParameterStartValues;
      }

      [Observation]
      public void should_have_created_one_entry_for_the_changed_parameter_in_the_parameter_start_value_building_block()
      {
         _psv[_parameterPath].ShouldNotBeNull();
         _psv[_parameterPath].StartValue.ShouldBeEqualTo(10);
      }

      [Observation]
      public void should_have_not_created_entries_for_property_parameter_of_the_enzyme()
      {
         var propertyParameters = _enzyme.AllParameters().Where(x => x.BuildMode == ParameterBuildMode.Property).ToList();
         propertyParameters.Count.ShouldBeGreaterThan(0);
         propertyParameters.Each(x =>
         {
            var parameterPath = _entityPathResolver.ObjectPathFor(x);
            _psv[parameterPath].ShouldBeNull();
         });
      }
   }
}