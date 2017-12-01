using System.Collections.Generic;
using System.Linq;
using PKSim.Assets;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Assets;
using OSPSuite.Utility.Events;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Events;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Individuals;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Presenters.Parameters;
using PKSim.Presentation.Services;
using PKSim.Presentation.Views.Individuals;
using OSPSuite.Core.Domain;

namespace PKSim.Presentation.Presenters.Individuals
{
   public interface IIndividualTransporterExpressionsPresenter : IIndividualMoleculeExpressionsPresenter,
      IListener<NoTranporterTemplateAvailableEvent>

   {
      /// <summary>
      ///    Update the menbrane ttype for the given transporter container
      /// </summary>
      void SetMembraneLocation(TransporterExpressionContainerDTO transporterExpressionContainerDTO, MembraneLocation membraneLocation);

      /// <summary>
      ///    Returns the available membrane type for the given expression container
      /// </summary>
      IEnumerable<MembraneLocation> AllProteinMembraneLocationsFor(TransporterExpressionContainerDTO transporterExpressionContainerDTO);

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
      ///    Returns the display name associaed with the given transport type
      /// </summary>
      string TransportTypeCaptionFor(TransportType transportType);
   }

   public interface IIndividualTransporterExpressionsPresenter<TSimulationSubject> : IIndividualTransporterExpressionsPresenter
   {
      
   }

   public class IndividualTransporterExpressionsPresenter<TSimulationSubject> : EditParameterPresenter<IIndividualTransporterExpressionsView, IIndividualTransporterExpressionsPresenter>, IIndividualTransporterExpressionsPresenter<TSimulationSubject> where TSimulationSubject : ISimulationSubject
   {
      private readonly IMoleculeExpressionTask<TSimulationSubject> _moleculeExpressionTask;
      private readonly IIndividualTransporterToTransporterExpressionDTOMapper _transporterExpressionDTOMapper;
      private readonly IIndividualMoleculePropertiesPresenter<TSimulationSubject> _moleculePropertiesPresenter;
      private readonly ITransporterContainerTemplateRepository _transporterContainerTemplateRepository;
      private IndividualTransporter _transporter;
      public ISimulationSubject SimulationSubject { get; set; }

      public IndividualTransporterExpressionsPresenter(IIndividualTransporterExpressionsView view, IEditParameterPresenterTask parameterTask, IMoleculeExpressionTask<TSimulationSubject> moleculeExpressionTask,
         IIndividualTransporterToTransporterExpressionDTOMapper transporterExpressionDTOMapper,
         ITransporterContainerTemplateRepository transporterContainerTemplateRepository, IIndividualMoleculePropertiesPresenter<TSimulationSubject> moleculePropertiesPresenter)
         : base(view, parameterTask)
      {
         _moleculeExpressionTask = moleculeExpressionTask;
         _transporterExpressionDTOMapper = transporterExpressionDTOMapper;
         _moleculePropertiesPresenter = moleculePropertiesPresenter;
         _transporterContainerTemplateRepository = transporterContainerTemplateRepository;
         AddSubPresenters(_moleculePropertiesPresenter);
         view.AddMoleculePropertiesView(_moleculePropertiesPresenter.View);
      }

      public void SetMembraneLocation(TransporterExpressionContainerDTO transporterExpressionContainerDTO, MembraneLocation membraneLocation)
      {
         AddCommand(_moleculeExpressionTask.SetMembraneLocationFor(transporterContainerFrom(transporterExpressionContainerDTO), _transporter.TransportType, membraneLocation));
      }

      public void UpdateTransportType(TransportType newTransportType)
      {
         AddCommand(_moleculeExpressionTask.SetTransporterTypeFor(_transporter, newTransportType));
      }

      public override void AddCommand(ICommand command)
      {
         base.AddCommand(command);
         //always hide warning as soon as the user changes the default settings
         _view.HideWarning();
      }

      public ApplicationIcon IconFor(TransportType transportType)
      {
         return TransportTypes.By(transportType).Icon;
      }

      public string TransportTypeCaptionFor(TransportType transportType)
      {
         return TransportTypes.By(transportType).DisplayName;
      }

      private TransporterExpressionContainer transporterContainerFrom(TransporterExpressionContainerDTO transporterExpressionContainerDTO)
      {
         return _transporter.AllExpressionsContainers().FindByName(transporterExpressionContainerDTO.ContainerName);
      }

      public IEnumerable<MembraneLocation> AllProteinMembraneLocationsFor(TransporterExpressionContainerDTO transporterExpressionContainerDTO)
      {
         return _transporterContainerTemplateRepository
            .TransportersFor(SimulationSubject.Species.Name, transporterExpressionContainerDTO.ContainerName)
            .Where(x => x.TransportType == _transporter.TransportType)
            .Select(x => x.MembraneLocation)
            .Distinct();
      }

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
         _view.BindTo(_transporterExpressionDTOMapper.MapFrom(_transporter));
         _moleculePropertiesPresenter.Edit(molecule,SimulationSubject.DowncastTo<TSimulationSubject>());
      }

      public void SetRelativeExpression(ExpressionContainerDTO expressionContainerDTO, double value)
      {
         AddCommand(_moleculeExpressionTask.SetRelativeExpressionFor(_transporter, expressionContainerDTO.ContainerName, value));
      }

      public void Handle(NoTranporterTemplateAvailableEvent eventToHandle)
      {
         if (!Equals(_transporter, eventToHandle.Transporter))
            return;

         _view.ShowWarning(PKSimConstants.Warning.NoTransporterTemplateFoundForTransporter(_transporter.Name, _transporter.TransportType.ToString()));
      }
   }
}