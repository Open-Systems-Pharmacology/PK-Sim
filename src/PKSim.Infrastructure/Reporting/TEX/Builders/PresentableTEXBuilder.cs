using System.Collections.Generic;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Services;
using OSPSuite.Infrastructure.Reporting;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Services;
using OSPSuite.Utility.Extensions;

namespace PKSim.Infrastructure.Reporting.TeX.Builders
{
   public abstract class PresentableTeXBuilder<TReportedObject, TPresentationSettings> : OSPSuiteTeXBuilder<TReportedObject> where TPresentationSettings : class, IPresentationSettings, new()
   {
      private readonly IPresentationSettingsTask _presentationSettingsTask;
      private readonly IDisplayUnitRetriever _displayUnitRetriever;

      protected PresentableTeXBuilder(IPresentationSettingsTask presentationSettingsTask, IDisplayUnitRetriever displayUnitRetriever)
      {
         _presentationSettingsTask = presentationSettingsTask;
         _displayUnitRetriever = displayUnitRetriever;
      }

      protected TPresentationSettings PresentationSettingsFor(IWithId withId, string presentationKey)
      {
         return _presentationSettingsTask.PresentationSettingsFor<TPresentationSettings>(presentationKey, withId);
      }

      protected void UpdateParameterDisplayUnit(IEnumerable<IQuantity> quantities, IPresentationSettings presentationSettings)
      {
         quantities.Each(p =>
         {
            var preferredUnit = _displayUnitRetriever.PreferredUnitFor(p);
            var unitName = presentationSettings.GetSetting(p.Name, preferredUnit.Name);
            p.DisplayUnit = p.Dimension.UnitOrDefault(unitName);
         });
      }
   }
}