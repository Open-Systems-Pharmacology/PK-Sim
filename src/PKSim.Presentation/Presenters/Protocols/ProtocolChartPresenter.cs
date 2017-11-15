using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.DTO.Protocols;
using PKSim.Presentation.Views.Protocols;

namespace PKSim.Presentation.Presenters.Protocols
{
   public interface IProtocolChartPresenter : 
      IPresenter<IProtocolChartView>,
      ICanCopyToClipboard 
   {
      void PlotProtocol(Protocol protocol);
      void PlotProtocols(ICache<Compound, Protocol> protocols);
      string DescriptionFor(SchemaItemDTO schemaItemDTO);
      SchemaItemDTO SchemaItemDTOFrom(object tag);
   }

   public class ProtocolChartPresenter : AbstractPresenter<IProtocolChartView, IProtocolChartPresenter>, IProtocolChartPresenter
   {
      private readonly IProtocolToSchemaItemsMapper _schemaItemsMapper;
      private readonly ISchemaItemToSchemaItemDTOMapper _schemaItemDTOMapper;
      private readonly IDimensionRepository _dimensionRepository;
      private IProtocolChartData _protocolChartData;
      private readonly IApplicationSettings _applicationSettings;

      public ProtocolChartPresenter(
         IProtocolChartView view,
         IProtocolToSchemaItemsMapper schemaItemsMapper,
         ISchemaItemToSchemaItemDTOMapper schemaItemDTOMapper,
         IDimensionRepository dimensionRepository, 
         IApplicationSettings applicationSettings) : base(view)
      {
         _schemaItemsMapper = schemaItemsMapper;
         _schemaItemDTOMapper = schemaItemDTOMapper;
         _dimensionRepository = dimensionRepository;
         _applicationSettings = applicationSettings;
      }

      public void PlotProtocol(Protocol protocol)
      {
         if (protocol == null) return;
         //add a dummy compound when plotting a single protocol
         var cache = new Cache<Compound, Protocol> {{new Compound(), protocol}};
         PlotProtocols(cache);
      }

      public void PlotProtocols(ICache<Compound, Protocol> protocols)
      {
         if (!protocols.Any())
         {
            _view.Clear();
            return;
         }

         var longestProtocol = protocols.LongestProtocol();
         //a good value for the bar witdh is 10 kernel unit [min]
         var timeUnit = longestProtocol.TimeUnit;
         _view.BarWidth = _dimensionRepository.Time.BaseUnitValueToUnitValue(timeUnit, 10);
         _protocolChartData = dataToPlotFrom(protocols, timeUnit);
         _view.XAxisTitle = Constants.NameWithUnitFor(PKSimConstants.UI.Time, timeUnit);
         _view.YAxisTitle = Constants.NameWithUnitFor(PKSimConstants.UI.Dose, _protocolChartData.YAxisUnit);
         _view.Y2AxisTitle = Constants.NameWithUnitFor(PKSimConstants.UI.Dose, _protocolChartData.Y2AxisUnit);
         _view.Plot(_protocolChartData);
      }

      public string DescriptionFor(SchemaItemDTO schemaItemDTO)
      {
         if (schemaItemDTO == null)
            return string.Empty;

         return
            PKSimConstants.UI.SchemaItemDescription(
               schemaItemDTO.ApplicationType.DisplayName,
               schemaItemDTO.FormulationKey, ParameterMessages.DisplayValueFor(schemaItemDTO.DoseParameter.Parameter),
               ParameterMessages.DisplayValueFor(schemaItemDTO.StartTimeParameter.Parameter));
      }

      public SchemaItemDTO SchemaItemDTOFrom(object tag)
      {
         return _protocolChartData.SchemaItemFor(tag);
      }

      private IProtocolChartData dataToPlotFrom(ICache<Compound, Protocol> protocols, Unit timeUnit)
      {
         var endTime = protocols.Max(x => x.EndTime);
         var allSchemaItemsCache = new Cache<Compound, IReadOnlyList<SchemaItemDTO>>();
         protocols.KeyValues.Each(kv => allSchemaItemsCache[kv.Key] = schemaItemsDTOFrom(kv.Value));
         return new ProtocolChartData(allSchemaItemsCache, _dimensionRepository.Time, timeUnit, endTime);
      }

      private IReadOnlyList<SchemaItemDTO> schemaItemsDTOFrom(Protocol protocol)
      {
         return _schemaItemsMapper.MapFrom(protocol).MapAllUsing(_schemaItemDTOMapper);
      }

      public void CopyToClipboard()
      {
         View.CopyToClipboard(_applicationSettings.WatermarkTextToUse);
      }
   }
}