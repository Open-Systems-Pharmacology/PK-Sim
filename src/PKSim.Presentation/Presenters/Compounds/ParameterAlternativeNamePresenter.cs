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

      protected override void InitializeResourcesFor(ParameterAlternativeGroup compoundParamGroup)
      {
         var parameterGroupDisplayName = _representationInfoRepository.DisplayNameFor(RepresentationObjectType.GROUP, compoundParamGroup.Name);
         _view.Caption = PKSimConstants.UI.CreateGroupParameterAlternativeCaption(parameterGroupDisplayName);
      }

      protected override ObjectBaseDTO CreateDTOFor(ParameterAlternativeGroup compoundParamGroup)
      {
         var dto = new ObjectBaseDTO();
         dto.AddUsedNames(compoundParamGroup.Children.Select(x => x.Name));
         return dto;
      }
   }
}