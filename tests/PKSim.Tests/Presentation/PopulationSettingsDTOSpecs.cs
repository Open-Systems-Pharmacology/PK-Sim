using NUnit.Framework;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Validation;
using PKSim.Presentation.DTO.Populations;

namespace PKSim.Presentation
{
   public class concern_for_PopulationSettingsDTO : ContextSpecification<PopulationSettingsDTO>
   {
      protected override void Context()
      {
         sut = new PopulationSettingsDTO();
         sut.NumberOfIndividuals = 5;
         sut.ProportionOfFemales = 50;
      }

      [TestCase(50, true)]
      [TestCase(101, false)]
      [TestCase(-1, false)]
      public void FemaleRate_should_be_properly_bounded(int femaleRate, bool expectation)
      {
         sut.ProportionOfFemales = femaleRate;
         sut.IsValid().ShouldBeEqualTo(expectation);
      }
   }
}
