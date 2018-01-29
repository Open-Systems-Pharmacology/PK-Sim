using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Infrastructure.ProjectConverter;
using PKSim.Infrastructure.ProjectConverter.v5_2;
using PKSim.IntegrationTests;
using OSPSuite.Core.Domain;

namespace PKSim.ProjectConverter.v5_2
{
   public abstract class concern_for_FormulaAndDimensionConverter : ContextWithLoadedProject<Converter514To521>
   {
      public override void GlobalContext()
      {
         base.GlobalContext();
         LoadProject("SimulationSettings_515");
      }
   }

   public class When_converting_the_project_formula_and_dimension : concern_for_FormulaAndDimensionConverter
   {
      private Simulation _simulation;
      private SimpleProtocol _protocol;
      private Compound _compound;

      protected override void Context()
      {
         _simulation = First<Simulation>();
         _protocol = First<SimpleProtocol>();
         _compound = First<Compound>();
      }

      [Observation]
      public void should_have_converted_the_mol_weight_of_the_compound_in_the_building_block_and_in_the_simulation()
      {
         _compound.Parameter(Constants.Parameters.MOL_WEIGHT).ValueInDisplayUnit.ShouldBeEqualTo(250, 1e-2);
         var simCompound = _simulation.BuildingBlock<Compound>();
         simCompound.Parameter(Constants.Parameters.MOL_WEIGHT).ValueInDisplayUnit.ShouldBeEqualTo(250, 1e-2);
         var modelComp = _simulation.Model.Root.Container(_compound.Name);
         modelComp.Parameter(Constants.Parameters.MOL_WEIGHT).ValueInDisplayUnit.ShouldBeEqualTo(250, 1e-2);
      }

      [Observation]
      public void should_have_converted_the_dose_in_the_protocol()
      {
         _protocol.Dose.Value.ShouldBeEqualTo(1e-6);
         var simProtocol = _simulation.BuildingBlock<SimpleProtocol>();
         simProtocol.Dose.Value.ShouldBeEqualTo(1e-6);
         var simApp = _simulation.Model.Root.Container(Constants.APPLICATIONS)
            .GetAllChildren<IParameter>().First(x => x.IsNamed(CoreConstants.Parameters.DOSE_PER_BODY_WEIGHT));
         simApp.Value.ShouldBeEqualTo(1e-6);
      }

      [Observation]
      public void should_have_converted_the_parameter_in_nano_length_dimension_in_the_accurate_value()
      {
         _compound.Parameter(ConverterConstants.Parameter.ParticleRadiusDissolved).ValueInDisplayUnit.ShouldBeEqualTo(0.01, 1e-2);
         var simCompound = _simulation.BuildingBlock<Compound>();
         simCompound.Parameter(ConverterConstants.Parameter.ParticleRadiusDissolved).ValueInDisplayUnit.ShouldBeEqualTo(0.01, 1e-2);
         var modelComp = _simulation.Model.Root.Container(_compound.Name);
         modelComp.Parameter(ConverterConstants.Parameter.ParticleRadiusDissolved).ValueInDisplayUnit.ShouldBeEqualTo(0.01, 1e-2);
      }
   }
}