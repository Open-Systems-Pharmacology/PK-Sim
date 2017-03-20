using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Reporting;
using OSPSuite.Core.Services;
using OSPSuite.Infrastructure.Reporting;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Services;
using PKSim.Infrastructure.Reporting.TeX.Builders;

namespace PKSim.Infrastructure
{
   public abstract class concern_for_PresentableTEXBuilder : ContextSpecification<PresentableTeXBuilder<Container, DefaultPresentationSettings>>
   {
      private IDisplayUnitRetriever _displayUnitRetriever;
      private IPresentationSettingsTask _presentationSettingsTask;
      protected IParameter _parameter1;
      protected IParameter _parameter2;
      protected IParameter _parameter3;
      private Container _container;
      private DefaultPresentationSettings _presentationSettings;

      protected override void Context()
      {
         _presentationSettingsTask = A.Fake<IPresentationSettingsTask>();
         _displayUnitRetriever = A.Fake<IDisplayUnitRetriever>();
         sut = new PresentableTeXBuilderForSpecs(_presentationSettingsTask, _displayUnitRetriever);

         _parameter1 = DomainHelperForSpecs.ConstantParameterWithValue(1).WithName("P1");
         _parameter1.Dimension = DomainHelperForSpecs.LengthDimensionForSpecs();

         _parameter2 = DomainHelperForSpecs.ConstantParameterWithValue(2).WithName("P2");
         _parameter2.Dimension = DomainHelperForSpecs.TimeDimensionForSpecs();

         _parameter3 = DomainHelperForSpecs.ConstantParameterWithValue(3).WithName("P3");
         _parameter3.Dimension = DomainHelperForSpecs.LengthDimensionForSpecs();


         _container = new Container
         {
            _parameter1,
            _parameter2,
            _parameter3,
         }.WithId("Id");

         _presentationSettings = new DefaultPresentationSettings();

         //existing unit
         _presentationSettings.SetSetting(_parameter1.Name, "cm");

         //wrong unit for dimension
         _presentationSettings.SetSetting(_parameter2.Name, "cm");

         A.CallTo(() => _displayUnitRetriever.PreferredUnitFor(_parameter1)).Returns(_parameter1.Dimension.BaseUnit);
         A.CallTo(() => _displayUnitRetriever.PreferredUnitFor(_parameter2)).Returns(_parameter2.Dimension.BaseUnit);

         //wrong preferred unit.
         A.CallTo(() => _displayUnitRetriever.PreferredUnitFor(_parameter3)).Returns(_parameter2.Dimension.DefaultUnit);

         A.CallTo(_presentationSettingsTask).WithReturnType<DefaultPresentationSettings>().Returns(_presentationSettings);
      }

      protected override void Because()
      {
         sut.Build(_container, new OSPSuiteTracker());
      }

      private class PresentableTeXBuilderForSpecs : PresentableTeXBuilder<Container, DefaultPresentationSettings>
      {
         public PresentableTeXBuilderForSpecs(IPresentationSettingsTask presentationSettingsTask, IDisplayUnitRetriever displayUnitRetriever) : base(presentationSettingsTask, displayUnitRetriever)
         {
         }

         public override void Build(Container container, OSPSuiteTracker buildTracker)
         {
            var settings = PresentationSettingsFor(container, "TOTO");
            UpdateParameterDisplayUnit(container.AllParameters(), settings);
         }
      }
   }

   public class When_updating_the_diplay_units_of_parameters_based_on_existing_settings : concern_for_PresentableTEXBuilder
   {
      [Observation]
      public void should_use_the_predefined_settings_if_available()
      {
         _parameter1.DisplayUnit.Name.ShouldBeEqualTo("cm");
      }

      [Observation]
      public void should_fall_back_to_the_preferred_unit_if_defined()
      {
         _parameter2.DisplayUnit.Name.ShouldBeEqualTo(_parameter2.Dimension.BaseUnit.Name);
      }

      [Observation]
      public void shoudl_favor_default_unit_as_last_option()
      {
         _parameter3.DisplayUnit.Name.ShouldBeEqualTo(_parameter3.Dimension.DefaultUnit.Name);
      }
   }
}