using System.Collections.Generic;
using OSPSuite.Assets;
using OSPSuite.Core.Extensions;
using PKSim.Core.Model;
using PKSim.Presentation.DTO.Individuals;
using PKSim.Presentation.Services;
using PKSim.Presentation.Views.Individuals;
using static PKSim.Core.Model.TransportDirection;

namespace PKSim.Presentation.Presenters.Individuals
{
   public interface ITransporterExpressionParametersPresenter : IExpressionParametersPresenter<TransporterExpressionParameterDTO>
   {
      void SetTransportDirection(TransporterExpressionParameterDTO transporter, TransportDirection transportDirection);
      IReadOnlyList<TransportDirection> AllTransportDirectionsFor(TransporterExpressionParameterDTO expressionParameterDTO);
      ApplicationIcon TransporterDirectionIconFor(TransportDirection transportDirection);
   }

   public class TransporterExpressionParametersPresenter : ExpressionParametersPresenter<TransporterExpressionParameterDTO>,
      ITransporterExpressionParametersPresenter
   {
      private static readonly TransportDirection[] PLASMA_DIRECTIONS = {PlasmaToInterstitial, InterstitialToPlasma, BiDirectional};
      private static readonly TransportDirection[] CELLS_DIRECTIONS = {Influx, Efflux, BiDirectional};

      public TransporterExpressionParametersPresenter(ITransporterExpressionParametersView view,
         IEditParameterPresenterTask editParameterPresenterTask) : base(view, editParameterPresenterTask)
      {
      }

      public void SetTransportDirection(TransporterExpressionParameterDTO transporter, TransportDirection transportDirection)
      {
      }

      public IReadOnlyList<TransportDirection> AllTransportDirectionsFor(TransporterExpressionParameterDTO expressionParameterDTO)
      {
         var direction = expressionParameterDTO.TransportDirection;

         if (direction.IsOneOf(Influx, Efflux))
            return CELLS_DIRECTIONS;

         if (direction.IsOneOf(PlasmaToInterstitial, InterstitialToPlasma))
            return PLASMA_DIRECTIONS;

         return new[] {direction};
      }

      public ApplicationIcon TransporterDirectionIconFor(TransportDirection transportDirection)
      {
         switch (transportDirection)
         {
            case Influx:
               return ApplicationIcons.Influx;
            case Efflux:
               return ApplicationIcons.Efflux;
            case BiDirectional:
               return ApplicationIcons.Refresh;
            case Excretion:
               return ApplicationIcons.Excretion;
            case InterstitialToPlasma:
               return ApplicationIcons.Interstitial;
            case PlasmaToInterstitial:
               return ApplicationIcons.Plasma;
            default:
               return ApplicationIcons.EmptyIcon;
         }
      }
   }
}