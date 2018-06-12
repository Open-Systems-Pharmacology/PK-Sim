using System.Collections;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Events;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Events;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.DTO.Parameters;
using PKSim.Presentation.DTO.Protocols;
using PKSim.Presentation.Views.Protocols;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Presentation.DTO;
using OSPSuite.Presentation.Presenters;

namespace PKSim.Presentation.Presenters.Protocols
{
   public interface IAdvancedProtocolPresenter : IPresenter<IAdvancedProtocolView>,
      IProtocolItemPresenter,
      IListener<AddSchemaItemToSchemaEvent>,
      IListener<RemoveSchemaItemFromSchemaEvent>,
      IListener<AddSchemaToProtocolEvent>,
      IListener<RemoveSchemaFromProtocolEvent>
   {
      void AddNewSchema();
      void RemoveSchema(SchemaDTO schemaToRemove);
      void SetApplicationType(SchemaItemDTO schemaItemDTO, ApplicationType newApplicationType);
      void SetFormulationType(SchemaItemDTO schemaItemDTO, string newFormulationType);
      void AddSchemaItemTo(SchemaDTO schemaDTO, SchemaItemDTO schemaItemDTOToDupicate);
      void RemoveSchemaItem(SchemaItemDTO schemaItemDTO);
      IList DynamicParametersFor(SchemaItemDTO schemaItemDTO);
      bool HasDynamicParameters(SchemaItemDTO schemaItemDTO);
      IEnumerable<Unit> AllTimeUnits();
      void ProtocolUnitChanged();
   }

   public class AdvancedProtocolPresenter : ProtocolItemPresenter<IAdvancedProtocolView, IAdvancedProtocolPresenter>, IAdvancedProtocolPresenter
   {
      private AdvancedProtocol _protocol;
      //if using a binding list here, it will be necessary to rebind in presenter
      private NotifyList<SchemaDTO> _allSchemas;
      private readonly ISchemaItemToSchemaItemDTOMapper _schemaItemDTOMapper;
      private readonly ISchemaToSchemaDTOMapper _schemaDTOMapper;
      private readonly IParameterToParameterDTOMapper _parameterDTOMapper;
      private readonly IDimensionRepository _dimensionRepository;

      public AdvancedProtocolPresenter(IAdvancedProtocolView view, ISchemaItemToSchemaItemDTOMapper schemaItemDTOMapper,
         ISchemaToSchemaDTOMapper schemaDTOMapper, IParameterToParameterDTOMapper parameterDTOMapper,
         IProtocolTask protocolTask, IParameterTask parameterTask, IDimensionRepository dimensionRepository)
         : base(view, protocolTask, parameterTask)
      {
         _schemaItemDTOMapper = schemaItemDTOMapper;
         _schemaDTOMapper = schemaDTOMapper;
         _parameterDTOMapper = parameterDTOMapper;
         _dimensionRepository = dimensionRepository;
      }

      public override void EditProtocol(Protocol protocol)
      {
         _protocol = protocol.DowncastTo<AdvancedProtocol>();
         _allSchemas = new NotifyList<SchemaDTO>(_protocol.AllSchemas.MapAllUsing(_schemaDTOMapper).ToList());

         _view.BindToProperties(_protocol);
         _view.BindToSchemas(_allSchemas);
      }

      public override IEnumerable<ApplicationType> AllApplications()
      {
         return ApplicationTypes.All().Where(x => !x.UserDefined);
      }

      public void AddNewSchema()
      {
         AddCommand(_protocolTask.AddSchemaTo(_protocol));
      }

      public void RemoveSchema(SchemaDTO schemaToRemove)
      {
         AddCommand(_protocolTask.RemoveSchemaFrom(SchemaFrom(schemaToRemove), _protocol));
      }

      public void SetApplicationType(SchemaItemDTO schemaItemDTO, ApplicationType newApplicationType)
      {
         AddCommand(_protocolTask.SetApplicationType(SchemaItemFrom(schemaItemDTO), newApplicationType));
      }

