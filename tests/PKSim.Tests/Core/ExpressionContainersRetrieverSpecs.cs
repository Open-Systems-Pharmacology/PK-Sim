using System;
using System.Collections.Generic;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Core.Model;
using PKSim.Core.Services;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;

namespace PKSim.Core
{
   public abstract class concern_for_ExpressionContainersRetriever : ContextSpecification<IExpressionContainersRetriever>
   {
      protected readonly Organism _organism = new Organism();
      protected readonly Organ _liver = new Organ().WithName("_liver");
      protected readonly Organ _kidney = new Organ().WithName("_kidney");
      protected readonly Compartment _intLiver = new Compartment().WithName(CoreConstants.Compartment.Interstitial);
      protected readonly Compartment _cellLiver = new Compartment().WithName(CoreConstants.Compartment.Intracellular);
      protected readonly Compartment _plsLiver = new Compartment().WithName(CoreConstants.Compartment.Plasma);
      protected readonly Compartment _bcLiver = new Compartment().WithName(CoreConstants.Compartment.BloodCells);
      protected readonly Compartment _endoLiver = new Compartment().WithName(CoreConstants.Compartment.Endosome);
      protected readonly Compartment _intKidney = new Compartment().WithName(CoreConstants.Compartment.Interstitial);
      protected readonly Compartment _cellKidney = new Compartment().WithName(CoreConstants.Compartment.Intracellular);
      protected readonly Compartment _plsKidney = new Compartment().WithName(CoreConstants.Compartment.Plasma);
      protected readonly Compartment _bcKidney = new Compartment().WithName(CoreConstants.Compartment.BloodCells);
      protected readonly Compartment _endoKidney = new Compartment().WithName(CoreConstants.Compartment.Endosome);
      protected IndividualProtein _protein;
      private readonly Organ _lumen = new Organ().WithName("Lumen");
      protected readonly Compartment _duodenum = new Compartment().WithName("Duodenum");

      protected override void Context()
      {
         sut = new ExpressionContainersRetriever();
         var objectPathFactory = new ObjectPathFactoryForSpecs();
         var entityPathResolver = new EntityPathResolverForSpecs();
         _liver.Add(_intLiver);
         _liver.Add(_cellLiver);
         _liver.Add(_plsLiver);
         _liver.Add(_bcLiver);
         _liver.Add(_endoLiver);

         _kidney.Add(_intKidney);
         _kidney.Add(_cellKidney);
         _kidney.Add(_plsKidney);
         _kidney.Add(_bcKidney);
         _kidney.Add(_endoKidney);

         _lumen.Add(_duodenum);
         _organism.Add(_liver);
         _organism.Add(_kidney);
         _organism.Add(_lumen);

         _protein = new IndividualEnzyme();
         _protein.Add(new MoleculeExpressionContainer {OrganPath = objectPathFactory.CreateObjectPathFrom(CoreConstants.Compartment.BloodCells)}.WithName(CoreConstants.Compartment.BloodCells));
         _protein.Add(new MoleculeExpressionContainer {OrganPath = objectPathFactory.CreateObjectPathFrom(CoreConstants.Compartment.Plasma)}.WithName(CoreConstants.Compartment.Plasma));
         _protein.Add(new MoleculeExpressionContainer {OrganPath = objectPathFactory.CreateObjectPathFrom(CoreConstants.Compartment.VascularEndothelium)}.WithName(CoreConstants.Compartment.VascularEndothelium));
         _protein.Add(new MoleculeExpressionContainer {OrganPath = entityPathResolver.ObjectPathFor(_liver)}.WithName(CoreConstants.Organ.Liver));
         _protein.Add(new MoleculeExpressionContainer {OrganPath = entityPathResolver.ObjectPathFor(_kidney)}.WithName(CoreConstants.Organ.Kidney));
         _protein.Add(new MoleculeExpressionContainer {OrganPath = entityPathResolver.ObjectPathFor(_lumen), GroupName = CoreConstants.Groups.GI_LUMEN, ContainerName = _duodenum.Name}.WithName(CoreConstants.ContainerName.LumenSegmentNameFor("Duodenum")));
      }
   }

