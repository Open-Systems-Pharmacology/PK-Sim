using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Infrastructure.Reporting;
using OSPSuite.TeXReporting.Builder;

namespace PKSim.Infrastructure.Reporting.TeX.Builders
{
   public class ModelTeXBuilder : OSPSuiteTeXBuilder<IModel>
   {
      private readonly ITeXBuilderRepository _builderRepository;
      private readonly IModelReportCreator _modelReportCreator;

      public ModelTeXBuilder(ITeXBuilderRepository builderRepository, IModelReportCreator modelReportCreator)
      {
         _builderRepository = builderRepository;
         _modelReportCreator = modelReportCreator;
      }

      public override void Build(IModel model, OSPSuiteTracker buildTracker)
      {
         var verbose = buildTracker.Settings.Verbose;
         _builderRepository.Report(_modelReportCreator.ModelReport(model, reportFormulaReferences: true, reportFormulaObjectPaths: verbose, reportDescriptions: verbose), buildTracker);
      }
   }
}