﻿using System.Linq;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Services;
using PKSim.Core;
using PKSim.Core.Model;
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

      protected override void Context()
      {
         _individual = DomainHelperForSpecs.CreateIndividual("Human");
         _lazyLoadTask = A.Fake<ILazyLoadTask>();
         _applicationConfiguration = new PKSimConfiguration();
         _objectPathFactory = new EntityPathResolverForSpecs();
         _objectBaseFactory = A.Fake<IObjectBaseFactory>();
         _calculationMethodDTOMapper = A.Fake<ICalculationMethodToCategoryCalculationMethodDTOMapper>();

         updateOriginDataForTest(_individual.OriginData);

         A.CallTo(() => _objectBaseFactory.Create<IndividualBuildingBlock>()).Returns(new IndividualBuildingBlock());
         A.CallTo(() => _objectBaseFactory.Create<IndividualParameter>()).Returns(new IndividualParameter());
         sut = new IndividualToIndividualBuildingBlockMapper(_objectBaseFactory, _objectPathFactory, _applicationConfiguration, _lazyLoadTask, _calculationMethodDTOMapper);
      }

      private static void updateOriginDataForTest(OriginData originData)
      {
         originData.Gender.DisplayName = originData.Gender.Name;
         originData.Population.DisplayName = originData.Population.Name;
         originData.Age = new OriginDataParameter(1, "year", "Age");
         originData.GestationalAge = new OriginDataParameter(52, "weeks", "Gestational Age");
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
      }
   }
}