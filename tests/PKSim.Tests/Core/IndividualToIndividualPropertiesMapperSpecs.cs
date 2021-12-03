using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Populations;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core
{
   public abstract class concern_for_IndividualToIndividualPropertiesMapper : ContextSpecification<IIndividualToIndividualValuesMapper>
   {
      protected IEntityPathResolver _entityPathResolver;
      private IContainerTask _containerTask;

      protected override void Context()
      {
         _entityPathResolver = A.Fake<IEntityPathResolver>();
         _containerTask = new ContainerTask(A.Fake<IObjectBaseFactory>(), _entityPathResolver);
         sut = new IndividualToIndividualValuesMapper(_containerTask);
      }
   }

   public class When_mapping_individual_properties_from_a_given_individual : concern_for_IndividualToIndividualPropertiesMapper
   {
      private Individual _individual;
      private IndividualValues _individualValues;
      private IParameter _para1;
      private IParameter _para2;
      private string _pathPara1;
      private string _pathPara2;
      private IParameter _para3;

      protected override void Context()
      {
         base.Context();
         _para1 = A.Fake<IParameter>().WithName("P1");
         A.CallTo(() => _para1.IsChangedByCreateIndividual).Returns(true);
         _para2 = A.Fake<IParameter>().WithName("P2");
         A.CallTo(() => _para2.IsChangedByCreateIndividual).Returns(true);
         _para3 = A.Fake<IParameter>().WithName("P3");
         A.CallTo(() => _para3.IsChangedByCreateIndividual).Returns(false);
         _pathPara1 = "tata";
         _pathPara2 = "tutu";
         _individual = new Individual();
         _individual.OriginData = new OriginData
         {
            Gender = new Gender().WithName("TOTO"),
            Population = new SpeciesPopulation(),
         };
         A.CallTo(() => _entityPathResolver.PathFor(_para1)).Returns(_pathPara1);
         A.CallTo(() => _entityPathResolver.PathFor(_para2)).Returns(_pathPara2);
         A.CallTo(() => _entityPathResolver.PathFor(_para2)).Returns(_pathPara2);
         _individual.Add(_para1);
         _individual.Add(_para2);
         _individual.Add(_para3);
      }

      protected override void Because()
      {
         _individualValues = sut.MapFrom(_individual);
      }

      [Observation]
      public void should_have_updated_the_original_origin_data()
      {
         _individualValues.Covariates[Constants.Population.GENDER].ShouldBeEqualTo(_individual.OriginData.Gender.Name);
      }

      [Observation]
      public void should_have_created_one_parameter_value_info_for_each_parameter_potentially_changed_by_the_create_individual_algorithm()
      {
         _individualValues.ParameterValues.Count().ShouldBeEqualTo(2);
         _individualValues.ParameterValue(_pathPara1).ShouldNotBeNull();
         _individualValues.ParameterValue(_pathPara2).ShouldNotBeNull();
      }
   }
}