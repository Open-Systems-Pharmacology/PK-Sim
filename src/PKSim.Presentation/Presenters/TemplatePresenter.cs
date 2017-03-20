using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using PKSim.Assets;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Nodes;
using OSPSuite.Utility.Collections;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Nodes;
using PKSim.Presentation.Views;
using OSPSuite.Core;
using OSPSuite.Assets;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Presenters.ContextMenus;
using OSPSuite.Presentation.Views;
using ITreeNodeFactory = PKSim.Presentation.Nodes.ITreeNodeFactory;

namespace PKSim.Presentation.Presenters
{
   public interface ITemplatePresenter : IDisposablePresenter, IPresenterWithContextMenu<ITreeNode>
   {
      /// <summary>
      ///    Allows the user to select a template to be loaded from the template database for the given
      ///    <paramref name="templateType" />
      /// </summary>
      /// <param name="templateType">Type of object that should be loaded</param>
      /// <returns>The loaded template if the user completed the action successfuly otherwise null</returns>
      IReadOnlyList<T> LoadFromTemplate<T>(TemplateType templateType);

      /// <summary>
      ///    Returns all available building block meta data for the given type
      /// </summary>
      IEnumerable<Template> AllTemplates();

      /// <summary>
      ///    This method is called whenever a node is being selected in the view
      /// </summary>
      void ActivateNode(ITreeNode node);

      /// <summary>
      ///    Rename the building block template given as parameter
      /// </summary>
      /// <param name="template">Building block template to rename</param>
      void Rename(Template template);

      /// <summary>
      ///    Delete the building block template guven as parameter
      /// </summary>
      /// <param name="template">Building block template to delete</param>
      void Delete(Template template);

      /// <summary>
      ///    Returns true if the user can edit the given <paramref name="template" /> otherwise false
      /// </summary>
      bool CanEdit(Template template);
   }

   public class TemplatePresenter : AbstractDisposablePresenter<IBuildingBlockFromTemplateView, ITemplatePresenter>, ITemplatePresenter
   {
      private readonly ITemplateTaskQuery _templateTaskQuery;
      private readonly IObjectTypeResolver _objectTypeResolver;
      private readonly ITreeNodeFactory _treeNodeFactory;
      private readonly ITreeNodeContextMenuFactory _contextMenuFactory;
      private readonly IApplicationController _applicationController;
      private readonly IDialogCreator _dialogCreator;
      private IEnumerable<Template> _availableBuildingBlocks;
      private Template _selectedTemplate;
      private bool _shouldAddItemIcons;
      private readonly IStartOptions _startOptions;

      public TemplatePresenter(IBuildingBlockFromTemplateView view, ITemplateTaskQuery templateTaskQuery,
         IObjectTypeResolver objectTypeResolver, ITreeNodeFactory treeNodeFactory,
         ITreeNodeContextMenuFactory contextMenuFactory, IApplicationController applicationController, IDialogCreator dialogCreator, IStartOptions startOptions)
         : base(view)
      {
         _templateTaskQuery = templateTaskQuery;
         _objectTypeResolver = objectTypeResolver;
         _treeNodeFactory = treeNodeFactory;
         _contextMenuFactory = contextMenuFactory;
         _applicationController = applicationController;
         _dialogCreator = dialogCreator;
         _startOptions = startOptions;
      }

      public IReadOnlyList<T> LoadFromTemplate<T>(TemplateType templateType)
      {
         var allTemplates = new List<T>();
         string buildingBlockTypeString = _objectTypeResolver.TypeFor<T>();
         _view.Caption = PKSimConstants.UI.LoadBuildingBlockFromTemplate(buildingBlockTypeString);
         _shouldAddItemIcons = !_templateTaskQuery.IsPrimitiveType(templateType);

         updateIcon(templateType);

         _availableBuildingBlocks = _templateTaskQuery.AllTemplatesFor(templateType);
         if (!_availableBuildingBlocks.Any())
            throw new NoBuildingBlockTemplateAvailableException(buildingBlockTypeString);

         var userTemplateNode = _treeNodeFactory.CreateFor(PKSimRootNodeTypes.UserTemplates);
         var systemTemplateNode = _treeNodeFactory.CreateFor(PKSimRootNodeTypes.SystemTemplates);
         addTemplatesTo(userTemplateNode, TemplateDatabaseType.User);
         addTemplatesTo(systemTemplateNode, TemplateDatabaseType.System);

         updateView();
         _view.Display();

         if (_view.Canceled)
            return allTemplates;

         if (_selectedTemplate.HasReferences)
         {
            if (_dialogCreator.MessageBoxYesNo(PKSimConstants.UI.DoYouWantToLoadReferencedTemplateAsWell) == ViewResult.Yes)
               return loadMultipleTemplate<T>();
         }

         allTemplates.Add(loadSingleTemplate<T>());
         return allTemplates;
      }

      private IReadOnlyList<T> loadMultipleTemplate<T>()
      {
         var allTemplates = new Cache<string, T>();
         loadTemplateWithReferences(allTemplates, _selectedTemplate);
         return allTemplates.ToList();
      }

      private void loadTemplateWithReferences<T>(Cache<string, T> allTemplates, Template template)
      {
         if(allTemplates.Contains(template.Name))
            return;

         allTemplates.Add(template.Name, loadTemplate<T>(template));
         foreach (var reference in template.References)
         {
            loadTemplateWithReferences(allTemplates, reference);
         }
      }

      private T loadSingleTemplate<T>()
      {
         return loadTemplate<T>(_selectedTemplate);
      }

      private T loadTemplate<T>(Template template)
      {
         return _templateTaskQuery.LoadTemplate<T>(template);
      }

      private void updateIcon(TemplateType templateType)
      {
         if (!_shouldAddItemIcons) return;

         var icon = ApplicationIcons.IconByName(templateType.ToString());
         if (icon != ApplicationIcons.EmptyIcon)
            _view.SetIcon(icon);
      }

      private void updateView()
      {
         _view.OkEnabled = (_selectedTemplate != null);
         _view.Description = _selectedTemplate == null ? string.Empty : _selectedTemplate.Description;
      }

      private void addTemplatesTo(ITreeNode rootNode, TemplateDatabaseType templateDatabaseType)
      {
         foreach (var bb in _availableBuildingBlocks.Where(x => x.DatabaseType == templateDatabaseType).OrderBy(x=>x.Name))
         {
            var node = _treeNodeFactory.CreateFor(bb).Under(rootNode);
            if (_shouldAddItemIcons)
               node.WithIcon(ApplicationIcons.IconByName(bb.TemplateType.ToString()));
         }
         _view.AddNode(rootNode);
      }

      public IEnumerable<Template> AllTemplates()
      {
         return _availableBuildingBlocks;
      }

      public void ActivateNode(ITreeNode node)
      {
         _selectedTemplate = node.TagAsObject as Template;
         updateView();
      }

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
         if (result == ViewResult.No) return;
         _templateTaskQuery.DeleteTemplate(template);
         _view.DestroyNode(template.Id);
      }

      public bool CanEdit(Template template)
      {
         return _startOptions.IsDeveloperMode || template.DatabaseType == TemplateDatabaseType.User;
      }

      public void ShowContextMenu(ITreeNode nodeRequestingPopup, Point popupLocation)
      {
         var contextMenu = _contextMenuFactory.CreateFor(nodeRequestingPopup, this);
         contextMenu.Show(_view, popupLocation);
      }
   }
}