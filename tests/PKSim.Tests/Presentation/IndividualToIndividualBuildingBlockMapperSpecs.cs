using System.Linq;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Core.Extensions;
using PKSim.Core;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Infrastructure;
using ILazyLoadTask = OSPSuite.Core.Domain.Services.ILazyLoadTask;

namespace PKSim.Presentation
{
   public abstract class concern_for_IndividualToIndividualBuildingBlockMapper : ContextSpecification<IndividualToIndividualBuildingBlockMapper>
   {
      protected Individual _individual;
      private IObjectBaseFactory _objectBaseFactory;
      private IApplicationConfiguration _applicationConfiguration;
      private ILazyLoadTask _lazyLoadTask;
      private EntityPathResolverForSpecs _objectPathFactory;
      private ICalculationMethodCategoryRepository _calculationMethodCategoryRepository;
      private static CalculationMethodCategory _category1;
      private ICloner _cloner;

      protected override void Context()
      {
         _individual = DomainHelperForSpecs.CreateIndividual("Human");
         _lazyLoadTask = A.Fake<ILazyLoadTask>();
         _applicationConfiguration = new PKSimConfiguration();
         _objectPathFactory = new EntityPathResolverForSpecs();
         _objectBaseFactory = A.Fake<IObjectBaseFactory>();
         _cloner = A.Fake<ICloner>();

         //cloning formula always returns the same formula for testing
         A.CallTo(() => _cloner.Clone(A<IFormula>._)).ReturnsLazily(x => x.GetArgument<IFormula>(0));

         var representationInfoRepository = A.Fake<IRepresentationInfoRepository>();
         A.CallTo(representationInfoRepository).WithReturnType<RepresentationInfo>().Returns(new RepresentationInfo {DisplayName = "displayName"});

         _calculationMethodCategoryRepository = A.Fake<ICalculationMethodCategoryRepository>();
         updateIndividualForTest(_individual);

         A.CallTo(() => _objectBaseFactory.Create<IndividualBuildingBlock>()).Returns(new IndividualBuildingBlock());
         A.CallTo(() => _objectBaseFactory.Create<IndividualParameter>()).Returns(new IndividualParameter());
         sut = new IndividualToIndividualBuildingBlockMapper(_objectBaseFactory, _objectPathFactory, _applicationConfiguration, _lazyLoadTask, representationInfoRepository, _calculationMethodCategoryRepository, _cloner);
      }

      private void updateIndividualForTest(Individual individual)
      {
         //add a parameter with a formula starting with ROOT so that we can test that this is being dealt with properly
         var parameterWithFormula = DomainHelperForSpecs.ConstantParameterWithValue(0).WithName("FormulaParameter");
         parameterWithFormula.Formula = new ExplicitFormula("P2").WithName("FormulaParameterFormula");
         parameterWithFormula.Formula.AddObjectPath(new FormulaUsablePath("ROOT", "ORGANISM", "P2").WithAlias("P2"));

         //for this formula, we return a real one to make sure we have actually changed the ROOT
         var cloneFormula = new ExplicitFormula("P2").WithName("FormulaParameterFormula");
         cloneFormula.AddObjectPath(new FormulaUsablePath("ROOT", "ORGANISM", "P2").WithAlias("P2"));

         A.CallTo(() => _cloner.Clone(parameterWithFormula.Formula)).Returns(cloneFormula);
         individual.Organism.Add(parameterWithFormula);

         var originData = individual.OriginData;
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

      [Observation]
      public void should_have_replaced_the_ROOT_key_element_path_in_all_formula()
      {
         var formula = _buildingBlock.FormulaCache.FindByName("FormulaParameterFormula");
         formula.ObjectPaths.Count.ShouldBeEqualTo(1);
         formula.ObjectPaths[0].ToPathString().ShouldBeEqualTo("ORGANISM|P2");
      }
   }
}