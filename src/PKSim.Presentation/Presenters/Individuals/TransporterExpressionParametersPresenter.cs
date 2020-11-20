using System;
using System.Collections.Generic;
using OSPSuite.Core.Extensions;
using PKSim.Core.Model;
using PKSim.Presentation.DTO.Individuals;
using PKSim.Presentation.Services;
using PKSim.Presentation.Views.Individuals;
using static PKSim.Core.Model.TransportDirections;

namespace PKSim.Presentation.Presenters.Individuals
{
   public interface ITransporterExpressionParametersPresenter : IExpressionParametersPresenter<TransporterExpressionParameterDTO>
   {
      Action<TransporterExpressionParameterDTO, TransportDirection> SetTransportDirection { get; set; }
      IReadOnlyList<TransportDirection> AllTransportDirectionsFor(TransporterExpressionParameterDTO expressionParameterDTO);
   }

   public class TransporterExpressionParametersPresenter : ExpressionParametersPresenter<TransporterExpressionParameterDTO>,
      ITransporterExpressionParametersPresenter
   {
      private static readonly TransportDirection[] PLASMA_DIRECTIONS = {PlasmaToInterstitial, InterstitialToPlasma, VascEndoBiDirectional};
      private static readonly TransportDirection[] CELLS_DIRECTIONS = {Influx, Efflux, CellsBiDirectional, PgpLike};

      public TransporterExpressionParametersPresenter(
         ITransporterExpressionParametersView view,
         IEditParameterPresenterTask editParameterPresenterTask
      ) : base(view, editParameterPresenterTask)
      {
      }

      public Action<TransporterExpressionParameterDTO, TransportDirection> SetTransportDirection { get; set; } = (x, y) => { };

      public IReadOnlyList<TransportDirection> AllTransportDirectionsFor(TransporterExpressionParameterDTO expressionParameterDTO)
      {
         var direction = expressionParameterDTO.TransportDirection;

         if (direction.IsOneOf(CELLS_DIRECTIONS))
            return CELLS_DIRECTIONS;

         if (direction.IsOneOf(PLASMA_DIRECTIONS))
            return PLASMA_DIRECTIONS;

         return new[] {direction};
      }
   }
}