   public class When_retrieving_the_container_for_a_spatial_structure_that_is_not_a_pksim_structure : concern_for_ExpressionContainersRetriever
   {
      [Observation]
      public void should_throw_an_exception()
      {
         The.Action(() => sut.AllContainersFor(new SpatialStructure(), A.Fake<IndividualMolecule>())).ShouldThrowAn<Exception>();
      }
   }

   public class When_retrieving_the_containers_in_a_spatial_structure_for_a_molecule_defined_in_the_extacellular_membrane_apical : concern_for_ExpressionContainersRetriever
   {
      private IEnumerable<IContainer> _results;

      protected override void Context()
      {
         base.Context();
         _protein.TissueLocation = TissueLocation.ExtracellularMembrane;
         _protein.MembraneLocation = MembraneLocation.Apical;
      }

      protected override void Because()
      {
         _results = sut.AllContainersFor(_organism, _protein);
      }

      [Observation]
      public void should_return_only_plasma_and_interstial_compartments_plus_lumen_segements()
      {
         _results.ShouldOnlyContain(_plsLiver, _plsKidney, _intLiver, _intKidney, _duodenum);
      }
   }

   public class When_retrieving_the_containers_in_a_spatial_structure_for_a_molecule_defined_in_the_extacellular_membrane_basolateral : concern_for_ExpressionContainersRetriever
   {
      private IEnumerable<IContainer> _results;

      protected override void Context()
      {
         base.Context();
         _protein.TissueLocation = TissueLocation.ExtracellularMembrane;
         _protein.MembraneLocation = MembraneLocation.Basolateral;
      }

      protected override void Because()
      {
         _results = sut.AllContainersFor(_organism, _protein);
      }

      [Observation]
      public void should_return_only_plasma_and_interstial_compartments_plus_lumen_segements()
      {
         _results.ShouldOnlyContain(_plsLiver, _plsKidney, _intLiver, _intKidney, _duodenum);
      }
   }

   public class When_retrieving_the_containers_in_a_spatial_structure_for_a_molecule_defined_in_the_intracellular_with_endosomal_localization : concern_for_ExpressionContainersRetriever
   {
      private IEnumerable<IContainer> _results;

      protected override void Context()
      {
         base.Context();
         _protein.IntracellularVascularEndoLocation = IntracellularVascularEndoLocation.Endosomal;
         _protein.TissueLocation = TissueLocation.Intracellular;
      }

      protected override void Because()
      {
         _results = sut.AllContainersFor(_organism, _protein);
      }

      [Observation]
      public void should_return_plasma_blood_cells_cells_and_endosome_compartments_plus_lumen_segements()
      {
         _results.ShouldOnlyContain(_plsLiver, _plsKidney, _bcLiver, _bcKidney, _cellKidney, _cellLiver, _endoLiver, _endoKidney, _duodenum);
      }
   }

   public class When_retrieving_the_containers_in_a_spatial_structure_for_a_molecule_defined_in_the_intracellular_without_endosomal_localization : concern_for_ExpressionContainersRetriever
   {
      private IEnumerable<IContainer> _results;

      protected override void Context()
      {
         base.Context();
         _protein.IntracellularVascularEndoLocation = IntracellularVascularEndoLocation.Interstitial;
         _protein.TissueLocation = TissueLocation.Intracellular;
      }

      protected override void Because()
      {
         _results = sut.AllContainersFor(_organism, _protein);
      }

      [Observation]
      public void should_return_plasma_blood_cells_cells_and_interstitial_compartments_plus_lumen_segements()
      {
         _results.ShouldOnlyContain(_plsLiver, _plsKidney, _bcLiver, _bcKidney, _cellKidney, _cellLiver, _intLiver, _intKidney, _duodenum);
      }
   }

   public class When_retrieving_the_containers_in_a_spatial_structure_for_a_molecule_defined_in_the_interstitial : concern_for_ExpressionContainersRetriever
   {
      private IEnumerable<IContainer> _results;

      protected override void Context()
      {
         base.Context();
         _protein.TissueLocation = TissueLocation.Interstitial;
      }

      protected override void Because()
      {
         _results = sut.AllContainersFor(_organism, _protein);
      }

      [Observation]
      public void should_return_plasma_blood_cells_and_interstitial_compartments_plus_lumen_segements()
      {
         _results.ShouldOnlyContain(_plsLiver, _plsKidney, _bcLiver, _bcKidney, _intLiver, _intKidney, _duodenum);
      }
   }
}