using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Container;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Infrastructure;
using OSPSuite.Core.Domain;
using IContainer = OSPSuite.Core.Domain.IContainer;
using SimulationRunOptions = PKSim.Core.Services.SimulationRunOptions;

namespace PKSim.IntegrationTests
{
   public abstract class concern_for_Mucosa_Permeability_Scaling : ContextForIntegration<Simulation>
   {
      protected IList<CalculationMethod> _intestinalPermeabilityCalcMethods;
      protected ISimulationEngine<IndividualSimulation> _simulationEngine;
      protected const string _intestinalPermAlternativeName = "P1";

      protected Compound _compound;
      protected Individual _individual;
      protected Protocol _protocol;
      protected IParameter _alternativeIntestinalPermParam;
      protected SimulationRunOptions _simulationRunOptions;

      public override void GlobalContext()
      {
         base.GlobalContext();

         _compound = DomainFactoryForSpecs.CreateStandardCompound();
         _individual = DomainFactoryForSpecs.CreateStandardIndividual();
         _protocol = DomainFactoryForSpecs.CreateStandardIVBolusProtocol();

         var cmRepo = IoC.Resolve<ICalculationMethodRepository>();
         var intestinalPermGroup = _compound.ParameterAlternativeGroup(CoreConstants.Groups.COMPOUND_INTESTINAL_PERMEABILITY);
         var paramAlternativeFactory = IoC.Resolve<IParameterAlternativeFactory>();
         var alternative = paramAlternativeFactory.CreateAlternativeFor(intestinalPermGroup).WithName(_intestinalPermAlternativeName);
         intestinalPermGroup.AddAlternative(alternative);

         _intestinalPermeabilityCalcMethods = cmRepo.Where(cm => cm.Category.Equals(CoreConstants.Category.IntestinalPermeability)).ToList();
         _alternativeIntestinalPermParam = alternative.Parameter(CoreConstants.Parameter.SpecificIntestinalPermeability);
         _simulationEngine = IoC.Resolve<ISimulationEngine<IndividualSimulation>>();
         _simulationRunOptions = new SimulationRunOptions();
      }

      protected IndividualSimulation CreateSimulationWithCalculatedSpecificPintFor(CalculationMethod intestinalPermeabilityMethod)
      {
         var modelProperties = DomainFactoryForSpecs.CreateDefaultModelPropertiesFor(_individual);

         _compound.RemoveCalculationMethodFor(CoreConstants.Category.IntestinalPermeability);
         _compound.AddCalculationMethod(intestinalPermeabilityMethod);
     
         var simulation = DomainFactoryForSpecs.CreateModelLessSimulationWith(_individual, _compound, _protocol, modelProperties).DowncastTo<IndividualSimulation>();

         DomainFactoryForSpecs.AddModelToSimulation(simulation);
         return simulation;
      }

      protected IndividualSimulation CreateSimulationWithSetSpecificPintFor(CalculationMethod intestinalPermeabilityMethod)
      {
         var modelProperties = DomainFactoryForSpecs.CreateDefaultModelPropertiesFor(_individual);
         _compound.RemoveCalculationMethodFor(CoreConstants.Category.IntestinalPermeability);
         _compound.AddCalculationMethod(intestinalPermeabilityMethod);

         _alternativeIntestinalPermParam.Value = 1.2345E-3;

         var simulation = DomainFactoryForSpecs.CreateModelLessSimulationWith(_individual, _compound, _protocol, modelProperties).DowncastTo<IndividualSimulation>();
         simulation.CompoundPropertiesList.First().CompoundGroupSelections.First(
            cgs => cgs.GroupName.Equals(CoreConstants.Groups.COMPOUND_INTESTINAL_PERMEABILITY))
            .AlternativeName = _intestinalPermAlternativeName;

         DomainFactoryForSpecs.AddModelToSimulation(simulation);
         return simulation;
      }

   
      protected double MucosaPermeabilityFactorFor(Simulation simulation)
      {
         return compoundContainer(simulation).Parameter(CoreConstants.Parameter.MUCOSA_PERMEABILITY_SCALE_FACTOR).Value;
      }

