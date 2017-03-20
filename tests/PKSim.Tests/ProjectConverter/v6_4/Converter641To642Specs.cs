using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Infrastructure.ProjectConverter.v6_4;
using PKSim.IntegrationTests;
using PKSim.Presentation;
using OSPSuite.Core.Domain;

namespace PKSim.ProjectConverter.v6_4
{
   public class When_converting_the_layout_63_project : ContextWithLoadedProject<Converter641To642>
   {
      public override void GlobalContext()
      {
         base.GlobalContext();
         LoadProject("Layout_631");
      }

      [Observation]
      public void should_be_able_to_load_the_project()
      {
         _project.ShouldNotBeNull();
      }

      [Observation]
      public void should_have_converted_all_layout_items()
      {
         var allLayoutItems = _workspace.WorkspaceLayout.LayoutItems.ToList();
         allLayoutItems.Any().ShouldBeTrue();
         allLayoutItems.Each(x => x.PresentationSettings.ShouldNotBeNull());
      }

      [Observation]
      public void should_have_layout_saved_for_individual_pk_analysis()
      {
         _workspace.WorkspaceLayout.LayoutItems.Any(x => x.PresentationKey == PresenterConstants.PresenterKeys.IndividualPKParametersPresenter).ShouldBeTrue();
      }

      [Observation]
      public void should_have_converted_the_chart_templates_in_the_project()
      {
         var template = FindByName<Simulation>("S1").SimulationSettings.ChartTemplates.FindByName("Test");
         foreach (var curveTemplate in template.Curves)
         {
            curveTemplate.xData.Path.ShouldNotBeNull();
            curveTemplate.yData.Path.ShouldNotBeNull();
         }
      }
   }
}