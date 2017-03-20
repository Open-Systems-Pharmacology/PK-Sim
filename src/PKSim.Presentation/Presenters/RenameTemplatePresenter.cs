using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Services;
using OSPSuite.Presentation.DTO;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Presenters
{
   public interface IRenameTemplatePresenter : INameTemplatePresenter
   {
   }

   public class RenameTemplatePresenter : NameTemplatePresenter, IRenameTemplatePresenter
   {
      private readonly IRenameObjectDTOFactory _renameObjectBaseDTOFactory;

      public RenameTemplatePresenter(IObjectBaseView view, IRenameObjectDTOFactory renameObjectBaseDTOFactory, ITemplateTaskQuery templateTaskQuery) : base(view,  templateTaskQuery)
      {
         _renameObjectBaseDTOFactory = renameObjectBaseDTOFactory;
      }

      protected override void InitializeResourcesFor(Template template)
      {
         _view.Caption = PKSimConstants.UI.Rename;
         _view.NameDescription = PKSimConstants.UI.RenameEntityCaption(PKSimConstants.ObjectTypes.Template, template.Name);
      }

      protected override ObjectBaseDTO CreateDTOFor(Template template)
      {
         var dto = _renameObjectBaseDTOFactory.CreateFor(template);
         AddExistingTemplateNames(template, dto);
         return dto;
      }
   }
}