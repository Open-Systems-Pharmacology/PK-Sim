using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Core.Domain.Services;
using PKSim.Presentation;
using PKSim.Presentation.Services;

namespace PKSim.Core
{
   public abstract class concern_for_ParameterAnalysableParameterSelector : ContextSpecification<IParameterAnalysableParameterSelector>
   {
      protected IUserSettings _userSettings;

      protected override void Context()
      {
         _userSettings = A.Fake<IUserSettings>();
         _userSettings.DefaultParameterGroupingMode = ParameterGroupingModeId.Advanced;
         sut = new ParameterAnalysableParameterSelector(_userSettings);
      }
   }

   public class When_checking_if_a_parameter_can_be_optimized : concern_for_ParameterAnalysableParameterSelector
   {
      private IParameter _parameter;

      protected override void Context()
      {
         base.Context();
         _parameter = new Parameter
         {
            CanBeVaried = true,
            Info = {ReadOnly = false},
            Visible = true
         };
      }

      [Observation]
      public void should_return_true_if_a_parameter_can_be_varied_and_is_not_readonly_and_is_visible()
      {
         sut.CanUseParameter(_parameter).ShouldBeTrue();
      }

      [Observation]
      public void should_return_false_if_a_parameter_is_a_table_parameter()
      {
         _parameter.Formula = new TableFormula();
         sut.CanUseParameter(_parameter).ShouldBeFalse();
      }

      [Observation]
      public void should_return_false_if_the_parameter_is_readonly()
      {
         _parameter.Info.ReadOnly = true;
         sut.CanUseParameter(_parameter).ShouldBeFalse();
      }

      [Observation]
      public void should_return_false_if_the_parameter_cannot_be_varied()
      {
         _parameter.CanBeVaried = false;
         sut.CanUseParameter(_parameter).ShouldBeFalse();
      }

      [Observation]
      public void should_return_false_if_the_parameter_is_hidden()
      {
         _parameter.Visible = false;
         sut.CanUseParameter(_parameter).ShouldBeFalse();
      }
   }

   public class When_retrieving_the_default_parameter_selection_mode : concern_for_ParameterAnalysableParameterSelector
   {
      [Observation]
      public void should_return_the_grouping_mode_from_the_user_settings_for_simple_or_advanced()
      {
         _userSettings.DefaultParameterGroupingMode = ParameterGroupingModeId.Advanced;
         sut.DefaultParameterSelectionMode.ShouldBeEqualTo(ParameterGroupingModes.Advanced);

         _userSettings.DefaultParameterGroupingMode = ParameterGroupingModeId.Simple;
         sut.DefaultParameterSelectionMode.ShouldBeEqualTo(ParameterGroupingModes.Simple);
      }

      [Observation]
      public void should_return_simple_if_the_default_selection_is_hierarchical()
      {
         _userSettings.DefaultParameterGroupingMode = ParameterGroupingModeId.Hierarchical;
         sut.DefaultParameterSelectionMode.ShouldBeEqualTo(ParameterGroupingModes.Simple);
      }
   }
}