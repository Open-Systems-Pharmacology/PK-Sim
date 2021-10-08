using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OSPSuite.Assets;
using OSPSuite.Core;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Nodes;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Presenters.ContextMenus;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.DTO;
using PKSim.Presentation.Views;

namespace PKSim.Presentation.Presenters
{
   public interface ITemplatePresenter : IDisposablePresenter, IPresenterWithContextMenu<ITreeNode>
   {
      /// <summary>
      ///    Allows the user to select a template to be loaded from the template database for the given
      ///    <paramref name="templateType" />
      /// </summary>
      /// <param name="templateType">Type of object that should be loaded</param>
      /// <returns>The loaded template if the user completed the action successfully otherwise null</returns>
      IReadOnlyList<T> LoadFromTemplate<T>(TemplateType templateType);

      /// <summary>
      ///    Rename the building block template given as parameter
      /// </summary>
      /// <param name="template">Building block template to rename</param>
      void Rename(Template template);

      /// <summary>
      ///    Delete the building block template given as parameter
      /// </summary>
      /// <param name="template">Building block template to delete</param>
      void Delete(Template template);

      /// <summary>
      ///    Returns true if the user can edit the given <paramref name="template" /> otherwise false
      /// </summary>
      bool CanEdit(Template template);

      void SelectedTemplatesChanged(IReadOnlyList<TemplateDTO> templates);
   }

   public class TemplatePresenter : AbstractDisposablePresenter<ITemplateView, ITemplatePresenter>, ITemplatePresenter
   {
      private readonly ITemplateTaskQuery _templateTaskQuery;
      private readonly IObjectTypeResolver _objectTypeResolver;
      private readonly ITreeNodeContextMenuFactory _contextMenuFactory;
      private readonly IApplicationController _applicationController;
      private readonly IDialogCreator _dialogCreator;
      private List<Template> _availableTemplates;
      private bool _shouldAddItemIcons;
      private readonly IStartOptions _startOptions;
      private string _buildingBlockTypeString = string.Empty;
      private readonly List<Template> _selectedTemplates = new List<Template>();

      public TemplatePresenter(
         ITemplateView view,
         ITemplateTaskQuery templateTaskQuery,
         IObjectTypeResolver objectTypeResolver,
         ITreeNodeContextMenuFactory contextMenuFactory,
         IApplicationController applicationController,
         IDialogCreator dialogCreator,
         IStartOptions startOptions)
         : base(view)
      {
         _templateTaskQuery = templateTaskQuery;
         _objectTypeResolver = objectTypeResolver;
         _contextMenuFactory = contextMenuFactory;
         _applicationController = applicationController;
         _dialogCreator = dialogCreator;
         _startOptions = startOptions;
      }

      public IReadOnlyList<T> LoadFromTemplate<T>(TemplateType templateType)
      {
         _buildingBlockTypeString = _objectTypeResolver.TypeFor<T>();
         _view.Caption = PKSimConstants.UI.LoadBuildingBlockFromTemplate(_buildingBlockTypeString);
         _shouldAddItemIcons = !_templateTaskQuery.IsPrimitiveType(templateType);

         updateIcon(templateType);

         _availableTemplates = _templateTaskQuery.AllTemplatesFor(templateType).OrderBy(x => x.Name).ToList();
         if (!_availableTemplates.Any())
            throw new NoBuildingBlockTemplateAvailableException(_buildingBlockTypeString);

         updateView();
         _view.Display();

         if (_view.Canceled)
            return new List<T>();

         return shouldLoadTemplateWithReferences() ? loadMultipleTemplate<T>() : loadSingleTemplate<T>();
      }

      private bool shouldLoadTemplateWithReferences()
      {
         if (!_selectedTemplates.Any(x => x.HasReferences))
            return false;

         return _dialogCreator.MessageBoxYesNo(PKSimConstants.UI.DoYouWantToLoadReferencedTemplateAsWell(_selectedTemplates.Count)) == ViewResult.Yes;
      }

      private IReadOnlyList<T> loadMultipleTemplate<T>()
      {
         var allTemplates = new Cache<string, T>();
         _selectedTemplates.Each(template => loadTemplateWithReferences(allTemplates, template));
         return allTemplates.ToList();
      }

      private void loadTemplateWithReferences<T>(Cache<string, T> allTemplates, Template template)
      {
         if (allTemplates.Contains(template.Name))
            return;

         allTemplates.Add(template.Name, loadTemplate<T>(template));
         foreach (var reference in template.References)
         {
            loadTemplateWithReferences(allTemplates, reference);
         }
      }

      private IReadOnlyList<T> loadSingleTemplate<T>() => _selectedTemplates.Select(loadTemplate<T>).ToList();

      private T loadTemplate<T>(Template template) => _templateTaskQuery.LoadTemplate<T>(template);

      private void updateIcon(TemplateType templateType)
      {
         if (!_shouldAddItemIcons) return;

         var icon = ApplicationIcons.IconByName(templateType.ToString());
         if (icon != ApplicationIcons.EmptyIcon)
            _view.SetIcon(icon);
      }

      private void refreshView()
      {
         var numberOfTemplateSelected = _selectedTemplates.Count;
         _view.OkEnabled = numberOfTemplateSelected > 0;

         // _view.Description =
         //    numberOfTemplateSelected == 0 ? string.Empty :
         //    numberOfTemplateSelected == 1 ? _selectedTemplates[0].Description :
         //    PKSimConstants.UI.NumberOfTemplatesSelectedIs(numberOfTemplateSelected, _buildingBlockTypeString);
      }

      private void updateView()
      {
         refreshView();
         var allTemplateDTOs = _availableTemplates.Select(x => new TemplateDTO(x)).ToList();
         _view.BindTo(allTemplateDTOs);
         _view.SelectTemplate(allTemplateDTOs.FirstOrDefault());
      }

      public IEnumerable<Template> AllTemplates() => _availableTemplates;

      public void Rename(Template template)
      {
         using (var renamePresenter = _applicationController.Start<IRenameTemplatePresenter>())
         {
            if (!renamePresenter.Edit(template))
               return;

            _templateTaskQuery.RenameTemplate(template, renamePresenter.Name);
         }
      }

      public void Delete(Template template)
      {
         var result = _dialogCreator.MessageBoxYesNo(PKSimConstants.UI.ReallyDeleteTemplate(template.Name));
         if (result == ViewResult.No)
            return;

         _templateTaskQuery.DeleteTemplate(template);
         _availableTemplates.Remove(template);
         _selectedTemplates.Remove(template);

         updateView();
      }

      public bool CanEdit(Template template)
      {
         return _startOptions.IsDeveloperMode || template.DatabaseType == TemplateDatabaseType.User;
      }

      public void SelectedTemplatesChanged(IReadOnlyList<TemplateDTO> templateDTOs)
      {
         _selectedTemplates.Clear();
         _selectedTemplates.AddRange(templateDTOs.Select(x => x.Template));
         refreshView();
      }

      public void ShowContextMenu(ITreeNode nodeRequestingPopup, Point popupLocation)
      {
         var contextMenu = _contextMenuFactory.CreateFor(nodeRequestingPopup, this);
         contextMenu.Show(_view, popupLocation);
      }
   }
}