using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Mappers;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Services;

namespace PKSim.Core
{
   public abstract class concern_for_SimulationToModelCoreSimulationMapper : ContextSpecification<ISimulationToModelCoreSimulationMapper>
   {
      protected IIdGenerator _idGenerator;
      protected ICloneManagerForBuildingBlock _cloneManagerForBuildingBlock;
      protected ICloneManagerForModel _cloneManagerForModel;

      protected override void Context()
      {
         _idGenerator = A.Fake<IIdGenerator>();
         _cloneManagerForBuildingBlock= A.Fake<ICloneManagerForBuildingBlock>();
         _cloneManagerForModel= A.Fake<ICloneManagerForModel>();
         sut = new SimulationToModelCoreSimulationMapper(_idGenerator, _cloneManagerForBuildingBlock,_cloneManagerForModel);
      }
   }

   public class When_mapping_a_simulation_to_a_model_core_simulation : concern_for_SimulationToModelCoreSimulationMapper
   {
      private Simulation _simulation;
      private IModelCoreSimulation _coreSimulation;
      private ISimulationSettings _cloneSettings;

      protected override void Context()
      {
         base.Context();
         _simulation = A.Fake<Simulation>().WithName("MyName");
         _simulation.Model = A.Fake<IModel>();
         _cloneSettings= A.Fake<ISimulationSettings>();
         A.CallTo(() => _idGenerator.NewId()).Returns("id");

         A.CallTo(() => _cloneManagerForBuildingBlock.Clone(_simulation.SimulationSettings)).Returns(_cloneSettings);
      }

      protected override void Because()
      {
         _coreSimulation = sut.MapFrom(_simulation,shouldCloneModel:false);
      }

      [Observation]
      public void should_return_a_model_core_simulation_with_the_model_defined_in_the_simulation()
      {
         _coreSimulation.Model.ShouldBeEqualTo(_simulation.Model);
      }

      [Observation]
      public void should_return_a_model_core_simulation_whose_name_was_set_to_the_simulation_name()
      {
         _coreSimulation.Name.ShouldBeEqualTo(_simulation.Name);
      }

      [Observation]
      public void should_have_set_a_defined_id_in_the_model_simulation()
      {
         _coreSimulation.Id.ShouldBeEqualTo("id");
      }

      [Observation]
      public void should_have_clone_the_simulation_settings()
      {
         _coreSimulation.BuildConfiguration.SimulationSettings.ShouldBeEqualTo(_cloneSettings);
      }
   }
}