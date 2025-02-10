using System.Collections.Generic;
using PKSim.Assets;
using OSPSuite.Presentation.MenuAndBars;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Presentation.Presenters.PopulationAnalyses;
using OSPSuite.Assets;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Presenters.ContextMenus;
using OSPSuite.Utility.Container;

namespace PKSim.Presentation.Presenters.ContextMenus
{
   public class PopulationAnalysisDerivedFieldContextMenu : ContextMenu<PopulationAnalysisDerivedField, IPopulationAnalysisFieldsPresenter>
   {
      public PopulationAnalysisDerivedFieldContextMenu(PopulationAnalysisDerivedField populationAnalysisDerivedField, IPopulationAnalysisFieldsPresenter presenter, IContainer container)
         : base(populationAnalysisDerivedField, presenter, container)
      {
      }

      protected override IEnumerable<IMenuBarItem> AllMenuItemsFor(PopulationAnalysisDerivedField populationAnalysisDerivedField, IPopulationAnalysisFieldsPresenter presenter)
      {
         yield return CreateMenuButton.WithCaption(MenuNames.Edit)
            .WithActionCommand(() => presenter.EditDerivedField(populationAnalysisDerivedField))
            .WithIcon(ApplicationIcons.Edit);


         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.SaveDerivedFieldToTemplate)
            .WithActionCommand(() => presenter.SaveDerivedFieldToTemplate(populationAnalysisDerivedField))
            .WithIcon(ApplicationIcons.SaveAsTemplate);

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.Delete)
            .WithActionCommand(() => presenter.RemoveField(populationAnalysisDerivedField))
            .WithIcon(ApplicationIcons.Delete)
            .AsGroupStarter();
      }
   }

   public class PopulationAnalysisDerivedFieldContextMenuFactory : IContextMenuSpecificationFactory<IPopulationAnalysisField>
   {
      private readonly IContainer _container;

      public PopulationAnalysisDerivedFieldContextMenuFactory(IContainer container)
      {
         _container = container;
      }

      public IContextMenu CreateFor(IPopulationAnalysisField populationAnalysisField, IPresenterWithContextMenu<IPopulationAnalysisField> presenter)
      {
         return new PopulationAnalysisDerivedFieldContextMenu(populationAnalysisField.DowncastTo<PopulationAnalysisDerivedField>(),
            presenter.DowncastTo<IPopulationAnalysisFieldsPresenter>(), _container);
      }

      public bool IsSatisfiedBy(IPopulationAnalysisField populationAnalysisField, IPresenterWithContextMenu<IPopulationAnalysisField> presenter)
      {
         return populationAnalysisField.IsAnImplementationOf<PopulationAnalysisDerivedField>() &&
                presenter.IsAnImplementationOf<IPopulationAnalysisFieldsPresenter>();
      }
   }
}