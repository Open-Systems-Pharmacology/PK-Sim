using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Commands;
using PKSim.Core.Events;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.DTO.Protocols;
using PKSim.Presentation.Presenters.Protocols;
using PKSim.Presentation.Views.Protocols;

namespace PKSim.Presentation
{
   public abstract class concern_for_AdvancedProtocolPresenter : ContextSpecification<IAdvancedProtocolPresenter>
   {
      private IAdvancedProtocolView _view;
      protected ISchemaToSchemaDTOMapper _schemaDTOMapper;
      protected IParameterTask _parameterTask;
      protected ISchemaItemToSchemaItemDTOMapper _schemaItemDTOMapper;
      protected IParameterToParameterDTOMapper _parameterDTOMapper;
      protected AdvancedProtocol _advancedProtocol;
      protected IList<Schema> _allSchemas;
      protected IProtocolTask _protocolTask;
      protected IDimensionRepository _dimensionRepository;
      protected IIndividualFactory _individualFactory;
      protected IRepresentationInfoRepository _representationInfoRepository;

      protected override void Context()
      {
         _advancedProtocol = A.Fake<AdvancedProtocol>();
         _allSchemas = new List<Schema>();
         A.CallTo(() => _advancedProtocol.AllSchemas).Returns(_allSchemas);

         _schemaDTOMapper = A.Fake<ISchemaToSchemaDTOMapper>();
         _schemaItemDTOMapper = A.Fake<ISchemaItemToSchemaItemDTOMapper>();
         _parameterDTOMapper = A.Fake<IParameterToParameterDTOMapper>();
         _protocolTask = A.Fake<IProtocolTask>();
         _parameterTask = A.Fake<IParameterTask>();
         _view = A.Fake<IAdvancedProtocolView>();
         _dimensionRepository = A.Fake<IDimensionRepository>();
         _individualFactory = A.Fake<IIndividualFactory>();
         _representationInfoRepository = A.Fake<IRepresentationInfoRepository>();
         sut = new AdvancedProtocolPresenter(_view, _schemaItemDTOMapper, _schemaDTOMapper, _parameterDTOMapper, _protocolTask, _parameterTask, _dimensionRepository, _individualFactory, _representationInfoRepository);
         sut.InitializeWith(A.Fake<ICommandCollector>());
      }
   }

   public class When_the_advanced_protocol_presenter_is_asked_to_add_a_new_schema_to_the_protocol : concern_for_AdvancedProtocolPresenter
   {
      protected override void Context()
      {
         base.Context();
         _allSchemas.Add(A.Fake<Schema>());
         sut.EditProtocol(_advancedProtocol);
      }

      protected override void Because()
      {
         sut.AddNewSchema();
      }

      [Observation]
      public void should_add_a_schema_to_the_protocol()
      {
         A.CallTo(() => _protocolTask.AddSchemaTo(_advancedProtocol)).MustHaveHappened();
      }
   }

   public class When_the_advanced_protocol_presenter_is_asked_to_add_a_new_schema_item_to_a_schema : concern_for_AdvancedProtocolPresenter
   {
      private SchemaDTO _schemaDTO;
      private Schema _schema;
      private SchemaItem _schemaItem;
      private SchemaItemDTO _schemaItemDTOToDuplicate;

      protected override void Context()
      {
         base.Context();
         _schemaDTO = A.Fake<SchemaDTO>();
         _schemaItem = A.Fake<SchemaItem>();
         _schema = A.Fake<Schema>();

         _allSchemas.Add(_schema);
         A.CallTo(() => _schemaDTO.Schema).Returns(_schema);
         sut.EditProtocol(_advancedProtocol);
         A.CallTo(() => _schema.SchemaItems).Returns(new[] { _schemaItem });
         _schemaItemDTOToDuplicate = new SchemaItemDTO(_schemaItem);
      }

