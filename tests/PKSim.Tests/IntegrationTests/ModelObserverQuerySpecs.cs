using System.Linq;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Descriptors;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Infrastructure;

namespace PKSim.IntegrationTests
{
   public class When_retrieving_the_observer_building_block_defined_for_some_model_properties_and_molecule_name : ContextForSimulationIntegration<IModelObserverQuery>
   {
      private string _compoundName;
      private ObserverBuildingBlock _observers;
      private MoleculeBuildingBlock _moleculeBuildingBlock;
      private string _complexProductName;
      private string _metaboliteProductName;
      private ObserverBuilder _observer;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _simulation = DomainFactoryForSpecs.CreateDefaultSimulation();
         _moleculeBuildingBlock = new MoleculeBuildingBlock();
         var compoundProperties = _simulation.CompoundPropertiesList.First();
         _compoundName = compoundProperties.Compound.Name;
         _moleculeBuildingBlock.Add(new MoleculeBuilder {Name = _compoundName, QuantityType = QuantityType.Drug});
         _moleculeBuildingBlock.Add(new MoleculeBuilder {Name = "Enzyme", QuantityType = QuantityType.Enzyme});
         _moleculeBuildingBlock.Add(new MoleculeBuilder {Name = "Metabolite", QuantityType = QuantityType.Metabolite});
         _moleculeBuildingBlock.Add(new MoleculeBuilder {Name = "Protein", QuantityType = QuantityType.OtherProtein});
         var specificBindingSelection = new ProcessSelection {CompoundName = _compoundName, MoleculeName = "Protein", ProcessName = "Specific Binding"};
         var metabolizationSelection = new EnzymaticProcessSelection {CompoundName = _compoundName, MoleculeName = "Protein", ProcessName = "Metabolism"};
         _complexProductName = specificBindingSelection.ProductName(CoreConstants.Molecule.Complex);
         _metaboliteProductName = metabolizationSelection.ProductName(CoreConstants.Molecule.Metabolite);
         _moleculeBuildingBlock.Add(new MoleculeBuilder {Name = _complexProductName, QuantityType = QuantityType.Complex});
         compoundProperties.Processes.SpecificBindingSelection.AddPartialProcessSelection(specificBindingSelection);
         compoundProperties.Processes.MetabolizationSelection.AddPartialProcessSelection(metabolizationSelection);

         var observerSet = new ObserverSet();
         var observerSetBuildingBlock = new UsedBuildingBlock("OBS_ID", PKSimBuildingBlockType.ObserverSet) {BuildingBlock = observerSet};
         _observer = new AmountObserverBuilder().WithName("Test");
         observerSet.Add(_observer);
         _simulation.AddUsedBuildingBlock(observerSetBuildingBlock);
      }

      protected override void Because()
      {
         _observers = sut.AllObserversFor(_moleculeBuildingBlock, _simulation);
      }

      [Observation]
      public void the_returned_observers_defined_for_all_molecules_should_have_an_empty_molecules_list()
      {
         foreach (var obs in _observers.Where(obs => obs.ForAll))
         {
            obs.MoleculeNames().Count().ShouldBeEqualTo(0);
         }
      }

      [Observation]
      public void the_returned_observers_defined_for_concentration_should_be_available_for_the_proteins()
      {
         foreach (var obs in _observers.Where(obs => obs.IsConcentration()))
         {
            obs.MoleculeNames().ShouldContain(_compoundName, "Enzyme");
         }
      }

      [Observation]
      public void should_have_created_a_fraction_of_dose_observer_per_compound_for_all_sink_metabolite_and_complex_defined_in_the_model()
      {
         var fractionOfDose = _observers.FindByName(CoreConstants.Observer.ObserverNameFrom(CoreConstants.Observer.FRACTION_OF_DOSE, _compoundName));
         fractionOfDose.ShouldNotBeNull();
         fractionOfDose.MoleculeNames().ShouldOnlyContain(_metaboliteProductName, _complexProductName);
         var formula = fractionOfDose.Formula.DowncastTo<ExplicitFormula>();
         var objectPath = formula.ObjectPaths.Find(x => x.Alias == "TotalDrugMass");
         objectPath[0].ShouldBeEqualTo(_compoundName);

         formula.FormulaString.StartsWith("TotalDrugMass>0").ShouldBeTrue();
      }

      [Observation]
      public void should_have_created_a_fraction_excreted_observer_for_the_drug()
      {
         var fractionExcretedToUrine = _observers.FindByName(CoreConstants.Observer.FRACTION_EXCRETED_TO_URINE);
         fractionExcretedToUrine.ShouldNotBeNull();
         fractionExcretedToUrine.MoleculeNames().ShouldOnlyContain(_compoundName);
      }

