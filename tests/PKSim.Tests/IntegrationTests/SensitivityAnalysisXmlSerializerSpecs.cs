using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Container;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Infrastructure.Services;
using PKSim.Presentation.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.SensitivityAnalyses;

namespace PKSim.IntegrationTests
{
   public class When_serializing_a_sensitivity_analysis : ContextForSerialization<SensitivityAnalysis>
   {
      private SensitivityAnalysis _sensitivityAnalysis;
      private SensitivityParameter _sensitivityParameter1;
      private ParameterSelection _parameterSelection1;
      private IndividualSimulation _sim1;
      private PKSimProject _project;
      private SensitivityAnalysis _deserializedSensitivityAnalysis;

      public override void GlobalContext()
      {
         base.GlobalContext();

         _sim1 = new IndividualSimulation
         {
            Id = "Sim1",
            Name = "Sim1",
            IsLoaded = true,
            Model = new Model {Root = new Container()}
         };
         _sim1.Model.Root.Add(new Container {new Parameter().WithName("P")}.WithName("Liver"));

         var objectBaseRepository = IoC.Resolve<IWithIdRepository>();
         var workspace = IoC.Resolve<IWorkspace>();
         _project = IoC.Resolve<PKSimProject>();
         workspace.Project = _project;
         objectBaseRepository.Register(_sim1);
         _project.AddBuildingBlock(_sim1);

         _sensitivityAnalysis = new SensitivityAnalysis
         {
            Name = "SA",
            Simulation = _sim1
         };
         _sensitivityParameter1 = DomainHelperForSpecs.SensitivityParameter(range: 0.5d, steps: 5);
         _parameterSelection1 = new ParameterSelection(_sim1, new QuantitySelection("Liver|P", QuantityType.Parameter));
         _sensitivityParameter1.ParameterSelection = _parameterSelection1;

         _sensitivityAnalysis.AddSensitivityParameter(_sensitivityParameter1);

         _deserializedSensitivityAnalysis = SerializeAndDeserialize(_sensitivityAnalysis);
      }

      public override SensitivityAnalysis SerializeAndDeserialize(SensitivityAnalysis sensitivityAnalysis)
      {
         var stream = _serializationManager.Serialize(sensitivityAnalysis);
         var serializationContectFactory = IoC.Resolve<ISerializationContextFactory>();
         using (var context = serializationContectFactory.Create(externalReferences: _project.All<ISimulation>()))
         {
            return _serializationManager.Deserialize<SensitivityAnalysis>(stream, context);
         }
      }

      [Observation]
      public void should_be_able_to_deserialize_the_sensitivity_analysis_and_retrieve_the_simulation_used()
      {
         _deserializedSensitivityAnalysis.ShouldNotBeNull();
         _deserializedSensitivityAnalysis.Simulation.ShouldBeEqualTo(_sim1);
      }

      [Observation]
      public void should_be_able_to_deserialize_the_sensitivity_parameters_and_their_selected_parameters()
      {
         _deserializedSensitivityAnalysis.AllSensitivityParameters.Count.ShouldBeEqualTo(1);
         var deserializedSensitivityParameter = _deserializedSensitivityAnalysis.AllSensitivityParameters[0];

         deserializedSensitivityParameter.SensitivityAnalysis.ShouldBeEqualTo(_deserializedSensitivityAnalysis);

         deserializedSensitivityParameter.NumberOfStepsParameter.Value.ShouldBeEqualTo(_sensitivityParameter1.NumberOfStepsParameter.Value);
         deserializedSensitivityParameter.VariationRangeParameter.Value.ShouldBeEqualTo(_sensitivityParameter1.VariationRangeParameter.Value);
         deserializedSensitivityParameter.ParameterSelection.ShouldBeEqualTo(_parameterSelection1);
      }
   }
}