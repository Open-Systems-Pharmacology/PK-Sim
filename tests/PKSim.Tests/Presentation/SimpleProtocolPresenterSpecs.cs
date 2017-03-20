using System.Collections.Generic;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Commands.Core;
using FakeItEasy;
using PKSim.Presentation.Presenters.Parameters;
using PKSim.Presentation.Presenters.Protocols;
using PKSim.Presentation.Views.Protocols;
using OSPSuite.Core.Domain;
using PKSim.Core;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Mappers;

namespace PKSim.Presentation
{
   public abstract class concern_for_SimpleProtocolPresenter : ContextSpecification<ISimpleProtocolPresenter>
   {
      protected ISimpleProtocolView _view;
      protected ISchemaToSchemaDTOMapper _schemaDTOMapper;
      protected IParameterTask _parameterTask;
      private ISimpleProtocolToSimpleProtocolDTOMapper _simpleProtocolToSimpleProtocolDTOMapper;
      protected IMultiParameterEditPresenter _dynamicParameterPresenter;
      protected IProtocolTask _protocolTask;
      private IIndividualFactory _individualFactory;
      private IRepresentationInfoRepository _representationInfoRepository;
      private Individual _individual;

      protected override void Context()
      {
         _view = A.Fake<ISimpleProtocolView>();
         _schemaDTOMapper = A.Fake<ISchemaToSchemaDTOMapper>();
         _protocolTask = A.Fake<IProtocolTask>();
         _parameterTask = A.Fake<IParameterTask>();
         _individualFactory = A.Fake<IIndividualFactory>();
         _representationInfoRepository = A.Fake<IRepresentationInfoRepository>();
         _dynamicParameterPresenter = A.Fake<IMultiParameterEditPresenter>();
         _simpleProtocolToSimpleProtocolDTOMapper = A.Fake<ISimpleProtocolToSimpleProtocolDTOMapper>();
         _individual = DomainHelperForSpecs.CreateIndividual();
         A.CallTo(() => _individualFactory.CreateParameterLessIndividual()).Returns(_individual);
         sut = new SimpleProtocolPresenter(_view, _dynamicParameterPresenter, _simpleProtocolToSimpleProtocolDTOMapper,
                                           _protocolTask, _parameterTask, _individualFactory, _representationInfoRepository);
         sut.InitializeWith(A.Fake<ICommandCollector>());
      }
   }

   public class When_the_simple_application_presenter_is_retrieving_all_available_applications : concern_for_SimpleProtocolPresenter
   {
      [Observation]
      public void should_return_all_the_defined_applications()
      {
         sut.AllApplications().ShouldBeEqualTo(ApplicationTypes.All());
      }
   }

   public class When_the_simple_protocol_presenter_is_initializing : concern_for_SimpleProtocolPresenter
   {
      [Observation]
      public void should_add_the_parameter_view_to_the_protocol_view()
      {
         A.CallTo(() => _view.AddDynamicParameterView(_dynamicParameterPresenter.View)).MustHaveHappened();
      }
   }

   public class When_the_application_type_was_changed : concern_for_SimpleProtocolPresenter
   {
      private ApplicationType _newApplicationType;
      private SimpleProtocol _simpleProtocol;
      private IPKSimCommand _setApplicationTypeCommand;
      private IParameter _para1;
      private IParameter _para2;
      private IEnumerable<IParameter> _editedParameters;

      protected override void Context()
      {
         base.Context();
         _newApplicationType = ApplicationTypes.UserDefined;
         _simpleProtocol = new SimpleProtocol();
         _para1 = A.Fake<IParameter>().WithName("toto");
         _para2 = A.Fake<IParameter>().WithName("tat");
         _setApplicationTypeCommand = A.Fake<IPKSimCommand>();
         A.CallTo(() => _protocolTask.AllDynamicParametersFor(_simpleProtocol)).Returns(new[] {_para1, _para2});
         A.CallTo(() => _protocolTask.SetApplicationType(_simpleProtocol, _newApplicationType)).Returns(_setApplicationTypeCommand);
         _simpleProtocol.ApplicationType = ApplicationTypes.UserDefined;
         sut.EditProtocol(_simpleProtocol);
         A.CallTo(() => _dynamicParameterPresenter.Edit(A<IEnumerable<IParameter>>._))
            .Invokes(x => _editedParameters = x.GetArgument<IEnumerable<IParameter>>(0));
      }

      protected override void Because()
      {
         sut.SetApplicationType(_newApplicationType);
      }

      [Observation]
      public void should_update_the_dynamic_parameter_with_all_parameters_defined_for_the_new_application_type()
      {
         _editedParameters.ShouldOnlyContain(_para1, _para2);
      }

      [Observation]
      public void should_register_the_command_corresponding_to_the_application_type_set()
      {
         A.CallTo(() => sut.CommandCollector.AddCommand(_setApplicationTypeCommand)).MustHaveHappened();
      }

      [Observation]
      public void should_display_the_target_selection_for_a_user_defined_protocol()
      {
         _view.TargetDefinitionVisible.ShouldBeTrue();
      }
   }


   public class When_retrieving_all_possible_organs_available_for_a_user_defined_application : concern_for_SimpleProtocolPresenter
   {
      private IEnumerable<string> _organs;

      protected override void Because()
      {
         _organs = sut.AllOrgans();
      }

      [Observation]
      public void should_return_all_tissues_vascular_and_lumen_organs()
      {
         _organs.ShouldNotContain(CoreConstants.Organ.Liver);
         _organs.ShouldContain(CoreConstants.Compartment.Periportal);
      }
   }


   public class When_retrieving_the_list_of_compartments_defined_for_an_organ : concern_for_SimpleProtocolPresenter
   {
      [Observation]
      public void should_return_the_list_of_direct_compartments_for_a_standard_organ()
      {
         sut.AllCompartmentsFor(CoreConstants.Organ.Kidney).ShouldContain(CoreConstants.Compartment.Plasma);
      }

      [Observation]
      public void should_return_the_list_of_direct_container_for_a_liver_zone()
      {
         sut.AllCompartmentsFor(CoreConstants.Compartment.Periportal).ShouldContain(CoreConstants.Compartment.Intracellular);
      }
   }
}