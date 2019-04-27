using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Compounds;
using PKSim.Presentation.Views.Compounds;
using OSPSuite.Core.Domain;

namespace PKSim.Presentation.Presenters.Compounds
{
   public interface ICompoundMoleculeTypePresenter : ICompoundParameterGroupPresenter
   {
      void SetMoleculeType(bool isSmallMolecule);
   }

   public class CompoundMoleculeTypePresenter : CompoundParameterGroupPresenter<ICompoundMoleculeTypeView>, ICompoundMoleculeTypePresenter
   {
      private readonly IRepresentationInfoRepository _representationInfoRepository;
      private readonly IParameterTask _parameterTask;
      private IParameter _parameter;

      public CompoundMoleculeTypePresenter(ICompoundMoleculeTypeView view, IRepresentationInfoRepository representationInfoRepository, IParameterTask parameterTask) :
         base(view, representationInfoRepository, CoreConstants.Groups.COMPOUND, needsCaption: false)
      {
         _representationInfoRepository = representationInfoRepository;
         _parameterTask = parameterTask;
      }

      public override void EditCompound(Compound compound)
      {
         _parameter = compound.Parameter(Constants.Parameters.IS_SMALL_MOLECULE);
         var info = _representationInfoRepository.InfoFor(_parameter);
         var isSmallMolecule = new IsSmallMoleculeDTO {Value = _parameter.Value == 1, Description = info.Description, Display = info.DisplayName};
         _view.BindTo(isSmallMolecule);
      }

      public void SetMoleculeType(bool isSmallMolecule)
      {
         AddCommand(_parameterTask.SetParameterDisplayValueAsStructureChange(_parameter, isSmallMolecule));
      }
   }
}