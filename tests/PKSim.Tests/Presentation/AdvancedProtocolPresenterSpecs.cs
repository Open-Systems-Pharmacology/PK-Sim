using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Commands.Core;
using PKSim.Core;
using PKSim.Core.Commands;
using PKSim.Core.Events;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Mappers;

using PKSim.Presentation.DTO.Protocols;
using PKSim.Presentation.Presenters.Protocols;
using FakeItEasy;
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
      protected  PKSim.Core.Model.AdvancedProtocol _advancedProtocol;
      protected IList<Schema> _allSchemas;
      protected IProtocolTask _protocolTask;
      protected IDimensionRepository _dimensionRepository;

      protected override void Context()
      {
         _advancedProtocol = A.Fake< PKSim.Core.Model.AdvancedProtocol>();
         _allSchemas = new List<Schema>();
         A.CallTo(() => _advancedProtocol.AllSchemas).Returns(_allSchemas);

         _schemaDTOMapper = A.Fake<ISchemaToSchemaDTOMapper>();
         _schemaItemDTOMapper = A.Fake<ISchemaItemToSchemaItemDTOMapper>();
         _parameterDTOMapper = A.Fake<IParameterToParameterDTOMapper>();
         _protocolTask = A.Fake<IProtocolTask>();
         _parameterTask = A.Fake<IParameterTask>();
         _view = A.Fake<IAdvancedProtocolView>();
         _dimensionRepository =A.Fake<IDimensionRepository>();
         sut = new AdvancedProtocolPresenter(_view, _schemaItemDTOMapper, _schemaDTOMapper, _parameterDTOMapper, _protocolTask, _parameterTask,_dimensionRepository);
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
         A.CallTo(() => _schema.SchemaItems).Returns(new[] {_schemaItem});
         _schemaItemDTOToDuplicate = new SchemaItemDTO(_schemaItem);
      }

      protected override void Because()
      {
         sut.AddSchemaItemTo(_schemaDTO, _schemaItemDTOToDuplicate);
      }

      [Observation]
      public void should_add_a_schema_item_to_the_schema()
      {
         A.CallTo(() => _protocolTask.AddSchemaItemTo(_schema,_schemaItem)).MustHaveHappened();
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
         A.CallTo(() => _schema.SchemaItems).Returns(new[] {_schemaItem, _schemaItemToDelete});
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
      public void shoud_add_a_schema_item_to_the_schema_and_register_the_command()
      {
         A.CallTo(() => sut.CommandCollector.AddCommand(_removeSchemaCommand)).MustHaveHappened();
      }
   }

   
   public class When_notifed_that_a_schema_item_was_added_to_a_schema_belonging_to_the_edited_protocol : concern_for_AdvancedProtocolPresenter
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
         sut.Handle(new AddSchemaItemToSchemaEvent {Container = _schema, Entity = _schemaItem});
      }

      [Observation]
      public void should_update_the_view_accordingly()
      {
         A.CallTo(() => _schemaDTO.AddSchemaItem(_schemaItemDTO)).MustHaveHappened();
      }
   }

   
   public class When_notifed_that_a_schema_item_was_added_to_a_schema_that_does_not_belong_to_the_edited_protocol : concern_for_AdvancedProtocolPresenter
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
         sut.Handle(new AddSchemaItemToSchemaEvent {Container = _schema, Entity = _schemaItem});
      }

      [Observation]
      public void should_not_do_anything()
      {
         A.CallTo(() => _schemaDTO.AddSchemaItem(A<SchemaItemDTO>.Ignored)).MustNotHaveHappened();
      }
   }

   
   public class When_notifed_that_a_schema_item_was_removed_from_a_schema_that_does_not_belong_to_the_edited_protocol : concern_for_AdvancedProtocolPresenter
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
         sut.Handle(new RemoveSchemaItemFromSchemaEvent {Container = _schema, Entity = _schemaItem});
      }

      [Observation]
      public void should_not_do_anything()
      {
         A.CallTo(() => _schemaDTO.RemoveSchemaItem(A<SchemaItemDTO>.Ignored)).MustNotHaveHappened();
      }
   }

   
   public class When_notifed_that_a_schema_item_was_removed_from_a_schema_that_does_belong_to_the_edited_protocol : concern_for_AdvancedProtocolPresenter
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
         A.CallTo(() => _schemaDTO.SchemaItems).Returns(new BindingList<SchemaItemDTO> {_schemaItemDTO});
         A.CallTo(() => _advancedProtocol.Contains(_schema)).Returns(true);
         _allSchemas.Add(_schema);
         A.CallTo(() => _schemaDTOMapper.MapFrom(_schema)).Returns(_schemaDTO);
         sut.EditProtocol(_advancedProtocol);
      }

      protected override void Because()
      {
         sut.Handle(new RemoveSchemaItemFromSchemaEvent {Container = _schema, Entity = _schemaItem});
      }

      [Observation]
      public void should_not_do_anything()
      {
         A.CallTo(() => _schemaDTO.RemoveSchemaItem(_schemaItemDTO)).MustHaveHappened();
      }
   }


   
   public class When_notifed_that_a_schema_item_was_removed_from_a_schema_for_a_presenter_that_was_not_initialized : concern_for_AdvancedProtocolPresenter
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

   
   public class When_notifed_that_a_schema_item_was_added_to_a_schema_for_a_presenter_that_was_not_initialized : concern_for_AdvancedProtocolPresenter
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
         _results =  sut.AllApplications();
      }

      [Observation]
      public void should_not_returned_user_defined_type()
      {
         _results.Any(x=>x.UserDefined).ShouldBeFalse();
      }

      [Observation]
      public void should_return_some_application()
      {
         _results.Count().ShouldBeGreaterThan(0);
      }
   }
}