      public void SetFormulationType(SchemaItemDTO schemaItemDTO, string newFormulationType)
      {
         AddCommand(_protocolTask.SetFormulationType(SchemaItemFrom(schemaItemDTO), newFormulationType));
      }

      protected SchemaItem SchemaItemFrom(SchemaItemDTO schemaItemDTO)
      {
         return schemaItemDTO.SchemaItem;
      }

      protected Schema SchemaFrom(SchemaDTO schemaDTO)
      {
         return schemaDTO.Schema;
      }

      public void AddSchemaItemTo(SchemaDTO schemaDTO, SchemaItemDTO schemaItemDTOToDupicate)
      {
         var schema = SchemaFrom(schemaDTO);
         var schemaItem = SchemaItemFrom(schemaItemDTOToDupicate);
         AddCommand(_protocolTask.AddSchemaItemTo(schema, schemaItem));
      }

      public void RemoveSchemaItem(SchemaItemDTO schemaItemDTOToDelete)
      {
         var schemaDTO = schemaItemDTOToDelete.ParentSchema;
         var schema = SchemaFrom(schemaDTO);
         var schemaItemToDelete = SchemaItemFrom(schemaItemDTOToDelete);
         AddCommand(_protocolTask.RemoveSchemaItemFrom(schemaItemToDelete, schema));
      }

      public IList DynamicParametersFor(SchemaItemDTO schemaItemDTO)
      {
         if (schemaItemDTO == null)
            return new List<IParameterDTO>();

         return _protocolTask.AllDynamicParametersFor(SchemaItemFrom(schemaItemDTO)).MapAllUsing(_parameterDTOMapper).ToList();
      }

      public bool HasDynamicParameters(SchemaItemDTO schemaItemDTO)
      {
         if (schemaItemDTO == null) return false;
         return _protocolTask.AllDynamicParametersFor(SchemaItemFrom(schemaItemDTO)).Any();
      }

      public IEnumerable<Unit> AllTimeUnits()
      {
         return _dimensionRepository.Time.Units;
      }

      public void ProtocolUnitChanged()
      {
         OnStatusChanged();
      }

      public override void AddCommand(ICommand commandToAdd)
      {
         base.AddCommand(commandToAdd);
         _view.UpdateEndTime();
      }

      public void Handle(AddSchemaItemToSchemaEvent eventToHandle)
      {
         if (!canHandle(eventToHandle)) return;
         var schemaDTO = schemaDTOFrom(eventToHandle.Container);
         schemaDTO.AddSchemaItem(_schemaItemDTOMapper.MapFrom(eventToHandle.Entity));
         _view.Rebind();
         OnStatusChanged();
      }

      public void Handle(RemoveSchemaItemFromSchemaEvent eventToHandle)
      {
         if (!canHandle(eventToHandle)) return;
         var schemaDTO = schemaDTOFrom(eventToHandle.Container);
         var schemaItemDTOToDelete = schemaDTO.SchemaItems.First(x => x.SchemaItem == eventToHandle.Entity);
         schemaDTO.RemoveSchemaItem(schemaItemDTOToDelete);
         _view.Rebind();
         OnStatusChanged();
      }

      private SchemaDTO schemaDTOFrom(Schema schema)
      {
         return _allSchemas.First(dto => Equals(dto.Schema, schema));
      }

      public void Handle(AddSchemaToProtocolEvent eventToHandle)
      {
         if (!canHandle(eventToHandle)) return;
         _allSchemas.Add(_schemaDTOMapper.MapFrom(eventToHandle.Entity));
         OnStatusChanged();
      }

      public void Handle(RemoveSchemaFromProtocolEvent eventToHandle)
      {
         if (!canHandle(eventToHandle)) return;
         _allSchemas.Remove(schemaDTOFrom(eventToHandle.Entity));
         OnStatusChanged();
      }

      private bool canHandle(EntityContainerEvent<SchemaItem, Schema> schemaItemEvent)
      {
         if (_protocol == null) return false;
         return _protocol.Contains(schemaItemEvent.Container);
      }

      private bool canHandle(EntityContainerEvent<Schema, AdvancedProtocol> schemaEvent)
      {
         return Equals(_protocol, schemaEvent.Container);
      }
   }
}