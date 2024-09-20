using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using FakeItEasy;

namespace PKSim.Core
{
   public abstract class concern_for_ModelPropertiesTask : ContextSpecification<IModelPropertiesTask>
   {
      protected IModelConfigurationRepository _modelConfigurationRepository;
      protected OriginData _originData;

      protected override void Context()
      {
         _modelConfigurationRepository = A.Fake<IModelConfigurationRepository>();
         _originData = new OriginData {Species = A.Fake<Species>().WithName(CoreConstants.Species.HUMAN)};
         sut = new ModelPropertiesTask(_modelConfigurationRepository);
      }
   }

   public class When_resolving_the_default_model_properties_for_a_given_origin_data : concern_for_ModelPropertiesTask
   {
      private CalculationMethod _cm1;
      private CalculationMethod _cm2;
      private ModelProperties _result;
      private ModelConfiguration _modelConfiguration;

      protected override void Context()
      {
         base.Context();
         _modelConfiguration = A.Fake<ModelConfiguration>();
         var cmc1 = A.Fake<CalculationMethodCategory>();
         var cmc2 = A.Fake<CalculationMethodCategory>();
         _cm1 = new CalculationMethod().WithName("toto");
         _cm2 = new CalculationMethod().WithName("tata");
         A.CallTo(() => cmc1.DefaultItemForSpecies(_originData.Species)).Returns(_cm1);
         A.CallTo(() => cmc2.DefaultItemForSpecies(_originData.Species)).Returns(_cm2);
         A.CallTo(() => _modelConfigurationRepository.DefaultFor(_originData.Species)).Returns(_modelConfiguration);
         A.CallTo(() => _modelConfiguration.CalculationMethodCategories).Returns(new[] {cmc1, cmc2});
      }

      protected override void Because()
      {
         _result = sut.DefaultFor(_originData);
      }

      [Observation]
      public void should_return_a_model_properties_containing_the_default_calculation_method_for_the_species()
      {
         _result.AllCalculationMethods().ShouldOnlyContainInOrder(_cm1, _cm2);
      }
   }
}