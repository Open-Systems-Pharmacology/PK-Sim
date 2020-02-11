using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Presentation.Core;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Presentation.Presenters.Compounds;
using PKSim.Presentation.Services;

namespace PKSim.Presentation
{
   public abstract class concern_for_CompoundProcessPresentationTask : ContextSpecification<ICompoundProcessPresentationTask>
   {
      protected ICompoundProcessRepository _compoundProcessRepository;
      protected IExecutionContext _executionContext;
      protected IApplicationController _applicationController;

      protected override void Context()
      {
         _compoundProcessRepository = A.Fake<ICompoundProcessRepository>();
         _executionContext = A.Fake<IExecutionContext>();
         _applicationController = A.Fake<IApplicationController>();
         sut = new CompoundProcessPresentationTask(_applicationController, _compoundProcessRepository, _executionContext);
      }
   }

   public abstract class When_comparing_partial_process_type_and_inherited_types<TFirstType, TSecondType> : concern_for_CompoundProcessPresentationTask
      where TSecondType : TFirstType
      where TFirstType : PartialProcess
   {
      private bool _result;

      protected override void Because()
      {
         _result = sut.AreProcessesBoth<TFirstType>(typeof(TFirstType), typeof(TSecondType));
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

   public abstract class When_adding_process_for_molecule_when_process_are_subtypes_of_enzymatic<TProcessType> : concern_for_CompoundProcessPresentationTask
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
         sut.AddPartialProcessesForMolecule(_compound, _moleculeName, typeof(TProcessType));
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

   public class When_adding_enzymatic_process_for_molecule_when_process_type_is_enzymatic_with_species_ : When_adding_process_for_molecule_when_process_are_subtypes_of_enzymatic<EnzymaticProcessWithSpecies>
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
}