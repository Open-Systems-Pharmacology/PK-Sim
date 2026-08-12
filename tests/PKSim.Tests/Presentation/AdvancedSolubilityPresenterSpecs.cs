using FakeItEasy;
using OSPSuite.BDDHelper;
using PKSim.Core.Model;
using PKSim.Presentation.Presenters.Compounds;
using PKSim.Presentation.Views;

namespace PKSim.Presentation
{
   public abstract class concern_for_AdvancedSolubilityPresenter : ContextSpecification<IAdvancedSolubilityPresenter>
   {
      protected IAdvancedSolubilityConstantsGroupPresenter _constantsGroupPresenter;
      protected IBileSaltPartitionCoefficientGroupPresenter _bileSaltPartitionCoefficientGroupPresenter;

      protected override void Context()
      {
         _constantsGroupPresenter = A.Fake<IAdvancedSolubilityConstantsGroupPresenter>();
         _bileSaltPartitionCoefficientGroupPresenter = A.Fake<IBileSaltPartitionCoefficientGroupPresenter>();
         sut = new AdvancedSolubilityPresenter(A.Fake<IMultiplePanelView>(), _constantsGroupPresenter, _bileSaltPartitionCoefficientGroupPresenter);
      }
   }

   public class When_editing_the_advanced_solubility_parameters_of_a_compound : concern_for_AdvancedSolubilityPresenter
   {
      private Compound _compound;

      protected override void Context()
      {
         base.Context();
         _compound = new Compound();
      }

      protected override void Because()
      {
         sut.EditCompound(_compound);
      }

      [Observation]
      public void should_edit_the_advanced_solubility_constants()
      {
         A.CallTo(() => _constantsGroupPresenter.EditCompound(_compound)).MustHaveHappened();
      }

      [Observation]
      public void should_edit_the_alternatives_of_the_neutral_bile_salt_partition_coefficient()
      {
         A.CallTo(() => _bileSaltPartitionCoefficientGroupPresenter.EditCompound(_compound)).MustHaveHappened();
      }
   }
}
