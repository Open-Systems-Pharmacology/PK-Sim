using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Core.Model;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Core.Repositories;
using PKSim.Core.Services;

namespace PKSim.Core
{
   public abstract class concern_for_FieldDimensionConverter : ContextSpecification<FieldDimensionConverter>
   {
      protected IDimensionRepository _dimensionRepository;
      protected IPopulationDataCollector _populationDataCollector;
      protected IQuantityField _quantityField;

      protected override void Context()
      {
         _quantityField = A.Fake<IQuantityField>();
         _dimensionRepository = A.Fake<IDimensionRepository>();
         _populationDataCollector = A.Fake<IPopulationDataCollector>();
         _quantityField.QuantityPath = "Quantity";
         sut = new MolarToMassAmountDimensionForFieldConverter(_quantityField, _populationDataCollector, _dimensionRepository);
      }
   }

   public class When_checking_if_a_field_dimension_converter_can_resolve_its_parameters : concern_for_FieldDimensionConverter
   {
      [Observation]
      public void should_return_false_if_the_underlying_field_is_a_quantity_field_for_which_the_mol_weight_cannot_be_resolved()
      {
         A.CallTo(() => _populationDataCollector.MolWeightFor(_quantityField.QuantityPath)).Returns(null);
         sut.CanResolveParameters().ShouldBeFalse();
      }

      [Observation]
      public void should_return_true_if_the_underlying_field_is_a_quantity_field_for_which_the_mol_weight_could_be_resolved()
      {
         A.CallTo(() => _populationDataCollector.MolWeightFor(_quantityField.QuantityPath)).Returns(250);
         sut.CanResolveParameters().ShouldBeTrue();
      }
   }
}