using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Model;

namespace PKSim.Core
{
   public abstract class concern_for_IndividualProtein : ContextSpecification<IndividualProtein>
   {
      protected override void Context()
      {
         sut = new IndividualEnzyme();
      }
   }

   public class When_setting_the_localization_of_a_protein : concern_for_IndividualProtein
   {
      [Observation]
      public void should_return_the_expected_localization()
      {
         sut.IsInterstitial = true;
         sut.IsInterstitial.ShouldBeTrue();

         sut.IsBloodCellsIntracellular = true;
         sut.InBloodCells.ShouldBeTrue();

         sut.IsBloodCellsMembrane = true;
         sut.InBloodCells.ShouldBeTrue();

         sut.IsBloodCellsIntracellular = false;
         sut.IsBloodCellsMembrane = false;
         sut.InBloodCells.ShouldBeFalse();
         sut.IsBloodCellsMembrane.ShouldBeFalse();
         sut.IsBloodCellsIntracellular.ShouldBeFalse();


         sut.IsVascEndosome = true;
         sut.IsVascEndosome.ShouldBeTrue();
         sut.InVascularEndothelium.ShouldBeTrue();
         sut.IsVascEndosome = false;
         sut.IsVascEndosome.ShouldBeFalse();
         sut.InVascularEndothelium.ShouldBeFalse();


         sut.IsVascMembraneBasolateral = true;
         sut.IsVascMembraneBasolateral.ShouldBeTrue();
         sut.InVascularEndothelium.ShouldBeTrue();
         sut.IsVascMembraneBasolateral = false;
         sut.IsVascMembraneBasolateral.ShouldBeFalse();
         sut.InVascularEndothelium.ShouldBeFalse();

         sut.IsVascMembraneApical = true;
         sut.IsVascMembraneApical.ShouldBeTrue();
         sut.InVascularEndothelium.ShouldBeTrue();
         sut.IsVascMembraneApical = false;
         sut.IsVascMembraneApical.ShouldBeFalse();
         sut.InVascularEndothelium.ShouldBeFalse();
      }
   }
}