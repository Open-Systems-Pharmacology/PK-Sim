using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OSPSuite.Assets;
using OSPSuite.Core;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Presenters.ContextMenus;
using OSPSuite.Utility;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.DTO;
using PKSim.Presentation.Views;

namespace PKSim.Presentation.Presenters
{
   public interface ITemplatePresenter : IDisposablePresenter
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

   public class TemplatePresenter : AbstractDisposablePresenter<ITemplateView, ITemplatePresenter>, ITemplatePresenter, ILatchable
   {
      private readonly ITemplateTaskQuery _templateTaskQuery;
      private readonly ITreeNodeContextMenuFactory _contextMenuFactory;
      private readonly IApplicationController _applicationController;
      private readonly IDialogCreator _dialogCreator;
      private List<Template> _availableTemplates;
      private bool _shouldAddItemIcons;
      private readonly IStartOptions _startOptions;
      private readonly IApplicationConfiguration _configuration;
      private string _templateTypeDisplay = string.Empty;
      private readonly List<Template> _selectedTemplates = new List<Template>();

      public TemplatePresenter(
         ITemplateView view,
         ITemplateTaskQuery templateTaskQuery,
         ITreeNodeContextMenuFactory contextMenuFactory,
         IApplicationController applicationController,
         IDialogCreator dialogCreator,
         IStartOptions startOptions,
         IApplicationConfiguration configuration)
         : base(view)
      {
         _templateTaskQuery = templateTaskQuery;
         _contextMenuFactory = contextMenuFactory;
         _applicationController = applicationController;
         _dialogCreator = dialogCreator;
         _startOptions = startOptions;
         _configuration = configuration;
      }

      public Task<IReadOnlyList<T>> LoadFromTemplateAsync<T>(TemplateType templateType)
      {
         _templateTypeDisplay = templateType.ToString();
         _view.Caption = PKSimConstants.UI.LoadItemFromTemplate(_templateTypeDisplay);
         _shouldAddItemIcons = !_templateTaskQuery.IsPrimitiveType(templateType);

         updateIcon(templateType);

         _availableTemplates = _templateTaskQuery.AllTemplatesFor(templateType)
            .Where(x => x.IsSupportedByCurrentVersion(_configuration.Version))
            .OrderBy(x => x.Name)
            .ToList();

         if (!_availableTemplates.Any())
            throw new NoTemplateAvailableException(_templateTypeDisplay);

         updateView();
         _view.Display();

         if (_view.Canceled)
            return Task.FromResult<IReadOnlyList<T>>(Array.Empty<T>());

         return shouldLoadTemplateWithReferences(templateType) ? loadMultipleTemplate<T>() : loadSingleTemplate<T>();
      }

      private bool shouldLoadTemplateWithReferences(TemplateType templateType)
      {
         if (!selectionSupportsReference(templateType))
            return false;

         var message = getMessageForLoadWithReference(templateType);
         return _dialogCreator.MessageBoxYesNo(message) == ViewResult.Yes;
      }

      private string getMessageForLoadWithReference(TemplateType templateType)
      {
         switch (templateType)
         {
            case TemplateType.Compound:
               return PKSimConstants.UI.DoYouWantToLoadMetabolites(_selectedTemplates.Count);
            default:
               return PKSimConstants.UI.DoYouWantToLoadExpressionProfiles(_selectedTemplates.Count);
         }
      }

      private bool selectionSupportsReference(TemplateType templateType)
      {
         //Type does not even supports reference. Nothing to load
         if (!templateType.SupportsReference())
            return false;

         //At least one remote and reference are supported, so we ask the user as we have no way to know before loading the whole template
         if (_selectedTemplates.Any(x => x.DatabaseType == TemplateDatabaseType.Remote))
            return true;

         //only local or system. We can check for references 
         return _selectedTemplates.OfType<LocalTemplate>().Any(x => x.HasReferences);
      }

      private async Task<IReadOnlyList<T>> loadMultipleTemplate<T>()
      {
         var allTemplates = new Cache<string, T>();
         foreach (var template in _selectedTemplates)
         {
            await loadTemplateWithReferences<T>(allTemplates, template);
         }

         return allTemplates.ToList();
      }

      private async Task loadTemplateWithReferences<T>(Cache<string, T> allTemplates, Template template)
      {
         if (allTemplates.Contains(template.Name))
            return;

         var loadedTemplate = await loadTemplateAsync<T>(template);

         //This can be the case for instance if a reference is not found.
         //It can happen easily with remote templates
         if (loadedTemplate == null)
            return;

         allTemplates.Add(template.Name, loadedTemplate);
         var references = _templateTaskQuery.AllReferenceTemplatesFor(template, loadedTemplate);
         foreach (var reference in references)
         {
            await loadTemplateWithReferences<T>(allTemplates, reference);
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
         _view.Description = PKSimConstants.UI.NumberOfTemplatesSelectedIs(numberOfTemplateSelected, _templateTypeDisplay);
      }

      private void updateView()
      {
         refreshView();
         var allTemplateDTOs = _availableTemplates.Select(x => new TemplateDTO(x)).ToList();
         this.DoWithinLatch(() => _view.BindTo(allTemplateDTOs));
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
         if (IsLatched) return;
         _selectedTemplates.Clear();
         _selectedTemplates.AddRange(templateDTOs.Select(x => x.Template));
         refreshView();
      }

      public bool IsLatched { get; set; }
   }
}