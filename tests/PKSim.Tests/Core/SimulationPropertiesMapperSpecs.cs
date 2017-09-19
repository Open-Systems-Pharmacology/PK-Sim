using System.Threading.Tasks;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Core.Snapshots;
using PKSim.Core.Snapshots.Mappers;

namespace PKSim.Core
{
   public abstract class concern_for_SimulationPropertiesMapper : ContextSpecificationAsync<SimulationPropertiesMapper>
   {
      protected SimulationProperties _simulationProperties;
      protected string _modelName = "MODE_NAME";
      protected SimulationConfiguration _snapshot;
      private IModelPropertiesTask _modelPropertiesTask;

      protected override Task Context()
      {
         _modelPropertiesTask= A.Fake<IModelPropertiesTask>();
         sut = new SimulationPropertiesMapper(_modelPropertiesTask);

         _simulationProperties = new SimulationProperties
         {
            AllowAging = true,
            ModelProperties =
            {
               ModelConfiguration = new ModelConfiguration
               {
                  ModelName = _modelName
               }
            }
         };

         return Task.FromResult(true);
      }
   }

   public class When_mapping_simulation_properties_to_configuration_snapshot : concern_for_SimulationPropertiesMapper
   {
      protected override async Task Because()
      {
         _snapshot = await sut.MapToSnapshot(_simulationProperties);
      }

      [Observation]
      public void should_set_the_model_name_into_the_snapshot()
      {
         _snapshot.Model.ShouldBeEqualTo(_modelName);
      }

      [Observation]
      public void should_save_the_allow_aging_flag_into_the_snapshot()
      {
         _snapshot.AllowAging.ShouldBeEqualTo(_simulationProperties.AllowAging);
      }
   }
}