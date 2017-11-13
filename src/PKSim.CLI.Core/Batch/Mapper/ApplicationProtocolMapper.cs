using OSPSuite.Core.Services;
using OSPSuite.Utility;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;

namespace PKSim.Core.Batch.Mapper
{
   internal interface IApplicationProtocolMapper : IMapper<ApplicationProtocol, Protocol>
   {
   }

   internal class ApplicationProtocolMapper : IApplicationProtocolMapper
   {
      private readonly IProtocolFactory _protocolFactory;
      private readonly ILogger _logger;

      public ApplicationProtocolMapper(IProtocolFactory protocolFactory, ILogger logger)
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

         _logger.AddDebug($"Application Type = {applicationType.Name}");
         _logger.AddDebug($"Dosing Interval = {simpleProtocol.DosingInterval.DisplayName}");
         _logger.AddDebug($"Application Dose = {simpleProtocol.Dose.Value} [{simpleProtocol.Dose.DisplayUnit}]");
         _logger.AddDebug($"Application End Time = {simpleProtocol.EndTime}");

         return simpleProtocol;
      }
   }
}