      protected override void Because()
      {
         sut.AddSchemaItemTo(_schemaDTO, _schemaItemDTOToDuplicate);
      }

      [Observation]
      public void should_add_a_schema_item_to_the_schema()
      {
         A.CallTo(() => _protocolTask.AddSchemaItemTo(_schema, _schemaItem)).MustHaveHappened();
      }
   }

   public class When_the_advanced_protocol_presenter_is_removing_a_schema_item_from_a_schema_containing_at_least_2_schema_items : concern_for_AdvancedProtocolPresenter
   {
      private SchemaDTO _schemaDTO;
      private Schema _schema;
      private SchemaItemDTO _schemaItemDTOToDelete;
      private SchemaItem _schemaItem;
      private SchemaItem _schemaItemToDelete;
      private IPKSimCommand _deleteSchemaItemCommand;

      protected override void Context()
      {
         base.Context();
         _schemaDTO = A.Fake<SchemaDTO>();
         _schemaItem = A.Fake<SchemaItem>();
         _schema = A.Fake<Schema>();
         _deleteSchemaItemCommand = A.Fake<IPKSimCommand>();
         _schemaItemDTOToDelete = DomainHelperForSpecs.SchemaItemDTO(ApplicationTypes.Intravenous);
         _schemaItemDTOToDelete.ParentSchema = _schemaDTO;
         _schemaItemToDelete = _schemaItemDTOToDelete.SchemaItem;
         A.CallTo(() => _schemaDTO.Schema).Returns(_schema);
         A.CallTo(() => _schema.SchemaItems).Returns(new[] { _schemaItem, _schemaItemToDelete });
         A.CallTo(() => _protocolTask.RemoveSchemaItemFrom(_schemaItemToDelete, _schema)).Returns(_deleteSchemaItemCommand);
      }

      protected override void Because()
      {
         sut.RemoveSchemaItem(_schemaItemDTOToDelete);
      }

      [Observation]
      public void should_remove_the_schema_item_from_the_schema_and_register_the_command()
      {
         A.CallTo(() => sut.CommandCollector.AddCommand(_deleteSchemaItemCommand)).MustHaveHappened();
      }
   }

   public class When_the_advanced_protocol_presenter_is_removing_a_schema_from_protocol_containing_at_least_two_schemas : concern_for_AdvancedProtocolPresenter
   {
      private SchemaDTO _schemaDTO;
      private Schema _schema;
      private Schema _schemaToDelete;
      private SchemaDTO _schemaToDeleteDTO;
      private IPKSimCommand _removeSchemaCommand;

      protected override void Context()
      {
         base.Context();
         _schemaDTO = A.Fake<SchemaDTO>();
         _schemaToDeleteDTO = A.Fake<SchemaDTO>();
         _schema = A.Fake<Schema>();
         _schemaToDelete = A.Fake<Schema>();
         _removeSchemaCommand = A.Fake<IPKSimCommand>();
         A.CallTo(() => _schemaDTO.Schema).Returns(_schema);
         A.CallTo(() => _schemaToDeleteDTO.Schema).Returns(_schemaToDelete);
         sut.EditProtocol(_advancedProtocol);
         _allSchemas.Add(_schema);
         _allSchemas.Add(_schemaToDelete);
         A.CallTo(() => _protocolTask.RemoveSchemaFrom(_schemaToDelete, _advancedProtocol)).Returns(_removeSchemaCommand);
      }

      protected override void Because()
      {
         sut.RemoveSchema(_schemaToDeleteDTO);
      }

      [Observation]
      public void should_add_a_schema_item_to_the_schema_and_register_the_command()
      {
         A.CallTo(() => sut.CommandCollector.AddCommand(_removeSchemaCommand)).MustHaveHappened();
      }
   }

   public class When_notified_that_a_schema_item_was_added_to_a_schema_belonging_to_the_edited_protocol : concern_for_AdvancedProtocolPresenter
   {
      private SchemaItem _schemaItem;
      private Schema _schema;
      private SchemaDTO _schemaDTO;
      private SchemaItemDTO _schemaItemDTO;

