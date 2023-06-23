using System.Collections.Generic;
using PKSim.Assets;
using OSPSuite.Presentation.MenuAndBars;
using OSPSuite.Utility.Extensions;
using PKSim.Presentation.Presenters.Parameters;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.DTO;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Presenters.ContextMenus;
using OSPSuite.Assets;
using OSPSuite.Utility.Container;

namespace PKSim.Presentation.Presenters.ContextMenus
{
   public class ParameterEditContextMenu : ContextMenu<IParameterDTO, IParameterSetPresenter>
   {
      public ParameterEditContextMenu(IParameterDTO parameterDTO, IParameterSetPresenter presenter, IContainer container)
         : base(parameterDTO, presenter, container)
      {
      }

      protected override IEnumerable<IMenuBarItem> AllMenuItemsFor(IParameterDTO parameterDTO, IParameterSetPresenter presenter)
      {
         if (!presenter.IsSetByUser(parameterDTO))
            yield break;

         yield return CreateMenuButton.WithCaption(PKSimConstants.MenuNames.Reset)
            .WithActionCommand(() => presenter.ResetParameter(parameterDTO))
            .WithIcon(ApplicationIcons.Reset);
      }
   }

   public class ParameterEditContextMenuFactory : IContextMenuSpecificationFactory<IParameterDTO>
   {
      private readonly IContainer _container;

      public ParameterEditContextMenuFactory(IContainer container)
      {
         _container = container;
      }

      public IContextMenu CreateFor(IParameterDTO parameterDTO, IPresenterWithContextMenu<IParameterDTO> presenter)
      {
         return new ParameterEditContextMenu(parameterDTO, presenter.DowncastTo<IParameterSetPresenter>(), _container);
      }

      public bool IsSatisfiedBy(IParameterDTO parameterDTO, IPresenterWithContextMenu<IParameterDTO> presenter)
      {
         return parameterDTO.Parameter != null && presenter.IsAnImplementationOf<IParameterSetPresenter>();
      }
   }
}