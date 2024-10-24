﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Presenters;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Core;

namespace PKSim.Presentation.Services
{
   public class TemplateTask : ITemplateTask
   {
      private readonly ITemplateTaskQuery _templateTaskQuery;
      private readonly IApplicationController _applicationController;
      private readonly IObjectTypeResolver _objectTypeResolver;
      private readonly IDialogCreator _dialogCreator;

      public TemplateTask(ITemplateTaskQuery templateTaskQuery, IApplicationController applicationController, IObjectTypeResolver objectTypeResolver, IDialogCreator dialogCreator)
      {
         _templateTaskQuery = templateTaskQuery;
         _applicationController = applicationController;
         _objectTypeResolver = objectTypeResolver;
         _dialogCreator = dialogCreator;
      }

      public void SaveToTemplate<T>(T objectToSave, TemplateType templateType, string defaultName) where T : class
      {
         using (var namePresenter = _applicationController.Start<INameTemplatePresenter>())
         {
            if (!namePresenter.NewName(defaultName, templateType))
               return;

            var templateItem = new LocalTemplate
            {
               Name = namePresenter.Name,
               Description = namePresenter.Description,
               Type = templateType,
               Object = objectToSave,
               DatabaseType = TemplateDatabaseType.User,
            };
            _templateTaskQuery.SaveToTemplate(templateItem);
            _dialogCreator.MessageBoxInfo(PKSimConstants.UI.TemplateSuccessfullySaved(templateItem.Name, _objectTypeResolver.TypeFor(objectToSave)));
         }
      }

      public void SaveToTemplate<T>(T objectToSave, TemplateType templateType) where T : class, IWithName
      {
         SaveToTemplate(objectToSave, templateType, objectToSave.Name);
      }

      public Task<IReadOnlyList<T>> LoadFromTemplateAsync<T>(TemplateType templateType) where T : class
      {
         using (var presenter = _applicationController.Start<ITemplatePresenter>())
         {
            return presenter.LoadFromTemplateAsync<T>(templateType);
         }
      }

      public async Task<T> LoadSingleFromTemplateAsync<T>(TemplateType templateType) where T : class
      {
         return (await LoadFromTemplateAsync<T>(templateType)).FirstOrDefault();
      }
   }
}