      [Observation]
      public void should_have_created_a_fraction_excreted_to_bile_observer_for_the_drug()
      {
         var fractionExcretedToBile = _observers.FindByName(CoreConstants.Observer.FRACTION_EXCRETED_TO_BILE);
         fractionExcretedToBile.ShouldNotBeNull();
         fractionExcretedToBile.MoleculeNames().ShouldOnlyContain(_compoundName);
      }

      [Observation]
      public void should_have_created_a_fraction_excreted_to_feces_observer_for_the_drug()
      {
         var fractionExcretedToFeces = _observers.FindByName(CoreConstants.Observer.FRACTION_EXCRETED_TO_FECES);
         fractionExcretedToFeces.ShouldNotBeNull();
         fractionExcretedToFeces.MoleculeNames().ShouldOnlyContain(_compoundName);
      }

      [Observation]
      public void should_have_created_a_receptor_occupancy_observer_for_the_drug()
      {
         var receptorOccupancy = _observers.FindByName(CoreConstants.Observer.ObserverNameFrom(CoreConstants.Observer.RECEPTOR_OCCUPANCY, _complexProductName));
         receptorOccupancy.ShouldNotBeNull();
         receptorOccupancy.Formula.DowncastTo<ExplicitFormula>().FormulaString.ShouldBeEqualTo("Protein + Complex > 0 ? Complex/(Protein + Complex) : 0");
         receptorOccupancy.MoleculeNames().ShouldOnlyContain(_complexProductName);
      }

      [Observation]
      public void concentration_in_lumen_observer_should_be_defined_in_all_lumen_segments_except_feces()
      {
         var tags = new Tags {new Tag(CoreConstants.Tags.LUMEN_SEGMENT)};

         var lumenSegment = A.Fake<IContainer>().WithName("Whatever, just not feces");
         A.CallTo(() => lumenSegment.Tags).Returns(tags);

         var feces = A.Fake<IContainer>().WithName(CoreConstants.Compartment.FECES);
         A.CallTo(() => feces.Tags).Returns(tags);

         var obs = _observers.FindByName(CoreConstants.Observer.CONCENTRATION_IN_LUMEN);

         obs.ContainerCriteria.IsSatisfiedBy(lumenSegment).ShouldBeTrue();
         obs.ContainerCriteria.IsSatisfiedBy(feces).ShouldBeFalse();
      }

      [Observation]
      public void should_have_added_the_user_defined_observers_to_the_observer_building_block()
      {
         _observers.ShouldContain(_observer);
      }
   }

   public class When_creating_the_observer_building_block_for_protein_model : ContextForSimulationIntegration<IModelObserverQuery>
   {
      private string _compoundName;
      private string _observerName;
      private ObserverBuildingBlock _observers;
      private MoleculeBuildingBlock _moleculeBuildingBlock;

      public override void GlobalContext()
      {
         base.GlobalContext();

         var compound = DomainFactoryForSpecs.CreateStandardCompound();
         var individual = DomainFactoryForSpecs.CreateStandardIndividual();
         var protocol = DomainFactoryForSpecs.CreateStandardIVBolusProtocol();

         _simulation = DomainFactoryForSpecs.CreateSimulationWith(individual, compound, protocol, CoreConstants.Model.TWO_PORES).DowncastTo<IndividualSimulation>();
         _moleculeBuildingBlock = new MoleculeBuildingBlock();
         var compoundProperties = _simulation.CompoundPropertiesList.First();
         _compoundName = compoundProperties.Compound.Name;
         _moleculeBuildingBlock.Add(new MoleculeBuilder {Name = _compoundName, QuantityType = QuantityType.Drug});

         _observerName = CoreConstants.Observer.ObserverNameFrom(CoreConstantsForSpecs.Observer.WHOLE_ORGAN_INCLUDING_FCRN_COMPLEX, _compoundName);
      }

      protected override void Because()
      {
         _observers = sut.AllObserversFor(_moleculeBuildingBlock, _simulation);
      }

      [Observation]
      public void should_create_WholeOrganInclFcRn_Observer()
      {
         _observers.ExistsByName(_observerName).ShouldBeTrue();
      }

      [Observation]
      public void WholeOrganInclFcRn_Observer_should_be_setup_only_for_compound()
      {
         var obs = _observers.FindByName(_observerName);
         obs.ForAll.ShouldBeFalse();

         obs.MoleculeList.MoleculeNames.ShouldOnlyContain(_compoundName);
      }
   }
}