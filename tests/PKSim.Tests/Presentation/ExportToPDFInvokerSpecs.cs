using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Presentation.Core;
using PKSim.Presentation.UICommands;
using OSPSuite.Core.Chart;
using OSPSuite.Core.Events;
using OSPSuite.Presentation.Core;

namespace PKSim.Presentation
{
   internal abstract class concern_for_ExportToPDFInvoker : ContextSpecification<IExportToPDFInvoker>
   {
      protected ExportChartToPDFCommandForSpecs _exportChartToPDFCommand;

      protected override void Context()
      {
         _exportChartToPDFCommand = new ExportChartToPDFCommandForSpecs();
         sut = new ExportToPDFInvoker(_exportChartToPDFCommand);
      }
   }

   internal class When_notify_than_a_chart_needs_to_be_reported : concern_for_ExportToPDFInvoker
   {
      private ICurveChart _chart;

      protected override void Context()
      {
         base.Context();
         _chart = A.Fake<ICurveChart>();
      }

      protected override void Because()
      {
         sut.Handle(new ExportToPDFEvent(_chart));
      }

      [Observation]
      public void the_export_to_pdf_invoker_should_export_the_object()
      {
         _exportChartToPDFCommand.Executed.ShouldBeTrue();
      }
   }

   internal class ExportChartToPDFCommandForSpecs : ExportChartToPDFCommand
   {
      public ExportChartToPDFCommandForSpecs() : base(A.Fake<IApplicationController>())
      {
      }

      protected override void PerformExecute()
      {
         Executed = true;
      }

      public bool Executed { get; set; }
   }
}	