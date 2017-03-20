using System;
using OSPSuite.Utility;
using OSPSuite.Utility.Container;
using PKSim.Presentation.Nodes;
using OSPSuite.Presentation.Presenters.Nodes;

namespace PKSim.Presentation.Presenters.Individuals.Mappers
{
   public interface IRootNodeToIndividualExpressionsPresenterMapper<TSimulationSubject> : IMapper<RootNode, IIndividualMoleculeExpressionsPresenter>
   {
   }

   public class RootNodeToIndividualExpressionsPresenterMapper<TSimulationSubject> : IRootNodeToIndividualExpressionsPresenterMapper<TSimulationSubject>
   {
      private readonly IContainer _container;

      public RootNodeToIndividualExpressionsPresenterMapper(IContainer container)
      {
         _container = container;
      }

      public IIndividualMoleculeExpressionsPresenter MapFrom(RootNode rootNode)
      {
         if (rootNode.Tag == PKSimRootNodeTypes.IndividualMetabolizingEnzymes)
            return _container.Resolve<IIndividualEnzymeExpressionsPresenter<TSimulationSubject>>();

         if (rootNode.Tag == PKSimRootNodeTypes.IndividualProteinBindingPartners)
            return _container.Resolve<IIndividualOtherProteinExpressionsPresenter<TSimulationSubject>>();

         if (rootNode.Tag == PKSimRootNodeTypes.IndividualTransportProteins)
            return _container.Resolve<IIndividualTransporterExpressionsPresenter<TSimulationSubject>>();


         throw new ArgumentException(rootNode.Text);
      }
   }
}