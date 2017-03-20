using PKSim.Assets;
using OSPSuite.Assets;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Presentation.DTO;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Presenters
{
   public interface IEditDescriptionPresenter : IObjectBasePresenter<IObjectBase>
   {
      string Title { get; set; }
   }

   public class EditDescriptionPresenter : ObjectBasePresenter<IObjectBase>, IEditDescriptionPresenter
   {
      private readonly IObjectTypeResolver _objectTypeResolver;
      public string Title { get; set; }

      public EditDescriptionPresenter(IObjectBaseView view, IObjectTypeResolver objectTypeResolver) : base(view, descriptionVisible: true, nameVisible: false)
      {
         _objectTypeResolver = objectTypeResolver;
      }

      protected override void InitializeResourcesFor(IObjectBase entity)
      {
         string entityType = _objectTypeResolver.TypeFor(entity);
         _view.Caption = PKSimConstants.UI.EditDescription;
         if (!string.IsNullOrEmpty(Title))
            _view.Caption = Title;

         _view.NameDescription = PKSimConstants.UI.RenameEntityCaption(entityType, entity.Name);
         _view.Icon = ApplicationIcons.Description;
      }

      protected override ObjectBaseDTO CreateDTOFor(IObjectBase entity)
      {
         return new ObjectBaseDTO {DescriptionRequired = true, Name = entity.Name, Description = entity.Description};
      }
   }
}