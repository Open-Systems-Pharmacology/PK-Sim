using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Container;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Infrastructure.Services;
using PKSim.Presentation.Core;
using OSPSuite.Core.Chart.ParameterIdentifications;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.ParameterIdentifications;

namespace PKSim.IntegrationTests
{
   public abstract class duplicated_parameter_identification_comparison_test : ContextForSerialization<ParameterIdentification>
   {
      protected ParameterIdentification _parameterIdentification;
      protected ParameterIdentification _duplicatedParameterIdentification;
      protected IWithIdRepository _objectBaseRepository;
      protected IndividualSimulation _sim1;
      protected IndividualSimulation _sim2;
      protected OutputMapping _outputMapping;
      protected DataRepository _observedData;
      protected IdentificationParameter _identificationParameter;
      protected ParameterSelection _parameterSelection1;
      protected ParameterSelection _parameterSelection2;
      protected ParameterIdentificationRunResult _runResult;
      protected PKSimProject _project;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _observedData = DomainHelperForSpecs.ObservedData();
         _sim1 = new IndividualSimulation
         {
            Id = "Sim1",
            Name = "Sim1",
            IsLoaded = true,
            Model = new Model {Root = new Container()}
         };
         _sim1.Model.Root.Add(new Container {new Parameter().WithName("P")}.WithName("Liver"));
         _sim2 = new IndividualSimulation
         {
            Id = "Sim2",
            Name = "Sim2",
            IsLoaded = true,
            Model = new Model {Root = new Container()}
         };
         _sim2.Model.Root.Add(new Container {new Parameter().WithName("P")}.WithName("Liver"));

         _objectBaseRepository = IoC.Resolve<IWithIdRepository>();
         var workspace = IoC.Resolve<IWorkspace>();
         _project = IoC.Resolve<PKSimProject>();
         workspace.Project = _project;
         _objectBaseRepository.Register(_sim1);
         _objectBaseRepository.Register(_sim2);
         _project.AddObservedData(_observedData);
         _project.AddBuildingBlock(_sim1);
         _project.AddBuildingBlock(_sim2);

         _parameterIdentification = new ParameterIdentification();
         _parameterIdentification.AddSimulation(_sim1);
         _parameterIdentification.AddSimulation(_sim2);

         _outputMapping = new OutputMapping
         {
            WeightedObservedData = new WeightedObservedData(_observedData),
            OutputSelection = new SimulationQuantitySelection(_sim1, new QuantitySelection("A|B", QuantityType.Metabolite)),
            Weight = 5,
            Scaling = Scalings.Log
         };

         _outputMapping.WeightedObservedData.Weights[1] = 10;
         _parameterIdentification.AddOutputMapping(_outputMapping);

         _identificationParameter = DomainHelperForSpecs.IdentificationParameter(min: 1, max: 10, startValue: 5);

         _parameterSelection1 = new ParameterSelection(_sim1, new QuantitySelection("Liver|P", QuantityType.Parameter));
         _parameterSelection2 = new ParameterSelection(_sim2, new QuantitySelection("Liver|P", QuantityType.Parameter));
         _identificationParameter.AddLinkedParameter(_parameterSelection1);
         _identificationParameter.AddLinkedParameter(_parameterSelection2);
         _parameterIdentification.AddIdentificationParameter(_identificationParameter);
         _identificationParameter.Scaling = Scalings.Linear;

         _parameterIdentification.Configuration.AlgorithmProperties = new OptimizationAlgorithmProperties("AA");
         _parameterIdentification.AlgorithmProperties.Add(new ExtendedProperty<double> {Name = "Toto", Value = 5});

         _runResult = new ParameterIdentificationRunResult();

         _parameterIdentification.AddResult(_runResult);

         _parameterIdentification.AddAnalysis(new ParameterIdentificationPredictedVsObservedChart());
         _parameterIdentification.AddAnalysis(new ParameterIdentificationTimeProfileChart());
         _parameterIdentification.AddAnalysis(new ParameterIdentificationResidualHistogram());
         _parameterIdentification.AddAnalysis(new ParameterIdentificationResidualVsTimeChart());

         GlobalBecause();
      }

      protected abstract void GlobalBecause();

      [Observation]
      public void should_be_able_to_deserialize_the_parameter_identification_and_retrieve_the_simulation_used()
      {
         _duplicatedParameterIdentification.ShouldNotBeNull();
         _duplicatedParameterIdentification.AllSimulations.ShouldOnlyContain(_sim1, _sim2);
      }

