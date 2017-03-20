using System.Linq;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Services;
using OSPSuite.Presentation.DTO;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Presenters
{
   public interface INameTemplatePresenter : IObjectBasePresenter<Template>
   {
      bool NewName(string defaultName, TemplateType templateType);
   }

   public class NameTemplatePresenter : ObjectBasePresenter<Template>, INameTemplatePresenter
   {
      private readonly ITemplateTaskQuery _templateTaskQuery;

      public NameTemplatePresenter(IObjectBaseView view, ITemplateTaskQuery templateTaskQuery)
         : base(view, descriptionVisible: true)
      {
         _templateTaskQuery = templateTaskQuery;
      }

      protected override void InitializeResourcesFor(Template template)
      {
         _view.Caption = PKSimConstants.UI.EnterNameEntityCaption(PKSimConstants.UI.Template);
         _view.NameDescription = PKSimConstants.UI.Name;
      }

      protected override ObjectBaseDTO CreateDTOFor(Template template)
      {
         var dto = new ObjectBaseDTO { Name = template.Name };
         AddExistingTemplateNames(template, dto);
         return dto;
      }

      protected void AddExistingTemplateNames(Template template, ObjectBaseDTO dto)
      {
         var allTemplates = _templateTaskQuery.AllTemplatesFor(template.DatabaseType, template.TemplateType);
         dto.AddUsedNames(allTemplates.Select(x => x.Name));
         dto.ContainerType = PKSimConstants.UI.TemplateDatabase;
      }

      public bool NewName(string defaultName, TemplateType templateType)
      {
         return Edit(new Template {Name = defaultName, TemplateType = templateType});
      }
   }
}