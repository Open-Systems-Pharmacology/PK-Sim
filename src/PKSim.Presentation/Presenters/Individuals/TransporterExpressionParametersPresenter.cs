using System;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Presentation.DTO.Individuals;
using PKSim.Presentation.Services;
using PKSim.Presentation.Views.Individuals;
using static PKSim.Core.Model.TransportDirectionId;

namespace PKSim.Presentation.Presenters.Individuals
{
   public interface ITransporterExpressionParametersPresenter : IExpressionParametersPresenter<TransporterExpressionParameterDTO>
   {
      Action<TransporterExpressionParameterDTO, TransportDirection> SetTransportDirection { get; set; }
      IReadOnlyList<TransportDirection> AllPossibleTransportDirectionsFor(TransporterExpressionParameterDTO expressionParameterDTO);
   }

   public class TransporterExpressionParametersPresenter : ExpressionParametersPresenter<TransporterExpressionParameterDTO>,
      ITransporterExpressionParametersPresenter
   {
      private readonly ITransportDirectionRepository _transportDirectionRepository;
      private static readonly TransportDirectionId[] PLASMA_DIRECTIONS = {InfluxPlasmaToInterstitial, EffluxInterstitialToPlasma};
      private static readonly TransportDirectionId[] BLOOD_CELLS_DIRECTIONS = {InfluxPlasmaToBloodCells, EffluxBloodCellsToPlasma};

      private static readonly TransportDirectionId[] CELLS_DIRECTIONS =
      {
         InfluxInterstitialToIntracellular, EffluxIntracellularToInterstitial,
         PgpIntracellularToInterstitial
      };

      private static readonly TransportDirectionId[] BRAIN_TISSUE_DIRECTIONS =
      {
         InfluxBrainInterstitialToTissue, EffluxBrainTissueToInterstitial,
         PgpBrainTissueToInterstitial
      };

      private static readonly TransportDirectionId[] BRAIN_BBB_DIRECTIONS =
      {
         InfluxBrainPlasmaToInterstitial, EffluxBrainInterstitialToPlasma,
         PgpBrainInterstitialToPlasma
      };

      private static readonly TransportDirectionId[][] ALL_DIRECTIONS =
         {PLASMA_DIRECTIONS, BLOOD_CELLS_DIRECTIONS, CELLS_DIRECTIONS, BRAIN_TISSUE_DIRECTIONS, BRAIN_BBB_DIRECTIONS};

      public TransporterExpressionParametersPresenter(
         ITransporterExpressionParametersView view,
         IEditParameterPresenterTask editParameterPresenterTask,
         ITransportDirectionRepository transportDirectionRepository
      ) : base(view, editParameterPresenterTask)
      {
         _transportDirectionRepository = transportDirectionRepository;
      }

      public Action<TransporterExpressionParameterDTO, TransportDirection> SetTransportDirection { get; set; } = (x, y) => { };

      public IReadOnlyList<TransportDirection> AllPossibleTransportDirectionsFor(TransporterExpressionParameterDTO expressionParameterDTO)
      {
         var direction = expressionParameterDTO.TransportDirection;

         foreach (var possibleDirections in ALL_DIRECTIONS)
         {
            if (direction.Id.IsOneOf(possibleDirections))
               return possibleDirections.Select(x => _transportDirectionRepository.ById(x)).ToList();
         }

         return new[] { direction };
      }
   }
}