      [Observation]
      public void should_be_able_to_deserialize_the_output_mapping_and_update_the_references_used()
      {
         _duplicatedParameterIdentification.AllOutputMappings.Count.ShouldBeEqualTo(1);
         var deserializedOutputMapping = _duplicatedParameterIdentification.AllOutputMappings[0];

         deserializedOutputMapping.OutputSelection.ShouldBeEqualTo(_outputMapping.OutputSelection);
         deserializedOutputMapping.Weight.ShouldBeEqualTo(_outputMapping.Weight);
         deserializedOutputMapping.Scaling.ShouldBeEqualTo(_outputMapping.Scaling);
         deserializedOutputMapping.WeightedObservedData.Weights.ShouldBeEqualTo(_outputMapping.WeightedObservedData.Weights);
         deserializedOutputMapping.WeightedObservedData.ObservedData.ShouldBeEqualTo(_outputMapping.WeightedObservedData.ObservedData);
         deserializedOutputMapping.Simulation.ShouldBeEqualTo(_sim1);
      }

      [Observation]
      public void should_be_able_to_deserialize_the_identifcation_parameters_and_their_linked_parameters()
      {
         _duplicatedParameterIdentification.AllIdentificationParameters.Count.ShouldBeEqualTo(1);
         var deserializedIdentificationParameter = _duplicatedParameterIdentification.AllIdentificationParameters[0];

         deserializedIdentificationParameter.ParameterIdentification.ShouldBeEqualTo(_duplicatedParameterIdentification);

         deserializedIdentificationParameter.MinValueParameter.Value.ShouldBeEqualTo(_identificationParameter.MinValueParameter.Value);
         deserializedIdentificationParameter.MaxValueParameter.Value.ShouldBeEqualTo(_identificationParameter.MaxValueParameter.Value);
         deserializedIdentificationParameter.StartValueParameter.Value.ShouldBeEqualTo(_identificationParameter.StartValueParameter.Value);
         deserializedIdentificationParameter.AllLinkedParameters.Count.ShouldBeEqualTo(2);
         deserializedIdentificationParameter.Scaling.ShouldBeEqualTo(_identificationParameter.Scaling);
         deserializedIdentificationParameter.UseAsFactor.ShouldBeEqualTo(_identificationParameter.UseAsFactor);

         deserializedIdentificationParameter.AllLinkedParameters[0].ShouldBeEqualTo(_parameterSelection1);
         deserializedIdentificationParameter.AllLinkedParameters[1].ShouldBeEqualTo(_parameterSelection2);
      }

      [Observation]
      public void should_be_able_to_deserialize_the_parameter_identification_configuration()
      {
         var deserializedParameterIdentificationConfiguration = _duplicatedParameterIdentification.Configuration;
         deserializedParameterIdentificationConfiguration.RemoveLLOQMode.ShouldBeEqualTo(deserializedParameterIdentificationConfiguration.RemoveLLOQMode);
         deserializedParameterIdentificationConfiguration.LLOQMode.ShouldBeEqualTo(deserializedParameterIdentificationConfiguration.LLOQMode);

         var deserializedAlgorithm = _duplicatedParameterIdentification.AlgorithmProperties;

         deserializedAlgorithm.Name.ShouldBeEqualTo(_parameterIdentification.AlgorithmProperties.Name);
         deserializedAlgorithm.Count.ShouldBeEqualTo(1);
         deserializedAlgorithm.ExistsByName("Toto").ShouldBeTrue();
      }

      public override void Cleanup()
      {
         base.Cleanup();
         _objectBaseRepository.Unregister(_observedData.Id);
         _objectBaseRepository.Unregister(_sim1.Id);
         _objectBaseRepository.Unregister(_sim2.Id);
      }
   }

   public class When_serializing_a_parameter_identification : duplicated_parameter_identification_comparison_test
   {
      public override ParameterIdentification SerializeAndDeserialize(ParameterIdentification source)
      {
         var stream = _serializationManager.Serialize(source);
         var serializationContectFactory = IoC.Resolve<ISerializationContextFactory>();
         using (var context = serializationContectFactory.Create(externalReferences: _project.All<ISimulation>()))
         {
            return _serializationManager.Deserialize<ParameterIdentification>(stream, context);
         }
      }

      protected override void GlobalBecause()
      {
         _duplicatedParameterIdentification = SerializeAndDeserialize(_parameterIdentification);
      }

      [Observation]
      public void should_have_deserialized_the_parameter_identification_results()
      {
         _duplicatedParameterIdentification.Results.Count.ShouldBeEqualTo(1);
      }

      [Observation]
      public void should_have_deserialized_the_parameter_identification_analyses()
      {
         _duplicatedParameterIdentification.Analyses.Count().ShouldBeEqualTo(4);
      }
   }
}