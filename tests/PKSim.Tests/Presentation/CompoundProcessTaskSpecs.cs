using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.Presenters.Compounds;
using PKSim.Presentation.Services;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.Core;

namespace PKSim.Presentation
{
   public abstract class concern_for_CompoundProcessTask : ContextSpecification<ICompoundProcessTask>
   {
      private ICompoundProcessRepository _compoundProcessRepository;
      protected IExecutionContext _executionContext;
      protected IApplicationController _applicationController;

      protected override void Context()
      {
         _compoundProcessRepository = A.Fake<ICompoundProcessRepository>();
         _executionContext = A.Fake<IExecutionContext>();
         _applicationController = A.Fake<IApplicationController>();
         sut = new CompoundProcessTask(_compoundProcessRepository, _executionContext, _applicationController);
      }
   }

   public abstract class When_comparing_partial_process_type_and_inherited_types<TFirstType, TSecondType> : concern_for_CompoundProcessTask
      where TSecondType : TFirstType
      where TFirstType : PartialProcess
   {
      private bool _result;

      protected override void Because()
      {
         _result = sut.AreProcessesBoth<TFirstType>(typeof (TFirstType), typeof (TSecondType));
      }

      [Observation]
      public void should_test_that_subtypes_of_a_process_types_are_equal()
      {
         _result.ShouldBeTrue();
      }
   }

   public class When_comparing_inhibition_and_own_type : When_comparing_partial_process_type_and_inherited_types<InhibitionProcess, InhibitionProcess>
   {
   }

   public class When_comparing_transport_and_subtype : When_comparing_partial_process_type_and_inherited_types<TransportPartialProcess, TransportPartialProcessWithSpecies>
   {
   }

   public class When_comparing_enzymatic_and_subtype : When_comparing_partial_process_type_and_inherited_types<EnzymaticProcess, EnzymaticProcessWithSpecies>
   {
   }

   public abstract class When_adding_process_for_molecule_when_process_are_subtypes_of_enzymatic<TProcessType> : concern_for_CompoundProcessTask
      where TProcessType : PartialProcess
   {
      private Compound _compound;
      private string _moleculeName;

      protected override void Context()
      {
         base.Context();
         _compound = A.Fake<Compound>();
         _moleculeName = "moleculeName";
      }

      protected override void Because()
      {
         sut.AddPartialProcessesForMolecule(_compound, _moleculeName, typeof (TProcessType));
      }

      [Observation]
      public abstract void should_result_in_a_call_to_create_an_appropriate_presenter();
   }

   public class When_adding_transport_process_for_molecule_when_process_type_is_specific_binding : When_adding_process_for_molecule_when_process_are_subtypes_of_enzymatic<SpecificBindingPartialProcess>
   {
      [Observation]
      public override void should_result_in_a_call_to_create_an_appropriate_presenter()
      {
         A.CallTo(() => _applicationController.Start<ICreateSpecificBindingPartialProcessPresenter>()).MustHaveHappened();
      }
   }

   public class When_adding_transport_process_for_molecule_when_process_type_is_inhibition : When_adding_process_for_molecule_when_process_are_subtypes_of_enzymatic<InhibitionProcess>
   {
      [Observation]
      public override void should_result_in_a_call_to_create_an_appropriate_presenter()
      {
         A.CallTo(() => _applicationController.Start<ICreateInhibitionProcessPresenter>()).MustHaveHappened();
      }
   }

   public class When_adding_transport_process_for_molecule_when_process_type_is_transport_with_species : When_adding_process_for_molecule_when_process_are_subtypes_of_enzymatic<TransportPartialProcessWithSpecies>
   {
      [Observation]
      public override void should_result_in_a_call_to_create_an_appropriate_presenter()
      {
         A.CallTo(() => _applicationController.Start<ICreateTransportPartialProcessPresenter>()).MustHaveHappened();
      }
   }

   public class When_adding_transport_process_for_molecule_when_process_type_is_transport : When_adding_process_for_molecule_when_process_are_subtypes_of_enzymatic<TransportPartialProcess>
   {
      [Observation]
      public override void should_result_in_a_call_to_create_an_appropriate_presenter()
      {
         A.CallTo(() => _applicationController.Start<ICreateTransportPartialProcessPresenter>()).MustHaveHappened();
      }
   }

   public class When_adding_enzyamtic_process_for_molecule_when_process_type_is_enzymatic_with_species_ : When_adding_process_for_molecule_when_process_are_subtypes_of_enzymatic<EnzymaticProcessWithSpecies>
   {
      [Observation]
      public override void should_result_in_a_call_to_create_an_appropriate_presenter()
      {
         A.CallTo(() => _applicationController.Start<ICreateEnzymaticProcessPresenter>()).MustHaveHappened();
      }
   }

   public class When_adding_enzymatic_process_for_molecule_when_process_type_is_enzymatic : When_adding_process_for_molecule_when_process_are_subtypes_of_enzymatic<EnzymaticProcess>
   {
      [Observation]
      public override void should_result_in_a_call_to_create_an_appropriate_presenter()
      {
         A.CallTo(() => _applicationController.Start<ICreateEnzymaticProcessPresenter>()).MustHaveHappened();
      }
   }

   public class When_creating_a_process_from_template : concern_for_CompoundProcessTask
   {
      private Compound _compound;
      private SystemicProcess _template;
      private CompoundProcess _result;
      private SystemicProcess _clone;
      private IParameter _p1;
      private IParameter _p2;
      private ParameterAlternative _fuAlternative;

      protected override void Context()
      {
         base.Context();
         _compound = new Compound();
         var fuGroup = new ParameterAlternativeGroup().WithName(CoreConstants.Groups.COMPOUND_FRACTION_UNBOUND);
         _fuAlternative = new ParameterAlternative().WithName("MyFu");
         _fuAlternative.Add(DomainHelperForSpecs.ConstantParameterWithValue(0.2).WithName(CoreConstants.Parameter.FRACTION_UNBOUND_PLASMA_REFERENCE_VALUE));
         _fuAlternative.IsDefault = true;
         fuGroup.AddAlternative(_fuAlternative);
         _compound.AddParameterAlternativeGroup(fuGroup);
         _template = new SystemicProcess();
         _p1 = DomainHelperForSpecs.ConstantParameterWithValue(0.5).WithName(CoreConstants.Parameter.FRATION_UNBOUND_EXPERIMENT);
         _p2 = DomainHelperForSpecs.ConstantParameterWithValue(0.9).WithName("toto");
         _template.Add(_p1);
         _template.Add(_p2);
         _clone = new SystemicProcess();
         _clone.Add(DomainHelperForSpecs.ConstantParameterWithValue(0.5).WithName(CoreConstants.Parameter.FRATION_UNBOUND_EXPERIMENT));

         A.CallTo(() => _executionContext.Clone(_template)).Returns(_clone);
      }

      protected override void Because()
      {
         _result = sut.CreateProcessFromTemplate(_template, _compound);
      }

      [Observation]
      public void should_return_a_clone_from_the_given_template()
      {
         _result.ShouldBeEqualTo(_clone);
      }

      [Observation]
      public void should_have_set_the_compound_specific_parameter_using_the_defined_value_if_available()
      {
         _result.Parameter(CoreConstants.Parameter.FRATION_UNBOUND_EXPERIMENT).Value.ShouldBeEqualTo(_fuAlternative.Parameter(CoreConstants.Parameter.FRACTION_UNBOUND_PLASMA_REFERENCE_VALUE).Value);
      }
   }
}