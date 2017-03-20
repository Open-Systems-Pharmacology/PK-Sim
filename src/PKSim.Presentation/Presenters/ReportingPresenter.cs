using System.Collections.Generic;
using System.Linq;
using PKSim.Assets;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using OSPSuite.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Reporting;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Views;
using OSPSuite.Assets.Extensions;

namespace PKSim.Presentation.Presenters
{
   public class ReportingPresenter : AbstractReportingPresenter
   {
      private readonly IObjectTypeResolver _objectTypeResolver;

      public ReportingPresenter(IReportingView view, IReportTemplateRepository reportTemplateRepository,
                                IDialogCreator dialogCreator, IReportingTask reportingTask, IObjectTypeResolver objectTypeResolver, IStartOptions startOptions)
         : base(view, reportTemplateRepository, dialogCreator, reportingTask, startOptions)
      {
         _objectTypeResolver = objectTypeResolver;
      }

      protected override string RetrieveObjectTypeFrom(object objectToReport)
      {
         if (objectToReport.IsAnImplementationOf<IEnumerable<DataRepository>>())
            return PKSimConstants.UI.ListOf(PKSimConstants.UI.ObservedData);

         if (objectToReport.IsAnImplementationOf<DataRepository>())
            return PKSimConstants.UI.ObservedData;

         var objectBase = objectToReport as IObjectBase;
         if (objectBase != null)
            return _objectTypeResolver.TypeFor(objectBase);

         if (objectToReport.IsAnImplementationOf<IEnumerable<IPKSimBuildingBlock>>())
         {
            var col = objectToReport.DowncastTo<IEnumerable<IPKSimBuildingBlock>>().ToList();
            if (col.Any())
               return PKSimConstants.UI.ListOf(_objectTypeResolver.TypeFor(col.First()).Pluralize());
         }

         return string.Empty;
      }
   }
}