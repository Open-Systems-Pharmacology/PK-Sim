using System.Collections;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Presentation.DTO;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Events;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core.Events;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.DTO.Protocols;
using PKSim.Presentation.Views.Protocols;

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
      void AddSchemaItemTo(SchemaDTO schemaDTO, SchemaItemDTO schemaItemDTOToDuplicate);
      void RemoveSchemaItem(SchemaItemDTO schemaItemDTO);
      IList DynamicContentFor(SchemaItemDTO schemaItemDTO);
      bool HasDynamicContent(SchemaItemDTO schemaItemDTO);
      IEnumerable<Unit> AllTimeUnits();
      void ProtocolUnitChanged();
      void SetSchemaItemTarget(SchemaItemTargetDTO schemaItemTargetDTO, string target);
      IEnumerable<string> AllTargetsFor(SchemaItemTargetDTO schemaItemTargetDTO);
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

      public AdvancedProtocolPresenter(
         IAdvancedProtocolView view, 
         ISchemaItemToSchemaItemDTOMapper schemaItemDTOMapper,
         ISchemaToSchemaDTOMapper schemaDTOMapper, 
         IParameterToParameterDTOMapper parameterDTOMapper,
         IProtocolTask protocolTask, 
         IParameterTask parameterTask, 
         IDimensionRepository dimensionRepository, 
         IIndividualFactory individualFactory,
         IRepresentationInfoRepository representationInfoRepository)
         : base(view, protocolTask, parameterTask, individualFactory, representationInfoRepository)
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
         SetApplicationType(newApplicationType, SchemaItemFrom(schemaItemDTO));
      }

      public void SetFormulationType(SchemaItemDTO schemaItemDTO, string newFormulationType)
      {
         AddCommand(_protocolTask.SetFormulationType(SchemaItemFrom(schemaItemDTO), newFormulationType));
      }

      protected SchemaItem SchemaItemFrom(SchemaItemDTO schemaItemDTO) => schemaItemDTO.SchemaItem;

      protected Schema SchemaFrom(SchemaDTO schemaDTO) => schemaDTO.Schema;

      public void AddSchemaItemTo(SchemaDTO schemaDTO, SchemaItemDTO schemaItemDTOToDuplicate)
      {
         var schema = SchemaFrom(schemaDTO);
         var schemaItem = SchemaItemFrom(schemaItemDTOToDuplicate);
         AddCommand(_protocolTask.AddSchemaItemTo(schema, schemaItem));
      }

      public void RemoveSchemaItem(SchemaItemDTO schemaItemDTOToDelete)
      {
         var schemaDTO = schemaItemDTOToDelete.ParentSchema;
         var schema = SchemaFrom(schemaDTO);
         var schemaItemToDelete = SchemaItemFrom(schemaItemDTOToDelete);
         AddCommand(_protocolTask.RemoveSchemaItemFrom(schemaItemToDelete, schema));
      }

      public IList DynamicContentFor(SchemaItemDTO schemaItemDTO)
      {
         if (schemaItemDTO == null)
            return new List<IParameterDTO>();

         if (schemaItemDTO.IsUserDefined)
            return new List<SchemaItemTargetDTO>
            {
               new SchemaItemTargetDTO(PKSimConstants.UI.TargetOrgan, schemaItemDTO, x => x.TargetOrgan),
               new SchemaItemTargetDTO(PKSimConstants.UI.TargetCompartment, schemaItemDTO, x => x.TargetCompartment),
            };

         return _protocolTask.AllDynamicParametersFor(SchemaItemFrom(schemaItemDTO)).MapAllUsing(_parameterDTOMapper).ToList();
      }

      public bool HasDynamicContent(SchemaItemDTO schemaItemDTO)
      {
         if (schemaItemDTO == null)
            return false;

         return schemaItemDTO.IsUserDefined || _protocolTask.AllDynamicParametersFor(SchemaItemFrom(schemaItemDTO)).Any();
      }

      public IEnumerable<Unit> AllTimeUnits() => _dimensionRepository.Time.Units;

      public void ProtocolUnitChanged() => OnStatusChanged();

      public void SetSchemaItemTarget(SchemaItemTargetDTO schemaItemTargetDTO, string target)
      {
         var schemaItem = schemaItemTargetDTO.SchemaItemDTO.SchemaItem;
         if (schemaItemTargetDTO.IsOrgan)
            SetTargetOrgan(target, schemaItem);
         else
            SetTargetCompartment(target, schemaItem);
      }

      public IEnumerable<string> AllTargetsFor(SchemaItemTargetDTO schemaItemTargetDTO)
      {
         if (schemaItemTargetDTO.IsOrgan)
            return AllOrgans();

         return AllCompartmentsFor(schemaItemTargetDTO.SchemaItemDTO.TargetOrgan);
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