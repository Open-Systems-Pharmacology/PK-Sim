using System.Collections.Generic;
using System.Linq;
using OSPSuite.Assets;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Events;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core.Events;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Individuals;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Presenters.Parameters;
using PKSim.Presentation.Services;
using PKSim.Presentation.Views.Individuals;

namespace PKSim.Presentation.Presenters.Individuals
{
   public interface IIndividualTransporterExpressionsPresenter : IIndividualMoleculeExpressionsPresenter, IEditParameterPresenter,
      IListener<NoTranporterTemplateAvailableEvent>

   {
      /// <summary>
      ///    Update the transport direction for the given transporter container
      /// </summary>
      void SetTransportDirection(TransporterExpressionParameterDTO transporterExpressionContainerDTO, TransportDirection transportDirection);

      /// <summary>
      ///    Returns the available transporter types
      /// </summary>
      IEnumerable<TransportType> AllTransportTypes();

      /// <summary>
      ///    This function is called when the user triggers the transporter type change. All process, for which the selected
      ///    transporter type may be apply will be updated
      /// </summary>
      /// <param name="newTransportType"> </param>
      void UpdateTransportType(TransportType newTransportType);

      /// <summary>
      ///    Returns the application icon associated with the given transport type
      /// </summary>
      ApplicationIcon IconFor(TransportType transportType);

      /// <summary>
      ///    Returns the display name associated with the given transport type
      /// </summary>
      string TransportTypeCaptionFor(TransportType transportType);
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
      private readonly ITransporterContainerTemplateRepository _transporterContainerTemplateRepository;
      private IndividualTransporter _transporter;
      private IndividualTransporterDTO _transporterExpressionDTO;
      public ISimulationSubject SimulationSubject { get; set; }

      public IndividualTransporterExpressionsPresenter(
         IIndividualTransporterExpressionsView view, IEditParameterPresenterTask parameterTask,
         IMoleculeExpressionTask<TSimulationSubject> moleculeExpressionTask,
         IIndividualTransporterToTransporterExpressionDTOMapper transporterExpressionDTOMapper,
         ITransporterContainerTemplateRepository transporterContainerTemplateRepository,
         IIndividualMoleculePropertiesPresenter<TSimulationSubject> moleculePropertiesPresenter,
         ITransporterExpressionParametersPresenter transporterExpressionParametersPresenter)
         : base(view, parameterTask)
      {
         _moleculeExpressionTask = moleculeExpressionTask;
         _transporterExpressionDTOMapper = transporterExpressionDTOMapper;
         _moleculePropertiesPresenter = moleculePropertiesPresenter;
         _transporterExpressionParametersPresenter = transporterExpressionParametersPresenter;
         _transporterContainerTemplateRepository = transporterContainerTemplateRepository;
         _transporterExpressionParametersPresenter.SetTransportDirection = SetTransportDirection;
         AddSubPresenters(_moleculePropertiesPresenter, _transporterExpressionParametersPresenter);
         view.AddMoleculePropertiesView(_moleculePropertiesPresenter.View);
         view.AddExpressionParametersView(_transporterExpressionParametersPresenter.View);
      }

      public void SetTransportDirection(TransporterExpressionParameterDTO transporterExpressionContainerDTO, TransportDirection transportDirection)
      {
         AddCommand(_moleculeExpressionTask.SetTransportDirection(transporterExpressionContainerDTO.TransporterExpressionContainer,
            transportDirection));
      }

      public void UpdateTransportType(TransportType newTransportType)
      {
         AddCommand(_moleculeExpressionTask.SetTransporterTypeFor(_transporter, newTransportType));
         //required to refresh view when changing transport type as some UI elements need to be invalidated
         RefreshView();
      }

      public override void AddCommand(ICommand command)
      {
         base.AddCommand(command);
         //always hide warning as soon as the user changes the default settings
         _view.HideWarning();
      }

      public ApplicationIcon IconFor(TransportType transportType) => TransportTypes.By(transportType).Icon;

      public string TransportTypeCaptionFor(TransportType transportType) => TransportTypes.By(transportType).DisplayName;

      public IEnumerable<TransportType> AllTransportTypes()
      {
         return TransportTypes.All().Select(x => x.TransportType);
      }

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
         _view.HideWarning();
         _transporterExpressionDTO = _transporterExpressionDTOMapper.MapFrom(_transporter, SimulationSubject.DowncastTo<TSimulationSubject>());
         _view.BindTo(_transporterExpressionDTO);
         _moleculePropertiesPresenter.Edit(molecule, SimulationSubject.DowncastTo<TSimulationSubject>());
         _transporterExpressionParametersPresenter.Edit(_transporterExpressionDTO.AllExpressionParameters);
         RefreshView();
      }

      public void RefreshView()
      {
         _moleculePropertiesPresenter.RefreshView();
         _transporterExpressionParametersPresenter.Edit(_transporterExpressionDTO.AllExpressionParameters);
      }

      public void Handle(NoTranporterTemplateAvailableEvent eventToHandle)
      {
         if (!Equals(_transporter, eventToHandle.Transporter))
            return;

         _view.ShowWarning(PKSimConstants.Warning.NoTransporterTemplateFoundForTransporter(_transporter.Name, _transporter.TransportType.ToString()));
      }
   }
}