using System.Linq;
using OSPSuite.Presentation.DTO;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Views;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Repositories;

namespace PKSim.Presentation.Presenters.Compounds
{
   public interface IParameterAlternativeNamePresenter : IObjectBasePresenter<ParameterAlternativeGroup>
   {
   }

   public abstract class ParameterAlternativeNamePresenter<TView> : ObjectBasePresenter<ParameterAlternativeGroup>, IParameterAlternativeNamePresenter where TView : IObjectBaseView
   {
      private readonly IRepresentationInfoRepository _representationInfoRepository;

      protected ParameterAlternativeNamePresenter(TView view, IRepresentationInfoRepository representationInfoRepository)
         : base(view, true)
      {
         _representationInfoRepository = representationInfoRepository;
      }

      protected override void InitializeResourcesFor(ParameterAlternativeGroup parameterAlternativeGroup)
      {
         var parameterGroupDisplayName = _representationInfoRepository.DisplayNameFor(RepresentationObjectType.GROUP, parameterAlternativeGroup.Name);
         _view.Caption = PKSimConstants.UI.CreateGroupParameterAlternativeCaption(parameterGroupDisplayName);
         _view.NameDescription = PKSimConstants.UI.Name;
         _view.DescriptionVisible = false;
      }

      protected override ObjectBaseDTO CreateDTOFor(ParameterAlternativeGroup parameterAlternativeGroup)
      {
         var dto = new ObjectBaseDTO();
         dto.AddUsedNames(parameterAlternativeGroup.Children.Select(x => x.Name));
         return dto;
      }
   }

   public class ParameterAlternativeNamePresenter : ParameterAlternativeNamePresenter<IObjectBaseView>
   {
      public ParameterAlternativeNamePresenter(IObjectBaseView view, IRepresentationInfoRepository representationInfoRepository) : base(view, representationInfoRepository)
      {
      }
   }
}