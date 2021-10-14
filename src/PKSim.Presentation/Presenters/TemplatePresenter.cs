using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using OSPSuite.Assets;
using OSPSuite.Core;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Nodes;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Presenters.ContextMenus;
using OSPSuite.Utility.Collections;
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
      Task<IReadOnlyList<T>> LoadFromTemplateAsync<T>(TemplateType templateType);

      /// <summary>
      ///    Rename the building block template given as parameter
      /// </summary>
      /// <param name="template">Building block template to rename</param>
      void Rename(TemplateDTO template);

      /// <summary>
      ///    Delete the building block template given as parameter
      /// </summary>
      /// <param name="templateDTO">Building block template to delete</param>
      void Delete(TemplateDTO templateDTO);

      /// <summary>
      ///    Returns true if the user can edit the given <paramref name="template" /> otherwise false
      /// </summary>
      bool CanEdit(TemplateDTO template);

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

      public Task<IReadOnlyList<T>> LoadFromTemplateAsync<T>(TemplateType templateType)
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
            return Task.FromResult<IReadOnlyList<T>>(Array.Empty<T>());

         return shouldLoadTemplateWithReferences() ? loadMultipleTemplate<T>() : loadSingleTemplate<T>();
      }

      private bool shouldLoadTemplateWithReferences()
      {
         if (!_selectedTemplates.Any(x => x.HasReferences))
            return false;

         return _dialogCreator.MessageBoxYesNo(PKSimConstants.UI.DoYouWantToLoadReferencedTemplateAsWell(_selectedTemplates.Count)) == ViewResult.Yes;
      }

      private async Task<IReadOnlyList<T>> loadMultipleTemplate<T>()
      {
         var allTemplates = new Cache<string, T>();
         foreach (var template in _selectedTemplates)
         {
            await loadTemplateWithReferences(allTemplates, template);
         }

         return allTemplates.ToList();
      }

      private async Task loadTemplateWithReferences<T>(Cache<string, T> allTemplates, Template template)
      {
         if (allTemplates.Contains(template.Name))
            return;

         
         allTemplates.Add(template.Name, await loadTemplateAsync<T>(template));
         foreach (var reference in template.References)
         {
            await loadTemplateWithReferences(allTemplates, reference);
         }
      }

      private async Task<IReadOnlyList<T>> loadSingleTemplate<T>()
      {
         var allTemplates = new List<T>();
         foreach (var template in _selectedTemplates)
         {
            allTemplates.Add(await loadTemplateAsync<T>(template));
         }

         return allTemplates;
      } 

      private Task<T> loadTemplateAsync<T>(Template template) => _templateTaskQuery.LoadTemplateAsync<T>(template);

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

      public void Rename(TemplateDTO templateDTO)
      {
         var template = templateDTO.Template;
         using (var renamePresenter = _applicationController.Start<IRenameTemplatePresenter>())
         {
            if (!renamePresenter.Edit(template))
               return;

            _templateTaskQuery.RenameTemplate(template, renamePresenter.Name);
         }
      }

      public void Delete(TemplateDTO templateDTO)
      {
         var result = _dialogCreator.MessageBoxYesNo(PKSimConstants.UI.ReallyDeleteTemplate(templateDTO.Name));
         if (result == ViewResult.No)
            return;

         var template = templateDTO.Template;
         _templateTaskQuery.DeleteTemplate(template);
         _availableTemplates.Remove(template);
         _selectedTemplates.Remove(template);

         updateView();
      }

      public bool CanEdit(TemplateDTO template)
      {
         var databaseType = template.DatabaseType;
         
         //Remote templates cannot be edited, even by dev
         if (databaseType == TemplateDatabaseType.Remote)
            return false;

         return _startOptions.IsDeveloperMode || databaseType == TemplateDatabaseType.User;
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