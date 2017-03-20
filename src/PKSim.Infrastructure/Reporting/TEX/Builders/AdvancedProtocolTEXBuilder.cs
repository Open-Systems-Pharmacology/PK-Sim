using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Infrastructure.Reporting;
using OSPSuite.TeXReporting.Builder;
using OSPSuite.TeXReporting.Items;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Reporting;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Infrastructure.Reporting.Extensions;

namespace PKSim.Infrastructure.Reporting.TeX.Builders
{
   public class AdvancedProtocolTeXBuilder : BuildingBlockTeXBuilder<AdvancedProtocol>
   {
      private readonly IRepresentationInfoRepository _representationInfoRepository;

      public AdvancedProtocolTeXBuilder(ITeXBuilderRepository builderRepository, IReportGenerator reportGenerator, ILazyLoadTask lazyLoadTask, IRepresentationInfoRepository representationInfoRepository)
         : base(builderRepository, reportGenerator, lazyLoadTask)
      {
         _representationInfoRepository = representationInfoRepository;
      }

      protected override IEnumerable<object> BuildingBlockReport(AdvancedProtocol advancedProtocol, OSPSuiteTracker tracker)
      {
         var objectToReports = new List<object>();
         foreach (var schema in advancedProtocol.AllSchemas)
         {
            var schemaValues = new List<Text>
               {
                  displayFor(schema.StartTime),
                  displayFor(PKSimConstants.UI.NumberOfRepetitions, schema.NumberOfRepetitions),
                  displayFor(PKSimConstants.UI.TimeBetweenRepetitions, schema.TimeBetweenRepetitions),
               };

            foreach (var schemaItem in schema.SchemaItems)
            {
               var schemaItemValues = new List<Text>
                  {
                     displayFor(schemaItem.StartTime),
                     displayFor(schemaItem.Dose),
                     displayFor(PKSimConstants.UI.ApplicationType, schemaItem.ApplicationType.ToString()),
                  };
               if (schemaItem.ApplicationType.NeedsFormulation)
                  schemaItemValues.Add(displayFor(PKSimConstants.UI.PlaceholderFormulation, schemaItem.FormulationKey));

               var allParameters = schemaItem.AllParametersToExport().ToList();
               if (allParameters.Any())
               {
                  schemaItemValues.Add(new Text("{0}{1}", new Text(PKSimConstants.UI.Parameters) {FontStyle = Text.FontStyles.bold}, new List(allParameters.Select(displayFor))));
               }
               schemaValues.Add(new Text("{0}{1}", new Text(PKSimConstants.UI.Dosing) { FontStyle = Text.FontStyles.bold }, new List(schemaItemValues)));
            }
            objectToReports.Add(new Par());
            objectToReports.Add(new Text("{0}", schema.Name) {FontStyle = Text.FontStyles.bold});
            objectToReports.Add(new List(schemaValues));
         }

         return objectToReports;
      }

      private Text displayFor(IParameter parameter)
      {
         return displayFor(_representationInfoRepository.DisplayNameFor(parameter), ParameterMessages.DisplayValueFor(parameter));
      }

      private Text displayFor(string caption, IParameter parameter)
      {
         return displayFor(caption, ParameterMessages.DisplayValueFor(parameter));
      }

      private Text displayFor(string caption, string display)
      {
         return new Text(PKSimConstants.UI.ReportIs(caption, display));
      }
   }
}