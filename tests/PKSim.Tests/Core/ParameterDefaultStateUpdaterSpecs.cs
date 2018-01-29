using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using PKSim.Core.Model;
using PKSim.Core.Services;

namespace PKSim.Core
{
   public abstract class concern_for_ParameterDefaultStateUpdater : ContextSpecification<IParameterDefaultStateUpdater>
   {
      protected override void Context()
      {
         sut = new ParameterDefaultStateUpdater();
      }
   }

   public class When_updating_the_parameter_default_state_of_a_spatial_structure : concern_for_ParameterDefaultStateUpdater
   {
      private ISpatialStructure _spatialStructure;
      private IContainer _organism;
      private IContainer _anotherTopContainer;
      private IParameter _ga;
      private IParameter _age;
      private IParameter _height;
      private IParameter _p1;
      private IParameter _p2;

      protected override void Context()
      {
         base.Context();
         _organism = new Organism();
         _anotherTopContainer = new Container().WithName("Another container");
         _spatialStructure = new PKSimSpatialStructure();
         _spatialStructure.AddTopContainer(_organism);
         _spatialStructure.AddTopContainer(_anotherTopContainer);

         _ga = DomainHelperForSpecs.ConstantParameterWithValue().WithName(Constants.Parameters.GESTATIONAL_AGE);
         _age = DomainHelperForSpecs.ConstantParameterWithValue().WithName(CoreConstants.Parameters.AGE);
         _height = DomainHelperForSpecs.ConstantParameterWithValue().WithName(CoreConstants.Parameters.HEIGHT);

         _p1 = DomainHelperForSpecs.ConstantParameterWithValue().WithName("P1");
         _p2 = DomainHelperForSpecs.ConstantParameterWithValue().WithName("P2");
         _organism.Add(_ga);
         _organism.Add(_age);
         _organism.Add(_height);
         _organism.Add(_p1);
         _anotherTopContainer.Add(_p2);
      }

      protected override void Because()
      {
         sut.UpdateDefaultFor(_spatialStructure);
      }

      [Observation]
      public void should_update_the_individual_parameters_defined_in_the_organism_that_are_purely_configuration_and_should_not_appear_as_changed()
      {
         _ga.IsDefault.ShouldBeTrue();
         _age.IsDefault.ShouldBeTrue();
         _height.IsDefault.ShouldBeTrue();
      }

      [Observation]
      public void should_not_reset_other_parameters()
      {
         _p1.IsDefault.ShouldBeFalse();
         _p2.IsDefault.ShouldBeFalse();
      }
   }

   public class When_updating_the_parameter_default_state_of_an_event_building_block : concern_for_ParameterDefaultStateUpdater
   {
      private IEventGroupBuildingBlock _eventBuildingBlockCreator;
      private IEventGroupBuilder _eventGroupBuilder1;
      private IEventGroupBuilder _eventGroupBuilder2;
      private IParameter _startTime1;
      private IParameter _startTime2;
      private IParameter _dose;
      private IParameter _dosePerBodyWeight;
      private IParameter _p1;
      private IParameter _p2;
      private IEventBuilder _eventBuilder1;
      private IEventBuilder _eventBuilder2;

      protected override void Context()
      {
         base.Context();
         _eventGroupBuilder1 = new EventGroupBuilder().WithName("EG1");
         _eventGroupBuilder2 = new EventGroupBuilder().WithName("EG2");
         _eventBuildingBlockCreator = new EventGroupBuildingBlock {_eventGroupBuilder1, _eventGroupBuilder2};

         _startTime1 = DomainHelperForSpecs.ConstantParameterWithValue().WithName(Constants.Parameters.START_TIME);
         _startTime2 = DomainHelperForSpecs.ConstantParameterWithValue().WithName(Constants.Parameters.START_TIME);
         _dose = DomainHelperForSpecs.ConstantParameterWithValue().WithName(CoreConstants.Parameters.DOSE);
         _dosePerBodyWeight = DomainHelperForSpecs.ConstantParameterWithValue().WithName(CoreConstants.Parameters.DOSE_PER_BODY_WEIGHT);

         _p1 = DomainHelperForSpecs.ConstantParameterWithValue().WithName("P1");
         _p2 = DomainHelperForSpecs.ConstantParameterWithValue().WithName("P2");

         _eventBuilder1 = new EventBuilder().WithName("E1");
         _eventBuilder2 = new EventBuilder().WithName("E2");

         _eventGroupBuilder1.Add(_eventBuilder1);
         _eventGroupBuilder2.Add(_eventBuilder2);

         _eventBuilder1.Add(_startTime1);
         _eventBuilder1.Add(_p1);
         _eventBuilder2.Add(_startTime2);
         _eventBuilder1.Add(_dosePerBodyWeight);
         _eventBuilder2.Add(_dose);
         _eventBuilder2.Add(_p2);
      }

      protected override void Because()
      {
         sut.UpdateDefaultFor(_eventBuildingBlockCreator);
      }

      [Observation]
      public void should_update_the_application_and_event_parameters_that_are_purely_configuration_and_should_not_appear_as_changed()
      {
         _startTime1.IsDefault.ShouldBeTrue();
         _startTime2.IsDefault.ShouldBeTrue();
         _dose.IsDefault.ShouldBeTrue();
         _dosePerBodyWeight.IsDefault.ShouldBeTrue();
      }

      [Observation]
      public void should_not_reset_other_parameters()
      {
         _p1.IsDefault.ShouldBeFalse();
         _p2.IsDefault.ShouldBeFalse();
      }
   }
}