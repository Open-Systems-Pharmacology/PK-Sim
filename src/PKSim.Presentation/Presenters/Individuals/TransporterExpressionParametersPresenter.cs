using System;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Presentation.DTO.Individuals;
using PKSim.Presentation.Services;
using PKSim.Presentation.Views.Individuals;
using static PKSim.Core.Model.TransportDirections;

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

      private static readonly TransportDirectionId[][] ALL_DIRECTIONS =
      {
         PLASMA_DIRECTIONS, 
         BLOOD_CELLS_DIRECTIONS, 
         MUCOSA_DIRECTIONS,
         TISSUE_DIRECTIONS, 
         BRAIN_TISSUE_DIRECTIONS, 
         BRAIN_BBB_DIRECTIONS
      };

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

         return new[] {direction};
      }
   }
}