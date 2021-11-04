using System.Collections.Generic;
using OSPSuite.Presentation.Presenters;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Presentation.DTO.ExpressionProfiles;
using PKSim.Presentation.Views.ExpressionProfiles;

namespace PKSim.Presentation.Presenters.ExpressionProfiles
{
   public interface IExpressionProfileMoleculesPresenter : IExpressionProfileItemPresenter
   {
      IEnumerable<Species> AllSpecies();
      void SpeciesChanged();
   }

   public class ExpressionProfileMoleculesPresenter : AbstractSubPresenter<IExpressionProfileMoleculesView, IExpressionProfileMoleculesPresenter>, IExpressionProfileMoleculesPresenter
   {
      private readonly ISpeciesRepository _speciesRepository;
      private ExpressionProfileDTO _expressionProfileDTO;
      private ExpressionProfile _expressionProfile;

      public ExpressionProfileMoleculesPresenter(
         IExpressionProfileMoleculesView view,
         ISpeciesRepository speciesRepository) : base(view)
      {
         _speciesRepository = speciesRepository;
      }

      public IEnumerable<Species> AllSpecies() => _speciesRepository.All();

      public void SpeciesChanged()
      {
         //TODO command
         _expressionProfile.Species = _expressionProfileDTO.Species;
      }

      public void Edit(ExpressionProfile expressionProfile)
      {
         _expressionProfile = expressionProfile;
         _expressionProfileDTO = new ExpressionProfileDTO {Species = expressionProfile.Species ?? _speciesRepository.DefaultSpecies};
         _view.BindTo(_expressionProfileDTO);
      }
   }
}