      protected override void Context()
      {
         base.Context();
         _schema = A.Fake<Schema>();
         _schemaDTO = A.Fake<SchemaDTO>();
         _schemaItemDTO = DomainHelperForSpecs.SchemaItemDTO(ApplicationTypes.Intravenous);
         _schemaItem = _schemaItemDTO.SchemaItem;
         A.CallTo(() => _schemaDTO.Schema).Returns(_schema);
         A.CallTo(() => _advancedProtocol.Contains(_schema)).Returns(true);
         _allSchemas.Add(_schema);
         A.CallTo(() => _schemaDTOMapper.MapFrom(_schema)).Returns(_schemaDTO);
         A.CallTo(() => _schemaItemDTOMapper.MapFrom(_schemaItem)).Returns(_schemaItemDTO);
         sut.EditProtocol(_advancedProtocol);
      }

      protected override void Because()
      {
         sut.Handle(new AddSchemaItemToSchemaEvent { Container = _schema, Entity = _schemaItem });
      }

      [Observation]
      public void should_update_the_view_accordingly()
      {
         A.CallTo(() => _schemaDTO.AddSchemaItem(_schemaItemDTO)).MustHaveHappened();
      }
   }

   public class When_notified_that_a_schema_item_was_added_to_a_schema_that_does_not_belong_to_the_edited_protocol : concern_for_AdvancedProtocolPresenter
   {
      private SchemaItem _schemaItem;
      private Schema _schema;
      private SchemaDTO _schemaDTO;

      protected override void Context()
      {
         base.Context();
         _schemaItem = A.Fake<SchemaItem>();
         _schema = A.Fake<Schema>();
         _schemaDTO = A.Fake<SchemaDTO>();
         A.CallTo(() => _schemaDTO.Schema).Returns(_schema);
         A.CallTo(() => _advancedProtocol.Contains(_schema)).Returns(false);
         _allSchemas.Add(_schema);
         A.CallTo(() => _schemaDTOMapper.MapFrom(_schema)).Returns(_schemaDTO);
         sut.EditProtocol(_advancedProtocol);
      }

      protected override void Because()
      {
         sut.Handle(new AddSchemaItemToSchemaEvent { Container = _schema, Entity = _schemaItem });
      }

      [Observation]
      public void should_not_do_anything()
      {
         A.CallTo(() => _schemaDTO.AddSchemaItem(A<SchemaItemDTO>.Ignored)).MustNotHaveHappened();
      }
   }

   public class When_notified_that_a_schema_item_was_removed_from_a_schema_that_does_not_belong_to_the_edited_protocol : concern_for_AdvancedProtocolPresenter
   {
      private SchemaItem _schemaItem;
      private Schema _schema;
      private SchemaDTO _schemaDTO;

      protected override void Context()
      {
         base.Context();
         _schemaItem = A.Fake<SchemaItem>();
         _schema = A.Fake<Schema>();
         _schemaDTO = A.Fake<SchemaDTO>();
         A.CallTo(() => _schemaDTO.Schema).Returns(_schema);
         A.CallTo(() => _advancedProtocol.Contains(_schema)).Returns(false);
         _allSchemas.Add(_schema);
         A.CallTo(() => _schemaDTOMapper.MapFrom(_schema)).Returns(_schemaDTO);
         sut.EditProtocol(_advancedProtocol);
      }

      protected override void Because()
      {
         sut.Handle(new RemoveSchemaItemFromSchemaEvent { Container = _schema, Entity = _schemaItem });
      }

      [Observation]
      public void should_not_do_anything()
      {
         A.CallTo(() => _schemaDTO.RemoveSchemaItem(A<SchemaItemDTO>.Ignored)).MustNotHaveHappened();
      }
   }

