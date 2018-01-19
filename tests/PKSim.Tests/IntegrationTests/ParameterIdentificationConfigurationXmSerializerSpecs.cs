using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Container;
using OSPSuite.Utility.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.ParameterIdentifications;
using OSPSuite.Core.Domain.Repositories;

namespace PKSim.IntegrationTests
{
   public abstract class When_serializing_a_parameter_identification_configuration : ContextForSerialization<ParameterIdentificationConfiguration>
   {
      private ParameterIdentificationConfiguration _parameterIdentificationConfiguration;
      protected ParameterIdentificationConfiguration _deserializedParameterIdentificationConfiguration;
   
      public override void GlobalContext()
      {
         base.GlobalContext();

         _parameterIdentificationConfiguration = new ParameterIdentificationConfiguration
         {
            AlgorithmProperties = new OptimizationAlgorithmProperties("Test")
            {
               new ExtendedProperty<string> {Name = "Prop1", Value = "Test"},
               new ExtendedProperty<bool> {Name = "Prop2", Value = true}
            },
            RunMode = CreateRunMode()
         };
         _deserializedParameterIdentificationConfiguration = SerializeAndDeserialize(_parameterIdentificationConfiguration);
      }

      protected abstract ParameterIdentificationRunMode CreateRunMode();

      [Observation]
      public void should_be_able_to_deserialize_the_parameter_identification_configuration()
      {
         _deserializedParameterIdentificationConfiguration.ShouldNotBeNull();
      }
   }

   public class When_serializing_a_parameter_identificaiton_configuration_with_categorial_run_mode : When_serializing_a_parameter_identification_configuration
   {
      private CategorialParameterIdentificationRunMode _categorialRunMode;
      private CalculationMethodCache _calculationMethodCache1;
      private CalculationMethodCache _calculationMethodCache2;
      private CalculationMethod _calculationMethod1;
      private CalculationMethod _calculationMethod2;

     
      protected override ParameterIdentificationRunMode CreateRunMode()
      {
         var calculationMethodRepository = IoC.Resolve<ICompoundCalculationMethodRepository>();
         _categorialRunMode = new CategorialParameterIdentificationRunMode {AllTheSame = false};
         _calculationMethod1 = calculationMethodRepository.All().ElementAt(0);
         _calculationMethod2 = calculationMethodRepository.All().ElementAt(1);
         _calculationMethodCache1 = new CalculationMethodCache();
         _calculationMethodCache2 = new CalculationMethodCache();
         _calculationMethodCache1.AddCalculationMethod(_calculationMethod1);
         _calculationMethodCache1.AddCalculationMethod(_calculationMethod2);
         _calculationMethodCache2.AddCalculationMethod(_calculationMethod2);
         _categorialRunMode.CalculationMethodsCache.Add("Drug1", _calculationMethodCache1);
         _categorialRunMode.CalculationMethodsCache.Add("Drug2", _calculationMethodCache2);

         return _categorialRunMode;
      }

      [Observation]
      public void should_deserialize_the_parameter_identification_run_mode()
      {
         _deserializedParameterIdentificationConfiguration.RunMode.ShouldBeAnInstanceOf<CategorialParameterIdentificationRunMode>();
         var deserializedCategorialRunMode = _deserializedParameterIdentificationConfiguration.RunMode.DowncastTo<CategorialParameterIdentificationRunMode>();
         deserializedCategorialRunMode.AllTheSame.ShouldBeFalse();
         deserializedCategorialRunMode.CalculationMethodsCache.Count.ShouldBeEqualTo(2);
         deserializedCategorialRunMode.CalculationMethodsCache["Drug1"].ShouldOnlyContain(_calculationMethod1, _calculationMethod2);
         deserializedCategorialRunMode.CalculationMethodsCache["Drug2"].ShouldOnlyContain(_calculationMethod2);
      }
   }
}