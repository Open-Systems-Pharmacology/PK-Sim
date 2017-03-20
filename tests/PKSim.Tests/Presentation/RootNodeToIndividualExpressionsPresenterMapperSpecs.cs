using System;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Container;
using PKSim.Presentation.Nodes;
using FakeItEasy;
using PKSim.Core.Model;
using PKSim.Presentation.Presenters.Individuals;
using PKSim.Presentation.Presenters.Individuals.Mappers;
using OSPSuite.Presentation.Presenters.Nodes;

namespace PKSim.Presentation
{
   public abstract class concern_for_RootNodeToIndividualExpressionsPresenterMapper : ContextSpecification<IRootNodeToIndividualExpressionsPresenterMapper<Individual>>
   {
      protected IContainer _container;

      protected override void Context()
      {
         _container = A.Fake<IContainer>();
         sut = new RootNodeToIndividualExpressionsPresenterMapper<Individual>(_container);
      }
   }

   
   public class When_mapping_a_root_node_to_a_presenter : concern_for_RootNodeToIndividualExpressionsPresenterMapper
   {
      private IIndividualEnzymeExpressionsPresenter<Individual> _enzymePresenter;
      private IIndividualOtherProteinExpressionsPresenter<Individual> _otherPresenter;

      protected override void Context()
      {
         base.Context();
         _enzymePresenter = A.Fake<IIndividualEnzymeExpressionsPresenter<Individual>>();
         _otherPresenter = A.Fake<IIndividualOtherProteinExpressionsPresenter<Individual>>();

         A.CallTo(() => _container.Resolve<IIndividualEnzymeExpressionsPresenter<Individual>>()).Returns(_enzymePresenter);
         A.CallTo(() => _container.Resolve<IIndividualOtherProteinExpressionsPresenter<Individual>>()).Returns(_otherPresenter);
      }

      [Observation]
      public void should_return_a_presenter_for_enzyme_if_the_node_represents_the_enzymes_node()
      {
         sut.MapFrom(new RootNode(PKSimRootNodeTypes.IndividualMetabolizingEnzymes)).ShouldBeEqualTo(_enzymePresenter);
      }

      [Observation]
      public void should_return_a_presenter_for_other_proteings_if_the_node_represents_the_other_proteins_node()
      {
         sut.MapFrom(new RootNode(PKSimRootNodeTypes.IndividualProteinBindingPartners)).ShouldBeEqualTo(_otherPresenter);
      }

      [Observation]
      public void should_throw_an_exception_if_the_node_is_unknown()
      {
         The.Action(() => sut.MapFrom(new RootNode(RootNodeTypes.ObservedDataFolder))).ShouldThrowAn<Exception>();
      }
   }
}