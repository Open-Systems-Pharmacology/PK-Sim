using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using OSPSuite.Core.Domain.Data;
using PKSim.Core.Repositories;
using PKSim.Core.Services;

namespace PKSim.Core
{
   public abstract class concern_for_MolarToMassAmoutDimensionConverter : ContextSpecification<AmountToMassDimensionConverter>
   {
      private DataColumn _dataColumn;
      protected double _molWeight = 400;
      protected IDimensionRepository _dimensionRepository;

      protected override void Context()
      {
         _dataColumn = new DataColumn();
         _dataColumn.DataInfo.MolWeight = _molWeight;
         _dimensionRepository = A.Fake<IDimensionRepository>();
         sut = new AmountToMassDimensionConverter(_dataColumn, _dimensionRepository);
      }
   }

   public class The_molar_to_mass_amount_converter : concern_for_MolarToMassAmoutDimensionConverter
   {
      [Observation]
      public void should_be_able_to_convert_to_mass()
      {
         sut.CanConvertTo(_dimensionRepository.Mass).ShouldBeTrue();
      }

      [Observation]
      public void should_be_able_to_convert_from_molar_amount()
      {
         sut.CanConvertFrom(_dimensionRepository.Amount).ShouldBeTrue();
      }
   }

   public class When_converting_some_molar_amount_to_mass_amount : concern_for_MolarToMassAmoutDimensionConverter
   {
      [Observation]
      public void should_return_the_expected_value()
      {
         //mol to mass
         sut.ConvertToTargetBaseUnit(1000).ShouldBeEqualTo(_molWeight * 1000);
      }
   }

   public class When_converting_some_mass_amount_to_molar_amount : concern_for_MolarToMassAmoutDimensionConverter
   {
      [Observation]
      public void should_return_the_expected_value()
      {
         //mass to mol
         sut.ConvertToSourceBaseUnit(1000).ShouldBeEqualTo(1000 / _molWeight);
      }
   }
}