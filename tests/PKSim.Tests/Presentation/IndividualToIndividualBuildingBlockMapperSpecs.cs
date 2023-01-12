using System.Linq;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Services;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Infrastructure;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Mappers;

namespace PKSim.Presentation
{
   public class concern_for_IndividualToIndividualBuildingBlockMapper : ContextSpecification<IndividualToIndividualBuildingBlockMapper>
   {
      protected Individual _individual;
      private IObjectBaseFactory _objectBaseFactory;
      private IApplicationConfiguration _applicationConfiguration;
      private ILazyLoadTask _lazyLoadTask;
      private ICalculationMethodToCategoryCalculationMethodDTOMapper _calculationMethodDTOMapper;
      private EntityPathResolverForSpecs _objectPathFactory;
      private ICalculationMethodCategoryRepository _calculationMethodCategoryRepository;
      private static CalculationMethodCategory _category1;

      protected override void Context()
      {
         _individual = DomainHelperForSpecs.CreateIndividual("Human");
         _lazyLoadTask = A.Fake<ILazyLoadTask>();
         _applicationConfiguration = new PKSimConfiguration();
         _objectPathFactory = new EntityPathResolverForSpecs();
         _objectBaseFactory = A.Fake<IObjectBaseFactory>();
         var representationInfoRepository = A.Fake<IRepresentationInfoRepository>();
         _calculationMethodDTOMapper = new CalculationMethodToCategoryCalculationMethodDTOMapper(representationInfoRepository);
         A.CallTo(() => representationInfoRepository.InfoFor(A<RepresentationObjectType>._, A<string>._)).Returns(new RepresentationInfo { DisplayName = "displayName" });

         _calculationMethodCategoryRepository = A.Fake<ICalculationMethodCategoryRepository>();
         updateOriginDataForTest(_individual.OriginData);

         A.CallTo(() => _objectBaseFactory.Create<IndividualBuildingBlock>()).Returns(new IndividualBuildingBlock());
         A.CallTo(() => _objectBaseFactory.Create<IndividualParameter>()).Returns(new IndividualParameter());
         sut = new IndividualToIndividualBuildingBlockMapper(_objectBaseFactory, _objectPathFactory, _applicationConfiguration, _lazyLoadTask, _calculationMethodDTOMapper, _calculationMethodCategoryRepository);
      }

      private void updateOriginDataForTest(OriginData originData)
      {
         originData.Gender.DisplayName = originData.Gender.Name;
         originData.Population.DisplayName = originData.Population.Name;
         originData.Age = new OriginDataParameter(1, "year", "Age");
         originData.GestationalAge = new OriginDataParameter(52, "weeks", "Gestational Age");

         _category1 = new CalculationMethodCategory();
         

         var calculationMethod = new CalculationMethod
         {
            DisplayName = "method1",
            Category = "category1",
         };
         calculationMethod.AddSpecies(_individual.Species.Name);

         originData.CalculationMethodCache.AddCalculationMethod(calculationMethod);
         _category1.Add(calculationMethod);

         A.CallTo(() => _calculationMethodCategoryRepository.HasMoreThanOneOption(A<CalculationMethod>._, _individual.Species)).WhenArgumentsMatch(x => x.Get<CalculationMethod>(0).Category.Equals("category2")).Returns(false);
         A.CallTo(() => _calculationMethodCategoryRepository.HasMoreThanOneOption(A<CalculationMethod>._, _individual.Species)).WhenArgumentsMatch(x => x.Get<CalculationMethod>(0).Category.Equals("category1")).Returns(true);

         calculationMethod = new CalculationMethod
         {
            DisplayName = "method2",
            Category = "category1",
         };
         calculationMethod.AddSpecies(_individual.Species.Name);
         
         _category1.Add(calculationMethod);

         calculationMethod = new CalculationMethod
         {
            Category = "category2",
            Name = "calculationMethod1"
            
         };
         originData.CalculationMethodCache.AddCalculationMethod(calculationMethod);
         originData.DiseaseState = new DiseaseState
         {
            DisplayName = "A Disease"
         };
      }
   }

   public class When_mapping_building_block_from_individual : concern_for_IndividualToIndividualBuildingBlockMapper
   {
      private IndividualBuildingBlock _buildingBlock;

      protected override void Because()
      {
         _buildingBlock = sut.MapFrom(_individual);
      }

      [Observation]
      public void the_properties_of_the_building_block_should_match()
      {
         _buildingBlock.OriginData.AllDataItems.Count(x => x.Name.Equals("Species")).ShouldBeEqualTo(1);
         _buildingBlock.OriginData.AllDataItems.Count(x => x.Name.Equals("Population")).ShouldBeEqualTo(1);
         _buildingBlock.OriginData.AllDataItems.Count(x => x.Name.Equals("Gender")).ShouldBeEqualTo(1);
         _buildingBlock.OriginData.AllDataItems.Count(x => x.Name.Equals("Weight")).ShouldBeEqualTo(1);
         _buildingBlock.OriginData.AllDataItems.Count(x => x.Name.Equals("Age")).ShouldBeEqualTo(1);
         _buildingBlock.OriginData.AllDataItems.Count(x => x.Name.Equals("Gestational Age")).ShouldBeEqualTo(1);
         _buildingBlock.OriginData.AllDataItems.Count(x => x.Name.Equals("Disease State")).ShouldBeEqualTo(1);

         // the count is 0 here because we intentionally left these out to make sure only populated origin data would be mapped
         _buildingBlock.OriginData.AllDataItems.Count(x => x.Name.Equals("Height")).ShouldBeEqualTo(0);
         _buildingBlock.OriginData.AllDataItems.Count(x => x.Name.Equals("BMI")).ShouldBeEqualTo(0);

         _buildingBlock.OriginData.AllDataItems.Count(x => x.Name.Equals("displayName")).ShouldBeEqualTo(1);
         _buildingBlock.OriginData.AllDataItems.Count.ShouldBeEqualTo(8);
      }
   }
}