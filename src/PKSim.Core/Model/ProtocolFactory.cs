using System;
using OSPSuite.Core.Domain;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using OSPSuite.Core.Services;

namespace PKSim.Core.Model
{
   public enum ProtocolMode
   {
      Simple,
      Advanced
   }

   public interface IProtocolFactory
   {
      Protocol Create(ProtocolMode protocolMode);
      Protocol Create(ProtocolMode protocolMode, ApplicationType applicationType);
   }

   public class ProtocolFactory : IProtocolFactory
   {
      private readonly IObjectBaseFactory _objectBaseFactory;
      private readonly ISchemaFactory _schemaFactory;
      private readonly IParameterFactory _parameterFactory;
      private readonly IDimensionRepository _dimensionRepository;
      private readonly ISchemaItemParameterRetriever _schemaItemParameterRetriever;
      private readonly IDisplayUnitRetriever _displayUnitRetriever;

      public ProtocolFactory(IObjectBaseFactory objectBaseFactory, ISchemaFactory schemaFactory,
                             IParameterFactory parameterFactory, IDimensionRepository dimensionRepository,
                             ISchemaItemParameterRetriever schemaItemParameterRetriever, IDisplayUnitRetriever displayUnitRetriever)
      {
         _objectBaseFactory = objectBaseFactory;
         _schemaFactory = schemaFactory;
         _parameterFactory = parameterFactory;
         _dimensionRepository = dimensionRepository;
         _schemaItemParameterRetriever = schemaItemParameterRetriever;
         _displayUnitRetriever = displayUnitRetriever;
      }

      public Protocol Create(ProtocolMode protocolMode)
      {
         return Create(protocolMode, ApplicationTypes.IntravenousBolus);
      }

      public Protocol Create(ProtocolMode protocolMode, ApplicationType applicationType)
      {
         Protocol protocol;
         switch (protocolMode)
         {
            case ProtocolMode.Simple:
               protocol = createSimpleProtocol(applicationType);
               break;
            case ProtocolMode.Advanced:
               protocol = createAdvancedProtocol(applicationType);
               break;
            default:
               throw new ArgumentOutOfRangeException(nameof(protocolMode));
         }
         protocol.IsLoaded = true;
         return protocol;
      }

      private SimpleProtocol createSimpleProtocol(ApplicationType applicationType)
      {
         var protocol = _objectBaseFactory.Create<SimpleProtocol>();
         protocol.Root = _objectBaseFactory.Create<IRootContainer>();
         protocol.DosingInterval = DosingIntervals.Single;
         protocol.ApplicationType = applicationType;
         protocol.FormulationKey = applicationType.NeedsFormulation ? CoreConstants.DEFAULT_FORMULATION_KEY : string.Empty;

         foreach (var parameter in _schemaItemParameterRetriever.AllParametersFor(protocol.ApplicationType))
         {
            protocol.AddParameter(parameter);
         }

         protocol.AddParameter(_parameterFactory.CreateFor(Constants.Parameters.END_TIME, CoreConstants.DEFAULT_PROTOCOL_END_TIME_IN_MIN, Constants.Dimension.TIME, PKSimBuildingBlockType.Protocol));

         return protocol;
      }

      private AdvancedProtocol createAdvancedProtocol(ApplicationType applicationType)
      {
         var protocol = _objectBaseFactory.Create<AdvancedProtocol>();
         protocol.Root = _objectBaseFactory.Create<IRootContainer>();
         protocol.TimeUnit = _displayUnitRetriever.PreferredUnitFor(_dimensionRepository.Time);

         var schema = _schemaFactory.CreateWithDefaultItem(applicationType, protocol);
         protocol.AddSchema(schema);
         return protocol;
      }
   }
}