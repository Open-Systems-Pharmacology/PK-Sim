using System;
using System.Collections.Generic;
using OSPSuite.BDDHelper;
using FakeItEasy;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Presentation.Presenters;
using PKSim.Presentation.Presenters.Compounds;
using PKSim.Presentation.Presenters.Parameters;
using PKSim.Presentation.Views.Compounds;
using OSPSuite.Core.Domain;

namespace PKSim.Presentation
{
   public abstract class concern_for_CompoundInSimulationPresenter : ContextSpecification<CompoundInSimulationPresenter>
   {
      protected IWithIdRepository _withIdRepository;
      protected ICalculationMethodSelectionPresenterForSimulation _calculationMethodSelectionPresenterForSimulation;
      protected ICompoundInSimulationView _compoundInSimulationView;
      protected IMultiParameterEditPresenter _simpleParameterPresenter;
      protected IMultiParameterEditPresenter _advancedParameterPresenter;

      protected override void Context()
      {
         _withIdRepository = A.Fake<IWithIdRepository>();
         _calculationMethodSelectionPresenterForSimulation = A.Fake<ICalculationMethodSelectionPresenterForSimulation>();
         _compoundInSimulationView = A.Fake<ICompoundInSimulationView>();
         _simpleParameterPresenter = A.Fake<IMultiParameterEditPresenter>();
         _advancedParameterPresenter = A.Fake<IMultiParameterEditPresenter>();
         sut = new CompoundInSimulationPresenter(_compoundInSimulationView, A.Fake<IMolWeightGroupPresenter>(),
         A.Fake<ICompoundTypeGroupPresenter>(), _simpleParameterPresenter,
         _advancedParameterPresenter, _calculationMethodSelectionPresenterForSimulation,
         _withIdRepository);
      }
   }

   public abstract class when_editing_parameters : concern_for_CompoundInSimulationPresenter
   {
      protected Simulation _simulation;
      protected CompoundProperties _compoundProperties;
      private IList<IParameter> _pkSimParameters;

      protected override void Context()
      {
         base.Context();
         _simulation = new IndividualSimulation { Properties = new SimulationProperties() };
         A.CallTo(() => _withIdRepository.Get<Simulation>(A<string>._)).Returns(_simulation);
      }

      protected override void Because()
      {
         _pkSimParameters = new List<IParameter> {new PKSimParameter {ParentContainer = A.Fake<IContainer>()}};
         sut.Edit(_pkSimParameters);
      }
   }

   public class when_editing_the_bile_salt_micelle_parameters_of_a_compound_in_a_simulation : concern_for_CompoundInSimulationPresenter
   {
      private IParameter _micellarConcentration;
      private IParameter _neutralPartitionCoefficient;
      private IParameter _lipophilicity;

      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _withIdRepository.Get<Simulation>(A<string>._)).Returns(new IndividualSimulation {Properties = new SimulationProperties()});
         _micellarConcentration = parameterInGroup(CoreConstants.Groups.COMPOUND_ADVANCED_SOLUBILITY);
         _neutralPartitionCoefficient = parameterInGroup(CoreConstants.Groups.COMPOUND_BILE_SALT_PARTITION_COEFFICIENT);
         _lipophilicity = parameterInGroup(CoreConstants.Groups.COMPOUND_LIPOPHILICITY);
      }

      protected override void Because()
      {
         sut.Edit(new[] {_micellarConcentration, _neutralPartitionCoefficient, _lipophilicity});
      }

      [Observation]
      public void should_show_the_advanced_solubility_and_partition_coefficient_parameters_as_advanced_parameters()
      {
         A.CallTo(() => _advancedParameterPresenter.Edit(A<IEnumerable<IParameter>>.That.IsSameSequenceAs(_micellarConcentration, _neutralPartitionCoefficient)))
            .MustHaveHappened();
      }

      [Observation]
      public void should_leave_the_other_parameters_in_the_simple_parameters()
      {
         A.CallTo(() => _simpleParameterPresenter.Edit(A<IEnumerable<IParameter>>.That.IsSameSequenceAs(_lipophilicity)))
            .MustHaveHappened();
      }

      private IParameter parameterInGroup(string groupName) =>
         new PKSimParameter {GroupName = groupName, ParentContainer = A.Fake<IContainer>()};
   }

   public class when_editing_parameters_where_the_simulation_can_be_found_but_the_compound_properties_cannot : when_editing_parameters
   {
      [Observation]
      public void calculation_method_presenter_should_not_be_used_to_edit_the_compound_properties()
      {
         A.CallTo(() => _calculationMethodSelectionPresenterForSimulation.Edit(_compoundProperties, A<Func<CalculationMethodCategory, bool>>._)).MustNotHaveHappened();
      }

      [Observation]
      public void the_view_should_hide_the_calculation_method_view()
      {
         A.CallTo(() => _compoundInSimulationView.HideCachedView(_calculationMethodSelectionPresenterForSimulation.BaseView)).MustHaveHappened();
      }
   }

   public class when_editing_parameters_where_the_simulation_can_be_found_and_the_compound_properties_resolved : when_editing_parameters
   {
      protected override void Context()
      {
         base.Context();
         _compoundProperties = new CompoundProperties { Compound = A.Fake<Compound>() };
         _simulation.Properties.AddCompoundProperties(_compoundProperties);
      }

      [Observation]
      public void calculation_method_presenter_must_have_been_used_to_edit_the_compound_properties()
      {
         A.CallTo(() => _calculationMethodSelectionPresenterForSimulation.Edit(_compoundProperties, A<Func<CalculationMethodCategory, bool>>._)).MustHaveHappened();
      }

      [Observation]
      public void the_view_should_not_hide_the_calculation_method_view()
      {
         A.CallTo(() => _compoundInSimulationView.HideCachedView(_calculationMethodSelectionPresenterForSimulation.BaseView)).MustNotHaveHappened();
      }
   }
}
