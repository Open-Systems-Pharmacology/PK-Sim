﻿using System.Collections.Generic;
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
   public class PopulationAnalysisDataFieldContextMenu : ContextMenu<PopulationAnalysisDataField, IPopulationAnalysisFieldsPresenter>
   {
      public PopulationAnalysisDataFieldContextMenu(PopulationAnalysisDataField populationAnalysisDataField, IPopulationAnalysisFieldsPresenter presenter, IContainer container)
         : base(populationAnalysisDataField, presenter, container)
      {
      }

      protected override IEnumerable<IMenuBarItem> AllMenuItemsFor(PopulationAnalysisDataField populationAnalysisDataField, IPopulationAnalysisFieldsPresenter presenter)
      {
         if (populationAnalysisDataField.DerivedFieldAllowed)
         {
            yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.CreateDerivedField)
               .WithActionCommand(() => presenter.CreateDerivedFieldFor(populationAnalysisDataField))
               .WithIcon(ApplicationIcons.Create);

            //TODO action command async
            yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.LoadDerivedFieldFromTemplate)
               .WithActionCommand(() => presenter.LoadDerivedFieldFromTemplateForAsync(populationAnalysisDataField))
               .WithIcon(ApplicationIcons.LoadFromTemplate);
         }

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.Delete)
            .WithActionCommand(() => presenter.RemoveField(populationAnalysisDataField))
            .WithIcon(ApplicationIcons.Delete)
            .AsGroupStarter();
      }
   }

   public class PopulationAnalysisDataFieldContextMenuFactory : IContextMenuSpecificationFactory<IPopulationAnalysisField>
   {
      private readonly IContainer _container;

      public PopulationAnalysisDataFieldContextMenuFactory(IContainer container)
      {
         _container = container;
      }

      public IContextMenu CreateFor(IPopulationAnalysisField populationAnalysisField, IPresenterWithContextMenu<IPopulationAnalysisField> presenter)
      {
         return new PopulationAnalysisDataFieldContextMenu(populationAnalysisField.DowncastTo<PopulationAnalysisDataField>(), 
            presenter.DowncastTo<IPopulationAnalysisFieldsPresenter>(), _container);
      }

      public bool IsSatisfiedBy(IPopulationAnalysisField populationAnalysisField, IPresenterWithContextMenu<IPopulationAnalysisField> presenter)
      {
         return populationAnalysisField.IsAnImplementationOf<PopulationAnalysisDataField>() &&
                presenter.IsAnImplementationOf<IPopulationAnalysisFieldsPresenter>();
      }
   }
}