      protected IParameter IntestinalPermeabilityParameter(Simulation simulation)
      {
         return compoundContainer(simulation).Parameter(CoreConstants.Parameter.INTESTINAL_PERMEABILITY);
      }

      private IContainer compoundContainer(Simulation simulation)
      {
         return simulation.Model.Root.Container(_compound.Name);
      }
   }

   public class When_creating_simulation_using_calculated_intestinal_permeability : concern_for_Mucosa_Permeability_Scaling
   {
      [Observation]
      public async Task mucosa_permeability_scale_factor_should_be_equal_to_one_for_every_calculation_method_and_simulation_should_run()
      {
         foreach (var intestinalPermeabilityCalcMethod in _intestinalPermeabilityCalcMethods)
         {
            var simulation = CreateSimulationWithCalculatedSpecificPintFor(intestinalPermeabilityCalcMethod);

            var scaleFactor = MucosaPermeabilityFactorFor(simulation);
            scaleFactor.ShouldBeEqualTo(1, 1e-5);

            await _simulationEngine.RunAsync(simulation, _simulationRunOptions);
            simulation.HasResults.ShouldBeTrue();
            Unregister(simulation);
         }
      }
   }

   public class When_creating_simulation_using_calculated_intestinal_permeability_and_overriding_intestinal_permeability_in_the_simulation : concern_for_Mucosa_Permeability_Scaling
   {
      [Observation]
      public async Task mucosa_permeability_scale_factor_should_be_set_to_correct_value_for_every_calculation_method_and_simulation_should_run()
      {
         const double intPermScaleFactor = 3;

         foreach (var intestinalPermeabilityCalcMethod in _intestinalPermeabilityCalcMethods)
         {
            var simulation = CreateSimulationWithCalculatedSpecificPintFor(intestinalPermeabilityCalcMethod);

            var intestinalPermParam = IntestinalPermeabilityParameter(simulation);
            intestinalPermParam.Value = intestinalPermParam.Value * intPermScaleFactor;

            var scaleFactor = MucosaPermeabilityFactorFor(simulation);
            scaleFactor.ShouldBeEqualTo(intPermScaleFactor, 1e-5);

            await _simulationEngine.RunAsync(simulation, _simulationRunOptions);
            simulation.HasResults.ShouldBeTrue();
            Unregister(simulation);
         }
      }
   }

   public class When_creating_simulation_using_non_calculated_intestinal_permeability : concern_for_Mucosa_Permeability_Scaling
   {
      [Observation]
      public async Task mucosa_permeability_scale_factor_should_be_set_to_correct_value_for_every_calculation_method_and_simulation_should_run()
      {
         foreach (var intestinalPermeabilityCalcMethod in _intestinalPermeabilityCalcMethods)
         {
            //create simualtion based on default (calculated) alternative of specific intestinal permeability
            //in order to obtain the value of default intestinal permeability in the simulation
            var simBasedOnCalculatedPInt = CreateSimulationWithCalculatedSpecificPintFor(intestinalPermeabilityCalcMethod);
            double defaultPIntInSimulation = IntestinalPermeabilityParameter(simBasedOnCalculatedPInt).Value;

            //create simulation based on non-calculated value of specific point and get intestinal perm in the sim
           var simulation = CreateSimulationWithSetSpecificPintFor(intestinalPermeabilityCalcMethod);
           double PIntInSimulation = IntestinalPermeabilityParameter(simulation).Value;

            //calculate expected mucosa permeability factor
            double intPermScaleFactor = PIntInSimulation/defaultPIntInSimulation;
            intPermScaleFactor.ShouldNotBeEqualTo(1.0);

            var scaleFactor = MucosaPermeabilityFactorFor(simulation);
            scaleFactor.ShouldBeEqualTo(intPermScaleFactor, 1e-5);

            await _simulationEngine.RunAsync(simulation, _simulationRunOptions);
            simulation.HasResults.ShouldBeTrue();
            Unregister(simulation);
         }
      }
   }
}