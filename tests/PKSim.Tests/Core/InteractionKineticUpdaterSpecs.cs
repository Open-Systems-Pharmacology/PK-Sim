using System.Collections.Generic;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;
using FakeItEasy;
using PKSim.Core.Model;
using PKSim.Core.Services;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Formulas;

namespace PKSim.Core
{
   public abstract class concern_for_InteractionKineticUpdater : ContextSpecification<IInteractionKineticUpdater>
   {
      protected ReactionBuilder _reaction;
      protected Simulation _simulation;
      private InteractionProperties _interactionProperties;
      private IRepository<IInteractionKineticUpdaterSpecification> _repository;
      protected List<IInteractionKineticUpdaterSpecification> _allKineticUpdaterSpecifications;
      private const string _enzymeName = "CYP3A4";
      private const string _processName = "MyInhibition";
      protected const string _drugName = "Drug";
      protected TransporterMoleculeContainer _transporterMoleculeContainer;
      protected IFormulaCache _formulaCache;
      protected PKSimParameter _kmFactor;
      protected PKSimParameter _vmaxFactor;
      private IObjectBaseFactory _objectBaseFactory;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _allKineticUpdaterSpecifications = new List<IInteractionKineticUpdaterSpecification>();
      }

      protected override void Context()
      {
         _formulaCache = A.Fake<IFormulaCache>();
         _repository = A.Fake<IRepository<IInteractionKineticUpdaterSpecification>>();
         _objectBaseFactory = A.Fake<IObjectBaseFactory>();
         A.CallTo(() => _objectBaseFactory.Create<ExplicitFormula>()).ReturnsLazily(x => new ExplicitFormula());
         A.CallTo(() => _repository.All()).Returns(_allKineticUpdaterSpecifications);
         sut = new InteractionKineticUpdater(_repository, _objectBaseFactory);

         _reaction = new ReactionBuilder();
         _transporterMoleculeContainer = new TransporterMoleculeContainer();
         _simulation = A.Fake<Simulation>();
         _interactionProperties = new InteractionProperties();

         _interactionProperties.AddInteraction(new InteractionSelection {MoleculeName = _enzymeName, ProcessName = _processName});
         A.CallTo(() => _simulation.InteractionProperties).Returns(_interactionProperties);


         _kmFactor = new PKSimParameter().WithName(CoreConstants.Parameters.KM_INTERACTION_FACTOR).WithFormula(new ExplicitFormula("1"));
         _vmaxFactor = new PKSimParameter().WithName(CoreConstants.Parameters.KCAT_INTERACTION_FACTOR).WithFormula(new ExplicitFormula("1"));

         _reaction.Add(_kmFactor);
         _reaction.Add(_vmaxFactor);

         _transporterMoleculeContainer.Add(_kmFactor);
         _transporterMoleculeContainer.Add(_vmaxFactor);
      }
   }

   public class When_updating_the_kinetic_of_a_reaction_for_which_the_triggering_enzyme_is_not_being_inhibited : concern_for_InteractionKineticUpdater
   {
      private IInteractionKineticUpdaterSpecification _kineticUpdaterSpecification;

      protected override void Context()
      {
         _kineticUpdaterSpecification = A.Fake<IInteractionKineticUpdaterSpecification>();
         A.CallTo(_kineticUpdaterSpecification).WithReturnType<bool>().Returns(false);
         _allKineticUpdaterSpecifications.Add(_kineticUpdaterSpecification);
         base.Context();
      }

      protected override void Because()
      {
         sut.UpdateReaction(_reaction, "NOT INHIBITED ENZYME",_drugName, _simulation, _formulaCache);
      }

      [Observation]
      public void should_not_update_the_km_and_vmax_inhibition_factor()
      {
         _kmFactor.Value.ShouldBeEqualTo(1);
         _vmaxFactor.Value.ShouldBeEqualTo(1);
      }
   }

   public class When_updating_the_kinetic_of_a_reaction_for_which_the_triggering_enzyme_is_being_inhibited : concern_for_InteractionKineticUpdater
   {
      private IInteractionKineticUpdaterSpecification _kineticUpdaterSpecification1;
      private IInteractionKineticUpdaterSpecification _kineticUpdaterSpecification2;
      private const string _inhibitedEnzyme = "INHIBITED ENZYME";

      protected override void Context()
      {
         _kineticUpdaterSpecification1 = A.Fake<IInteractionKineticUpdaterSpecification>();
         _kineticUpdaterSpecification2 = A.Fake<IInteractionKineticUpdaterSpecification>();
         A.CallTo(_kineticUpdaterSpecification1).WithReturnType<bool>().Returns(true);
         A.CallTo(_kineticUpdaterSpecification2).WithReturnType<bool>().Returns(true);
         _allKineticUpdaterSpecifications.Add(_kineticUpdaterSpecification1);
         _allKineticUpdaterSpecifications.Add(_kineticUpdaterSpecification2);
         base.Context();

         A.CallTo(() => _kineticUpdaterSpecification1.KcatDenominatorTerm(_inhibitedEnzyme, _drugName, _simulation)).Returns("A1/B1");
         A.CallTo(() => _kineticUpdaterSpecification1.KmNumeratorTerm(_inhibitedEnzyme, _drugName, _simulation)).Returns("C1/D1");
         A.CallTo(() => _kineticUpdaterSpecification1.KmDenominatorTerm(_inhibitedEnzyme, _drugName, _simulation)).Returns(string.Empty);

         A.CallTo(() => _kineticUpdaterSpecification2.KcatDenominatorTerm(_inhibitedEnzyme, _drugName, _simulation)).Returns(string.Empty);
         A.CallTo(() => _kineticUpdaterSpecification2.KmNumeratorTerm(_inhibitedEnzyme, _drugName, _simulation)).Returns("C2/D2");
         A.CallTo(() => _kineticUpdaterSpecification2.KmDenominatorTerm(_inhibitedEnzyme, _drugName, _simulation)).Returns("E2/D2");
      }

      protected override void Because()
      {
         sut.UpdateReaction(_reaction, _inhibitedEnzyme, _drugName, _simulation, _formulaCache);
      }

      [Observation]
      public void should_update_the_km_and_vmax_values_as_expected()
      {
         _kmFactor.Formula.DowncastTo<ExplicitFormula>().FormulaString.ShouldBeEqualTo("(1 + C1/D1 + C2/D2)/(1 + E2/D2)");
         _vmaxFactor.Formula.DowncastTo<ExplicitFormula>().FormulaString.ShouldBeEqualTo("1/(1 + A1/B1)");
      }
   }

   public class When_updating_the_kinetic_of_a_reaction_for_which_the_triggering_enzyme_is_also_responsible_for_metabolization : concern_for_InteractionKineticUpdater
   {
      private IInteractionKineticUpdaterSpecification _kineticUpdaterSpecification1;
      private IInteractionKineticUpdaterSpecification _kineticUpdaterSpecification2;
      private const string _inhibitedEnzyme = "INHIBITED ENZYME";

      protected override void Context()
      {
         _kineticUpdaterSpecification1 = A.Fake<IInteractionKineticUpdaterSpecification>();
         _kineticUpdaterSpecification2 = A.Fake<IInteractionKineticUpdaterSpecification>();
         A.CallTo(_kineticUpdaterSpecification1).WithReturnType<bool>().Returns(true);
         A.CallTo(_kineticUpdaterSpecification2).WithReturnType<bool>().Returns(true);
         _allKineticUpdaterSpecifications.Add(_kineticUpdaterSpecification1);
         _allKineticUpdaterSpecifications.Add(_kineticUpdaterSpecification2);
         base.Context();

         A.CallTo(() => _kineticUpdaterSpecification1.KcatDenominatorTerm(_inhibitedEnzyme, _drugName, _simulation)).Returns("A1/B1");
         A.CallTo(() => _kineticUpdaterSpecification1.KmNumeratorTerm(_inhibitedEnzyme, _drugName, _simulation)).Returns("C1/D1");
         A.CallTo(() => _kineticUpdaterSpecification1.KmDenominatorTerm(_inhibitedEnzyme, _drugName, _simulation)).Returns(string.Empty);

         A.CallTo(() => _kineticUpdaterSpecification2.KcatDenominatorTerm(_inhibitedEnzyme, _drugName, _simulation)).Returns(string.Empty);
         A.CallTo(() => _kineticUpdaterSpecification2.KmNumeratorTerm(_inhibitedEnzyme, _drugName, _simulation)).Returns("C2/D2");
         A.CallTo(() => _kineticUpdaterSpecification2.KmDenominatorTerm(_inhibitedEnzyme, _drugName, _simulation)).Returns("E2/D2");
      }

      protected override void Because()
      {
         sut.UpdateReaction(_reaction, _inhibitedEnzyme, _drugName, _simulation, _formulaCache);
      }

      [Observation]
      public void should_update_the_km_and_vmax_values_as_expected()
      {
         _kmFactor.Formula.DowncastTo<ExplicitFormula>().FormulaString.ShouldBeEqualTo("(1 + C1/D1 + C2/D2)/(1 + E2/D2)");
         _vmaxFactor.Formula.DowncastTo<ExplicitFormula>().FormulaString.ShouldBeEqualTo("1/(1 + A1/B1)");
      }
   }

   public class When_updating_the_kinetic_of_a_transporter_for_which_the_triggering_transport_is_being_inhibited : concern_for_InteractionKineticUpdater
   {
      private IInteractionKineticUpdaterSpecification _kineticUpdaterSpecification1;
      private IInteractionKineticUpdaterSpecification _kineticUpdaterSpecification2;
      private const string _inhibitedTransporter = "INHIBITED TRANSPORTER";

      protected override void Context()
      {
         _kineticUpdaterSpecification1 = A.Fake<IInteractionKineticUpdaterSpecification>();
         _kineticUpdaterSpecification2 = A.Fake<IInteractionKineticUpdaterSpecification>();
         A.CallTo(_kineticUpdaterSpecification1).WithReturnType<bool>().Returns(true);
         A.CallTo(_kineticUpdaterSpecification2).WithReturnType<bool>().Returns(true);
         _allKineticUpdaterSpecifications.Add(_kineticUpdaterSpecification1);
         _allKineticUpdaterSpecifications.Add(_kineticUpdaterSpecification2);
         base.Context();

         A.CallTo(() => _kineticUpdaterSpecification1.KcatDenominatorTerm(_inhibitedTransporter, _drugName, _simulation)).Returns("A1/B1");
         A.CallTo(() => _kineticUpdaterSpecification1.KmNumeratorTerm(_inhibitedTransporter, _drugName, _simulation)).Returns("C1/D1");
         A.CallTo(() => _kineticUpdaterSpecification1.KmDenominatorTerm(_inhibitedTransporter, _drugName, _simulation)).Returns(string.Empty);

         A.CallTo(() => _kineticUpdaterSpecification2.KcatDenominatorTerm(_inhibitedTransporter, _drugName, _simulation)).Returns(string.Empty);
         A.CallTo(() => _kineticUpdaterSpecification2.KmNumeratorTerm(_inhibitedTransporter, _drugName, _simulation)).Returns("C2/D2");
         A.CallTo(() => _kineticUpdaterSpecification2.KmDenominatorTerm(_inhibitedTransporter, _drugName, _simulation)).Returns("E2/D2");
      }

      protected override void Because()
      {
         sut.UpdateTransport(_transporterMoleculeContainer, _inhibitedTransporter, _drugName, _simulation, _formulaCache);
      }

      [Observation]
      public void should_update_the_km_and_vmax_values_as_expected()
      {
         _kmFactor.Formula.DowncastTo<ExplicitFormula>().FormulaString.ShouldBeEqualTo("(1 + C1/D1 + C2/D2)/(1 + E2/D2)");
         _vmaxFactor.Formula.DowncastTo<ExplicitFormula>().FormulaString.ShouldBeEqualTo("1/(1 + A1/B1)");
      }

      [Observation]
      public void should_used_the_name_of_the_transported_molecule_in_the_name_of_the_factor_formula()
      {
         _kmFactor.Formula.Name.Contains(_drugName).ShouldBeTrue();
      }
   }
}