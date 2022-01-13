using System.Collections.Generic;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.ParameterIdentifications;
using OSPSuite.Core.Domain.SensitivityAnalyses;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Presenters.ParameterIdentifications;
using OSPSuite.Utility.Visitor;
using PKSim.Core;
using PKSim.Core.Chart;
using PKSim.Core.Model;

namespace PKSim.Presentation.Core
{
   public interface IOpenSingleStartPresenterInvoker
   {
      ISingleStartPresenter OpenPresenterFor<T>(T subject);
   }

   public class OpenSingleStartPresenterInvoker : IOpenSingleStartPresenterInvoker,
      IVisitor<Individual>,
      IVisitor<Compound>,
      IVisitor<Protocol>,
      IVisitor<IndividualSimulation>,
      IVisitor<PopulationSimulation>,
      IVisitor<RandomPopulation>,
      IVisitor<ImportPopulation>,
      IVisitor<Formulation>,
      IVisitor<PKSimEvent>,
      IVisitor<IndividualSimulationComparison>,
      IVisitor<PopulationSimulationComparison>,
      IVisitor<DataRepository>,
      IVisitor<ParameterIdentification>,
      IVisitor<SensitivityAnalysis>,
      IVisitor<ObserverSet>,
      IVisitor<ExpressionProfile>,
      IVisitor<ParameterIdentificationFeedback>,
      IVisitor<IEnumerable<DataRepository>>,
      IStrictVisitor
   {
      private readonly IApplicationController _applicationController;
      private ISingleStartPresenter _presenter;
      private readonly ICommandCollector _commandCollector;

      public OpenSingleStartPresenterInvoker(IApplicationController applicationController, ICoreWorkspace commandCollector)
      {
         _applicationController = applicationController;
         _commandCollector = commandCollector;
      }

      public ISingleStartPresenter OpenPresenterFor<T>(T subject)
      {
         try
         {
            this.Visit(subject);
            return _presenter;
         }
         finally
         {
            _presenter = null;
         }
      }

      private void openPresenter<T>(T objectToVisit)
      {
         _presenter = _applicationController.Open(objectToVisit, _commandCollector);
      }

      public void Visit(Individual objToVisit) => openPresenter(objToVisit);

      public void Visit(Compound objToVisit) => openPresenter(objToVisit);

      public void Visit(Protocol objToVisit) => openPresenter(objToVisit);

      public void Visit(IndividualSimulation objToVisit) => openPresenter(objToVisit);

      public void Visit(PopulationSimulation objToVisit) => openPresenter(objToVisit);

      public void Visit(RandomPopulation objToVisit) => openPresenter(objToVisit);

      public void Visit(Formulation objToVisit) => openPresenter(objToVisit);

      public void Visit(PKSimEvent objToVisit) => openPresenter(objToVisit);

      public void Visit(IndividualSimulationComparison objToVisit) => openPresenter(objToVisit);

      public void Visit(ImportPopulation objToVisit) => openPresenter(objToVisit);

      public void Visit(PopulationSimulationComparison objToVisit) => openPresenter(objToVisit);

      public void Visit(DataRepository objToVisit) => openPresenter(objToVisit);

      public void Visit(IEnumerable<DataRepository> objToVisit) => openPresenter(objToVisit);

      public void Visit(ParameterIdentification objToVisit) => openPresenter(objToVisit);

      public void Visit(SensitivityAnalysis objToVisit) => openPresenter(objToVisit);

      public void Visit(ObserverSet objToVisit) => openPresenter(objToVisit);

      public void Visit(ExpressionProfile objToVisit) => openPresenter(objToVisit);

      public void Visit(ParameterIdentificationFeedback objToVisit) => openPresenter(objToVisit);
   }
}