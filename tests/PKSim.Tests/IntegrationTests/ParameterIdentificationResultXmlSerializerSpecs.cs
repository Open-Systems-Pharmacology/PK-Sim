using System;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Container;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Presentation.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.ParameterIdentifications;

namespace PKSim.IntegrationTests
{
   public class When_serializing_a_parameter_identification_result : ContextForSerialization<ParameterIdentificationRunResult>
   {
      private OptimizedParameterValue _optimizedParameterValue1;
      private OptimizationRunResult _bestRunResult;
      private ParameterIdentificationRunResult _parameterIdentificationRunResult;
      private ParameterIdentificationRunResult _deserializedParameterIdentificationRunResult;
      private OptimizedParameterValue _optimizedParameterValue2;
      private ResidualsResult _residualResults;
      private DataRepository _observedData1;
      private DataRepository _observedData2;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _optimizedParameterValue1 = new OptimizedParameterValue("P1", 10,11);
         _optimizedParameterValue2 = new OptimizedParameterValue("P2", 20, 21);

         _parameterIdentificationRunResult = new ParameterIdentificationRunResult();

         _residualResults = new ResidualsResult();
         _bestRunResult = new OptimizationRunResult {ResidualsResult = _residualResults};
         _parameterIdentificationRunResult.BestResult = _bestRunResult;
         _parameterIdentificationRunResult.Duration = new TimeSpan(1, 2, 3, 4);
         _observedData1 = DomainHelperForSpecs.ObservedData("OBS1");
         _observedData2 = DomainHelperForSpecs.ObservedData("OBS2");
         _residualResults.AddOutputResiduals("OutputPath1", _observedData1, new[] {new Residual(1f, 2f,1), new Residual(2f, 3f,2) });
         _residualResults.AddOutputResiduals("OutputPath2", _observedData2, new[] {new Residual(3f, 4f,4)});
         _bestRunResult.AddValue(_optimizedParameterValue1);
         _bestRunResult.AddValue(_optimizedParameterValue2);

         _bestRunResult.AddResult(DomainHelperForSpecs.IndividualSimulationDataRepositoryFor("S1"));
         _bestRunResult.AddResult(DomainHelperForSpecs.IndividualSimulationDataRepositoryFor("S2"));

         var workspace = IoC.Resolve<IWorkspace>();
         var project = IoC.Resolve<PKSimProject>();
         workspace.Project = project;
         project.AddObservedData(_observedData1);
         project.AddObservedData(_observedData2);
         


         _deserializedParameterIdentificationRunResult = SerializeAndDeserialize(_parameterIdentificationRunResult);
      }

      [Observation]
      public void should_be_able_to_deserialize_the_given_parameter_identificaiton_result()
      {
         _deserializedParameterIdentificationRunResult.ShouldNotBeNull();
      }

      [Observation]
      public void should_have_the_expected_parameter_values()
      {
         _deserializedParameterIdentificationRunResult.BestResult.Values.Count.ShouldBeEqualTo(2);
         _deserializedParameterIdentificationRunResult.BestResult.Values[0].Name.ShouldBeEqualTo(_optimizedParameterValue1.Name);
         _deserializedParameterIdentificationRunResult.BestResult.Values[1].Name.ShouldBeEqualTo(_optimizedParameterValue2.Name);
         _deserializedParameterIdentificationRunResult.Duration.Days.ShouldBeEqualTo(1);
         _deserializedParameterIdentificationRunResult.Duration.Hours.ShouldBeEqualTo(2);
         _deserializedParameterIdentificationRunResult.Duration.Minutes.ShouldBeEqualTo(3);
         _deserializedParameterIdentificationRunResult.Duration.Seconds.ShouldBeEqualTo(4);
      }

      [Observation]
      public void should_have_deserialized_the_best_results()
      {
         var deserializedRunResult = _deserializedParameterIdentificationRunResult.BestResult;
         deserializedRunResult.ShouldBeAnInstanceOf<OptimizationRunResult>();
         deserializedRunResult.TotalError.ShouldBeEqualTo(_bestRunResult.TotalError);
         deserializedRunResult.AllOutputResiduals.Count.ShouldBeEqualTo(2);
         deserializedRunResult.AllOutputResiduals.ElementAt(0).ObservedData.ShouldBeEqualTo(_observedData1);
         deserializedRunResult.AllOutputResiduals.ElementAt(1).ObservedData.ShouldBeEqualTo(_observedData2);
         deserializedRunResult.SimulationResults.Count.ShouldBeEqualTo(2);
      }
   }
}