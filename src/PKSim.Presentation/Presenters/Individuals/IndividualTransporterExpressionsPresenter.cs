using System.Collections.Generic;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Individuals;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Presenters.Parameters;
using PKSim.Presentation.Services;
using PKSim.Presentation.Views.Individuals;

namespace PKSim.Presentation.Presenters.Individuals
{
   public interface IIndividualTransporterExpressionsPresenter :
      IIndividualMoleculeExpressionsPresenter,
      IEditParameterPresenter
   {
      /// <summary>
      ///    Update the transport direction for the given transporter container
      /// </summary>
      void SetTransportDirection(TransporterExpressionParameterDTO transporterExpressionContainerDTO, TransportDirection transportDirection);

      /// <summary>
      ///    Returns the available transporter types
      /// </summary>
      IEnumerable<TransportTypeDTO> AllTransportTypes();

      /// <summary>
      ///    This function is called when the user triggers the transporter type change. All process, for which the selected
      ///    transporter type may be apply will be updated
      /// </summary>
      /// <param name="newTransportType"> </param>
      void UpdateTransportType(TransportType newTransportType);
   }

   public interface IIndividualTransporterExpressionsPresenter<TSimulationSubject> : IIndividualTransporterExpressionsPresenter
   {
   }

   public class IndividualTransporterExpressionsPresenter<TSimulationSubject> :
      EditParameterPresenter<IIndividualTransporterExpressionsView, IIndividualTransporterExpressionsPresenter>,
      IIndividualTransporterExpressionsPresenter<TSimulationSubject> where TSimulationSubject : ISimulationSubject
   {
      private readonly IMoleculeExpressionTask<TSimulationSubject> _moleculeExpressionTask;
      private readonly IIndividualTransporterToTransporterExpressionDTOMapper _transporterExpressionDTOMapper;
      private readonly IIndividualMoleculePropertiesPresenter<TSimulationSubject> _moleculePropertiesPresenter;
      private readonly ITransporterExpressionParametersPresenter _transporterExpressionParametersPresenter;
      private IndividualTransporter _transporter;
      private IndividualTransporterDTO _transporterExpressionDTO;
      public ISimulationSubject SimulationSubject { get; set; }

      public IndividualTransporterExpressionsPresenter(
         IIndividualTransporterExpressionsView view,
         IEditParameterPresenterTask parameterTask,
         IMoleculeExpressionTask<TSimulationSubject> moleculeExpressionTask,
         IIndividualTransporterToTransporterExpressionDTOMapper transporterExpressionDTOMapper,
         IIndividualMoleculePropertiesPresenter<TSimulationSubject> moleculePropertiesPresenter,
         ITransporterExpressionParametersPresenter transporterExpressionParametersPresenter)
         : base(view, parameterTask)
      {
         _moleculeExpressionTask = moleculeExpressionTask;
         _transporterExpressionDTOMapper = transporterExpressionDTOMapper;
         _moleculePropertiesPresenter = moleculePropertiesPresenter;
         _transporterExpressionParametersPresenter = transporterExpressionParametersPresenter;
         _transporterExpressionParametersPresenter.SetTransportDirection = SetTransportDirection;
         AddSubPresenters(_moleculePropertiesPresenter, _transporterExpressionParametersPresenter);
         view.AddMoleculePropertiesView(_moleculePropertiesPresenter.View);
         view.AddExpressionParametersView(_transporterExpressionParametersPresenter.View);
      }

      public void SetTransportDirection(TransporterExpressionParameterDTO transporterExpressionContainerDTO, TransportDirection transportDirection)
      {
         AddCommand(_moleculeExpressionTask.SetTransportDirection(transporterExpressionContainerDTO.TransporterExpressionContainer,
            transportDirection.Id));
      }

      public void UpdateTransportType(TransportType newTransportType)
      {
         AddCommand(_moleculeExpressionTask.SetTransporterTypeFor(_transporter, newTransportType));
         //required to refresh view when changing transport type as some UI elements need to be invalidated
         _transporterExpressionDTOMapper.UpdateExpressionParameters(_transporterExpressionDTO, SimulationSubject);
         rebind();
      }

      public IEnumerable<TransportTypeDTO> AllTransportTypes() => TransportTypes.All();

      public bool OntogenyVisible
      {
         set => _moleculePropertiesPresenter.OntogenyVisible = value;
      }

      public bool MoleculeParametersVisible
      {
         set => _moleculePropertiesPresenter.MoleculeParametersVisible = value;
      }

      public void ActivateMolecule(IndividualMolecule molecule)
      {
         _transporter = molecule.DowncastTo<IndividualTransporter>();
         _transporterExpressionDTO = _transporterExpressionDTOMapper.MapFrom(_transporter, SimulationSubject.DowncastTo<TSimulationSubject>());
         _view.BindTo(_transporterExpressionDTO);
         rebind();
      }

      private void updateWarning()
      {
         if (_transporterExpressionDTO.DefaultAvailableInDatabase)
            return;

         //The transporter type does not have the default type. This means that the value was changed by the user.
         //nothing to show
         if (_transporter.TransportType != CoreConstants.DEFAULT_TRANSPORTER_TYPE)
         {
            _view.HideWarning();
            return;
         }

         _view.ShowWarning(PKSimConstants.Warning.NoTransporterTemplateFoundForTransporter(_transporter.Name, _transporter.TransportType.ToString()));
      }

      public void DisableEdit()
      {
         _moleculePropertiesPresenter.DisableEdit();
         _transporterExpressionParametersPresenter.DisableEdit();
         _view.ReadOnly = false;
      }

      private void rebind()
      {
         _moleculePropertiesPresenter.Edit(_transporter, SimulationSubject.DowncastTo<TSimulationSubject>());
         _transporterExpressionParametersPresenter.Edit(_transporterExpressionDTO.AllExpressionParameters);
         updateWarning();
      }
   }
}