   public class When_notified_that_a_schema_item_was_removed_from_a_schema_that_does_belong_to_the_edited_protocol : concern_for_AdvancedProtocolPresenter
   {
      private SchemaItem _schemaItem;
      private Schema _schema;
      private SchemaDTO _schemaDTO;
      private SchemaItemDTO _schemaItemDTO;

      protected override void Context()
      {
         base.Context();
         _schemaItem = A.Fake<SchemaItem>();
         _schema = A.Fake<Schema>();
         _schemaDTO = A.Fake<SchemaDTO>();
         _schemaItemDTO = DomainHelperForSpecs.SchemaItemDTO(ApplicationTypes.Intravenous);
         _schemaItem = _schemaItemDTO.SchemaItem;
         A.CallTo(() => _schemaDTO.Schema).Returns(_schema);
         A.CallTo(() => _schemaDTO.SchemaItems).Returns(new BindingList<SchemaItemDTO> { _schemaItemDTO });
         A.CallTo(() => _advancedProtocol.Contains(_schema)).Returns(true);
         _allSchemas.Add(_schema);
         A.CallTo(() => _schemaDTOMapper.MapFrom(_schema)).Returns(_schemaDTO);
         sut.EditProtocol(_advancedProtocol);
      }

      protected override void Because()
      {
         sut.Handle(new RemoveSchemaItemFromSchemaEvent { Container = _schema, Entity = _schemaItem });
      }

      [Observation]
      public void should_not_do_anything()
      {
         A.CallTo(() => _schemaDTO.RemoveSchemaItem(_schemaItemDTO)).MustHaveHappened();
      }
   }

   public class When_notified_that_a_schema_item_was_removed_from_a_schema_for_a_presenter_that_was_not_initialized : concern_for_AdvancedProtocolPresenter
   {
      private SchemaItem _schemaItem;
      private Schema _schema;

      protected override void Context()
      {
         base.Context();
         _schemaItem = A.Fake<SchemaItem>();
         _schema = A.Fake<Schema>();
      }

      [Observation]
      public void should_not_crash()
      {
         sut.Handle(new RemoveSchemaItemFromSchemaEvent { Container = _schema, Entity = _schemaItem });
      }
   }

   public class When_notified_that_a_schema_item_was_added_to_a_schema_for_a_presenter_that_was_not_initialized : concern_for_AdvancedProtocolPresenter
   {
      private SchemaItem _schemaItem;
      private Schema _schema;

      protected override void Context()
      {
         base.Context();
         _schemaItem = A.Fake<SchemaItem>();
         _schema = A.Fake<Schema>();
      }

      [Observation]
      public void should_not_crash()
      {
         sut.Handle(new AddSchemaItemToSchemaEvent() { Container = _schema, Entity = _schemaItem });
      }
   }

   public class When_the_advanced_protocol_presenter_is_asked_for_the_available_application_type : concern_for_AdvancedProtocolPresenter
   {
      private IEnumerable<ApplicationType> _results;

      protected override void Because()
      {
         _results = sut.AllApplications();
      }

      [Observation]
      public void should_returned_all_available_types()
      {
         _results.ShouldOnlyContainInOrder(ApplicationTypes.All());
      }
   }

   public class When_retrieving_the_dynamic_content_for_a_user_defined_schema_item : concern_for_AdvancedProtocolPresenter
   {
      private SchemaItemDTO _schemaItemDTO;
      private SchemaItem _schemaItem;
      private SchemaItemTargetDTO _targetOrgan;
      private SchemaItemTargetDTO _targetCompartment;

      protected override void Context()
      {
         base.Context();
         _schemaItem = new SchemaItem { ApplicationType = ApplicationTypes.UserDefined, TargetOrgan = "Liv", TargetCompartment = "Cell" };

         _schemaItemDTO = new SchemaItemDTO(_schemaItem);
      }

