using OSPSuite.Presentation.Presenters;
using PKSim.Core;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Core.Snapshots;
using PKSim.Core.Snapshots.Mappers;
using PKSim.Presentation.Presenters.Individuals;
using PKSim.UI.Starter.Views;
using Individual = PKSim.Core.Model.Individual;
using OriginData = PKSim.Core.Snapshots.OriginData;

namespace PKSim.UI.Starter.Presenters
{
   public interface IStarterRelativeExpressionPresenter : IDisposablePresenter
   {
      void Start();
   }

   public class StarterRelativeExpressionPresenter : AbstractDisposableCommandCollectorPresenter<IStarterRelativeExpressionView, IStarterRelativeExpressionPresenter>,
      IStarterRelativeExpressionPresenter
   {
      private readonly OriginDataMapper _originDataMapper;
      private readonly IIndividualFactory _individualFactory;
      private readonly IIndividualEnzymeFactory _individualEnzymeFactory;
      private readonly IIndividualEnzymeExpressionsPresenter<Individual> _individualEnzymeExpressionsPresenter;

      public StarterRelativeExpressionPresenter(
         IStarterRelativeExpressionView view,
         OriginDataMapper originDataMapper,
         IIndividualFactory individualFactory,
         IIndividualEnzymeFactory individualEnzymeFactory, IIndividualEnzymeExpressionsPresenter<Individual> individualEnzymeExpressionsPresenter) : base(view)
      {
         _originDataMapper = originDataMapper;
         _individualFactory = individualFactory;
         _individualEnzymeFactory = individualEnzymeFactory;
         _individualEnzymeExpressionsPresenter = individualEnzymeExpressionsPresenter;
         AddSubPresenters(individualEnzymeExpressionsPresenter);
         InitializeWith(new PKSimMacroCommand());
      }

      public void Start()
      {
         var individual = createIndividual();
         var enzyme = _individualEnzymeFactory.AddMoleculeTo(individual, "CYP3A4");
         View.AddExpressionPresenter(_individualEnzymeExpressionsPresenter.BaseView);
         _individualEnzymeExpressionsPresenter.SimulationSubject = individual;
         _individualEnzymeExpressionsPresenter.ActivateMolecule(enzyme);
         View.Display();
      }

      private Individual createIndividual()
      {
         var originData = new OriginData
         {
            Species = CoreConstants.Species.HUMAN,
            Population = CoreConstants.Population.ICRP,
            Age = new Parameter
            {
               Value = 30,
               Unit = "year(s)",
            },
            Weight = new Parameter
            {
               Value = 75,
               Unit = "kg",
            },
            Height = new Parameter
            {
               Value = 175,
               Unit = "cm",
            },
            Gender = CoreConstants.Gender.MALE
         };

         var modelOriginData = _originDataMapper.MapToModel(originData, new SnapshotContext()).Result;
         var individual = _individualFactory.CreateAndOptimizeFor(modelOriginData);

         return individual;
      }
   }
}