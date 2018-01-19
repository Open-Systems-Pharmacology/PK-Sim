using System.Linq;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using OSPSuite.Presentation.DTO;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Presenters.Compounds
{
   public interface IParameterAlternativeNamePresenter : IObjectBasePresenter<ParameterAlternativeGroup>
   {
   }

   public class ParameterAlternativeNamePresenter : ObjectBasePresenter<ParameterAlternativeGroup>, IParameterAlternativeNamePresenter
   {
      private readonly IRepresentationInfoRepository _representationInfoRepository;

      public ParameterAlternativeNamePresenter(IObjectBaseView view, IRepresentationInfoRepository representationInfoRepository)
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
}