using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Services;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Compounds;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Views.Compounds;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Presentation.DTO;

namespace PKSim.Presentation.Presenters.Compounds
{
  

   public interface IFractionUnboundGroupPresenter : ICompoundParameterGroupWithAlternativePresenter
   {
      void SetFractionUnboundUnit(IParameterDTO fractionUnboundParameter, Unit newUnit);
      void SetFractionUnboundValue(FractionUnboundAlternativeDTO fractionUnboundAlternativeDTO, double newValue);
      void SetSpeciesValue(FractionUnboundAlternativeDTO fractionUnboundAlternativeDTO, Species species);
      PlasmaProteinBindingPartner PlasmaProteinBindingPartner { get; set; }

      /// <summary>
      ///    Returns all available species
      /// </summary>
      IEnumerable<Species> AllSpecies();
   }

   public class FractionUnboundGroupPresenter : CompoundParameterGroupWithAlternativePresenter<IFractionUnboundGroupView>, IFractionUnboundGroupPresenter
   {
      private readonly IParameterGroupAlternativeToFractionUnboundAlternativeDTOMapper _fractionUnboundAlternativeDTOMapper;
      private readonly ISpeciesRepository _speciesRepository;
      private readonly IParameterTask _parameterTask;
      private IReadOnlyList<FractionUnboundAlternativeDTO> _fractionUnboundDTOs;
      private IParameter _plasmaProteinPartner;

      public FractionUnboundGroupPresenter(IFractionUnboundGroupView view, IRepresentationInfoRepository representationRepository,
         ICompoundAlternativeTask compoundAlternativeTask,
         IParameterGroupAlternativeToFractionUnboundAlternativeDTOMapper fractionUnboundAlternativeDTOMapper,
         ISpeciesRepository speciesRepository, IDialogCreator dialogCreator, IParameterTask parameterTask) :
            base(view, representationRepository, compoundAlternativeTask, dialogCreator, CoreConstants.Groups.COMPOUND_FRACTION_UNBOUND)
      {
         _fractionUnboundAlternativeDTOMapper = fractionUnboundAlternativeDTOMapper;
         _speciesRepository = speciesRepository;
         _parameterTask = parameterTask;
      }

      public override void EditCompound(Compound compound)
      {
         _plasmaProteinPartner = compound.Parameter(CoreConstants.Parameter.PLASMA_PROTEIN_BINDING_PARTNER);
         base.EditCompound(compound);
      }

      public void SetFractionUnboundValue(FractionUnboundAlternativeDTO fractionUnboundAlternativeDTO, double newValue)
      {
         AddCommand(_compoundAlternativeTask.SetAlternativeParameterValue(fractionUnboundAlternativeDTO.FractionUnboundParameter.Parameter, newValue));
      }

      public void SetSpeciesValue(FractionUnboundAlternativeDTO fractionUnboundAlternativeDTO, Species species)
      {
         AddCommand(_compoundAlternativeTask.SetSpeciesForAlternative(fractionUnboundAlternativeDTO.ParameterAlternative, species));
      }

      public PlasmaProteinBindingPartner PlasmaProteinBindingPartner
      {
         get => _compound.PlasmaProteinBindingPartner;
         set => AddCommand(_parameterTask.SetParameterDisplayValue(_plasmaProteinPartner, value));
      }

      public IEnumerable<Species> AllSpecies()
      {
         return _speciesRepository.All();
      }

      public void SetFractionUnboundUnit(IParameterDTO fractionUnboundParameter, Unit newUnit)
      {
         AddCommand(_compoundAlternativeTask.SetAlternativeParameterUnit(fractionUnboundParameter.Parameter, newUnit));
      }

      protected override IEnumerable<ParameterAlternativeDTO> FillUpParameterGroupAlternatives()
      {
         _fractionUnboundDTOs = _parameterGroup.AllAlternatives.Cast<ParameterAlternativeWithSpecies>().MapAllUsing(_fractionUnboundAlternativeDTOMapper);
         _view.BindTo(_fractionUnboundDTOs);
         return _fractionUnboundDTOs;
      }
   }
}