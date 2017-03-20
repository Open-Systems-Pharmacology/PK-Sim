using OSPSuite.Utility;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Services;
using OSPSuite.Core.Services;

namespace PKSim.Core.Batch.Mapper
{
   public interface IApplicationProtocolMapper : IMapper<ApplicationProtocol, Protocol>
   {
   }

   public class ApplicationProtocolMapper : IApplicationProtocolMapper
   {
      private readonly IProtocolFactory _protocolFactory;
      private readonly IBatchLogger _logger;

      public ApplicationProtocolMapper(IProtocolFactory protocolFactory, IBatchLogger logger)
      {
         _protocolFactory = protocolFactory;
         _logger = logger;
      }

      public Protocol MapFrom(ApplicationProtocol batchProtocol)
      {
         var applicationType = ApplicationTypes.ByName(batchProtocol.ApplicationType);
         var dosingIntervalId = EnumHelper.ParseValue<DosingIntervalId>(batchProtocol.DosingInterval);
         var simpleProtocol = _protocolFactory.Create(ProtocolMode.Simple, applicationType).DowncastTo<SimpleProtocol>();
         simpleProtocol.Name = batchProtocol.Name ?? "Protocol";
         simpleProtocol.EndTimeParameter.Value = batchProtocol.EndTime;
         simpleProtocol.Dose.DisplayUnit = simpleProtocol.Dose.Dimension.Unit(batchProtocol.DoseUnit);
         simpleProtocol.Dose.Value = simpleProtocol.Dose.Dimension.UnitValueToBaseUnitValue(simpleProtocol.Dose.DisplayUnit, batchProtocol.Dose);
         simpleProtocol.DosingInterval = DosingIntervals.ById(dosingIntervalId);

         _logger.AddDebug("Application Type = {0}".FormatWith(applicationType.Name));
         _logger.AddDebug("Dosing Interval = {0}".FormatWith(simpleProtocol.DosingInterval.DisplayName));
         _logger.AddDebug("Application Dose = {0} [{1}]".FormatWith(simpleProtocol.Dose.Value, simpleProtocol.Dose.DisplayUnit));
         _logger.AddDebug("Application End Time = {0}".FormatWith(simpleProtocol.EndTime));

         return simpleProtocol;
      }
   }
}