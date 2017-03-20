using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Infrastructure.Reporting;
using OSPSuite.TeXReporting.Builder;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Reporting;
using PKSim.Core.Services;
using PKSim.Infrastructure.Reporting.Extensions;

namespace PKSim.Infrastructure.Reporting.TeX.Builders
{
   public class SimpleProtocolTeXBuilder : BuildingBlockTeXBuilder<SimpleProtocol>
   {
      public SimpleProtocolTeXBuilder(ITeXBuilderRepository builderRepository, IReportGenerator reportGenerator, ILazyLoadTask lazyLoadTask) : base(builderRepository, reportGenerator, lazyLoadTask)
      {
      }

      protected override IEnumerable<object> BuildingBlockReport(SimpleProtocol simpleProtocol, OSPSuiteTracker tracker)
      {
         var table = new TablePart(PKSimConstants.UI.Property, PKSimConstants.UI.Value);
         table.AddIs(PKSimConstants.UI.ProcessType, PKSimConstants.UI.SimpleProtocolMode);
         table.AddIs(PKSimConstants.UI.ApplicationType, simpleProtocol.ApplicationType.DisplayName);
         table.AddIs(PKSimConstants.UI.DosingInterval, simpleProtocol.DosingInterval.DisplayName);
         var parameters = new ParameterList(PKSimConstants.UI.Parameters, simpleProtocol.AllParameters().Where(simpleProtocol.ParameterShouldBeExported));
         return new object[] {table, parameters};
      }
   }
}