      protected override void Because()
      {
         var result = sut.DynamicContentFor(_schemaItemDTO);
         _targetOrgan = result[0].DowncastTo<SchemaItemTargetDTO>();
         _targetCompartment = result[1].DowncastTo<SchemaItemTargetDTO>();
      }

      [Observation]
      public void should_return_two_elements_for_target_organ_and_target_compartment()
      {
         _targetCompartment.ShouldNotBeNull();
         _targetOrgan.ShouldNotBeNull();
      }

      [Observation]
      public void should_return_target_items_initialized_with_the_expected_values()
      {
         _targetCompartment.Target.ShouldBeEqualTo(_schemaItem.TargetCompartment);
         _targetCompartment.IsOrgan.ShouldBeFalse();

         _targetOrgan.Target.ShouldBeEqualTo(_schemaItem.TargetOrgan);
         _targetOrgan.IsOrgan.ShouldBeTrue();
      }
   }

   public class When_checking_if_a_schema_item_dto_has_dynamic_content_to_display : concern_for_AdvancedProtocolPresenter
   {
      private readonly SchemaItem _userDefinedSchemaItem = new SchemaItem { ApplicationType = ApplicationTypes.UserDefined };
      private readonly SchemaItem _schemaItemWithParameters = new SchemaItem { ApplicationType = ApplicationTypes.IntravenousBolus };
      private readonly SchemaItem _schemaItemWithoutParameters = new SchemaItem { ApplicationType = ApplicationTypes.Intravenous };

      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _protocolTask.AllDynamicParametersFor(_schemaItemWithParameters)).Returns(new[] { DomainHelperForSpecs.ConstantParameterWithValue(10) });
         A.CallTo(() => _protocolTask.AllDynamicParametersFor(_schemaItemWithoutParameters)).Returns(Enumerable.Empty<IParameter>());
      }

      [Observation]
      public void should_return_false_if_the_schema_item_is_undefined()
      {
         sut.HasDynamicContent(null).ShouldBeFalse();
      }

      [Observation]
      public void should_return_true_if_the_schema_item_represented_a_user_defined_application()
      {
         var schemaItemDTO = new SchemaItemDTO(_userDefinedSchemaItem);
         sut.HasDynamicContent(schemaItemDTO).ShouldBeTrue();
      }

      [Observation]
      public void should_return_true_if_the_schema_item_has_dynamic_parameters()
      {
         var schemaItemDTO = new SchemaItemDTO(_schemaItemWithParameters);
         sut.HasDynamicContent(schemaItemDTO).ShouldBeTrue();
      }

      [Observation]
      public void should_return_false_if_the_schema_item_does_not_have_dynamic_parameters()
      {
         var schemaItemDTO = new SchemaItemDTO(_schemaItemWithoutParameters);
         sut.HasDynamicContent(schemaItemDTO).ShouldBeFalse();
      }
   }

   public class When_the_advanced_protocol_presenter_is_setting_the_application_type_to_user_defined_for_a_schema_item : concern_for_AdvancedProtocolPresenter
   {
      private readonly SchemaItem _schemaItem = new SchemaItem { ApplicationType = ApplicationTypes.IntravenousBolus };
      private SchemaItemDTO _schemaItemDTO;

      protected override void Context()
      {
         base.Context();
         _schemaItemDTO = new SchemaItemDTO(_schemaItem);
      }

      protected override void Because()
      {
         sut.SetApplicationType(_schemaItemDTO, ApplicationTypes.UserDefined);
      }

      [Observation]
      public void should_also_set_the_default_value_for_target_and_compartment()
      {
         _schemaItem.TargetOrgan.ShouldBeEqualTo(CoreConstants.Organ.ARTERIAL_BLOOD);
         _schemaItem.TargetCompartment.ShouldBeEqualTo(CoreConstants.Compartment.PLASMA);
      }
   }

   public class SchematicItemEqualityComparer : GenericEqualityComparer<SchemaItem>
   {

   }
}