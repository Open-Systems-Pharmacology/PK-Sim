using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using OSPSuite.Core.Domain;
using PKSim.Core.Repositories;

namespace PKSim.Core
{
   public abstract class concern_for_MoleculeToQueryExpressionSettingsMapper : ContextSpecification<IMoleculeToQueryExpressionSettingsMapper>
   {
      protected IndividualProtein _protein;
      protected QueryExpressionSettings _result;
      protected IRepresentationInfoRepository _representationInfoRepository;
      private ISimulationSubject _individual;

      protected override void Context()
      {
         _individual= A.Fake<ISimulationSubject>();
         _representationInfoRepository = A.Fake<IRepresentationInfoRepository>(); 
         sut = new MoleculeToQueryExpressionSettingsMapper(_representationInfoRepository);
      }

      protected override void Because()
      {
         _result = sut.MapFrom(_protein, _individual);
      }
   }

   public class When_mapping_a_protein_expression_to_a_query_expression_setting : concern_for_MoleculeToQueryExpressionSettingsMapper
   {
      private double _proteinContent;
      private MoleculeExpressionContainer _exp1;
      private MoleculeExpressionContainer _exp2;

      protected override void Context()
      {
         base.Context();
         _protein = A.Fake<IndividualProtein>();
         _protein.QueryConfiguration = "toto";
         _exp1 = new MoleculeExpressionContainer().WithName("exp1");
         _exp2 = new MoleculeExpressionContainer().WithName("exp2");
         A.CallTo(() => _protein.AllExpressionsContainers()).Returns(new[] {_exp1, _exp2});
         _proteinContent = 10;
         A.CallTo(() => _protein.ReferenceConcentration).Returns(DomainHelperForSpecs.ConstantParameterWithValue(_proteinContent));
         A.CallTo(() => _representationInfoRepository.DisplayNameFor(_exp1)).Returns("disp1");
         A.CallTo(() => _representationInfoRepository.DisplayNameFor(_exp2)).Returns("disp2");
      }

      [Observation]
      public void should_return_a_query_settings_initialized_with_one_container()
      {
         _result.ExpressionContainers.Count().ShouldBeEqualTo(2);
         _result.ExpressionContainers.ElementAt(0).ContainerDisplayName.ShouldBeEqualTo("disp1");
         _result.ExpressionContainers.ElementAt(1).ContainerDisplayName.ShouldBeEqualTo("disp2");
      }

      [Observation]
      public void should_have_set_the_query_configuration_to_the_predefined_configuration()
      {
         _result.QueryConfiguration.ShouldBeEqualTo(_protein.QueryConfiguration);
      }
   }
}