using System.Collections.Generic;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Container;
using PKSim.Core;
using PKSim.Core.Repositories;
using PKSim.Infrastructure.ORM.Repositories;
using OSPSuite.Core.Domain;

namespace PKSim.IntegrationTests
{
   public abstract class concern_for_ModelContainerMoleculeRepository : ContextForIntegration<IModelContainerMoleculeRepository>
   {
   }

   public class when_getting_molecule_names_for_model : concern_for_ModelContainerMoleculeRepository
   {
      private IEnumerable<string> _molecules_4Comp;
      private IEnumerable<string> _molecules_2Poren;

      protected override void Because()
      {
         _molecules_4Comp = sut.MoleculeNamesWithoutDrug(CoreConstants.Model.FOUR_COMP);
         _molecules_2Poren = sut.MoleculeNamesWithoutDrug(CoreConstants.Model.TWO_PORES);
      }

      [Observation]
      public void molecule_names_4comp_should_be_empty()
      {
         _molecules_4Comp.Count().ShouldBeEqualTo(0);
      }

      [Observation]
      public void molecule_names_2poren_should_not_contain_drug()
      {
         _molecules_2Poren.Contains(CoreConstants.Molecule.Drug).ShouldBeFalse();
      }

      [Observation]
      public void molecule_names_2poren_should_contain_FcRn_and_FcRnComplex()
      {
         _molecules_2Poren.ShouldContain(CoreConstants.Molecule.FcRn, CoreConstants.Molecule.DrugFcRnComplexTemplate);
      }
   }

   public class when_getting_is_present_info_for_molecules : concern_for_ModelContainerMoleculeRepository
   {
      private int _igg_pls_id, _igg_int_id, _igg_endo_id, salivagland_id;
      private int _liv_periportal_pls_id, _liv_periportal_cell_id, _liv_periportal_int_id, _liv_periportal_endo_id, _liv_periportal_bc_id;
      private int _liv_pericentral_pls_id, _liv_pericentral_cell_id, _liv_pericentral_int_id, _liv_pericentral_endo_id, _liv_pericentral_bc_id;
      private int[] _liver_periportal_ids, _liver_pericentral_ids;
      private int[] _igg_ids;
      private readonly string _fcRn = CoreConstants.Molecule.FcRn;
      private readonly string _fcRnComplex = CoreConstants.Molecule.DrugFcRnComplexTemplate;
      private readonly string _ligandEndo = CoreConstants.Molecule.LigandEndo;
      private readonly string _ligandEndoComplex = CoreConstants.Molecule.LigandEndoComplex;
      private string[] _molecules, _moleculesWithoutDrug, _moleculesWithoutLigandEndo;
      private readonly string _drug = CoreConstants.Molecule.Drug;

      protected override void Context()
      {
         base.Context();
         var flatContainerRepo = IoC.Resolve<IFlatContainerRepository>();

         const string endoIggPrefix = "Organism|EndogenousIgG|";
         const string liverPeriportalPrefix = "Organism|Liver|Periportal|";
         const string liverPericentralPrefix = "Organism|Liver|Pericentral|";

         _igg_pls_id = flatContainerRepo.ContainerFrom(endoIggPrefix + CoreConstants.Compartment.PLASMA).Id;
         _igg_int_id = flatContainerRepo.ContainerFrom(endoIggPrefix + CoreConstants.Compartment.INTERSTITIAL).Id;
         _igg_endo_id = flatContainerRepo.ContainerFrom(endoIggPrefix + CoreConstants.Compartment.ENDOSOME).Id;
         _igg_ids = new int[] {_igg_pls_id, _igg_int_id, _igg_endo_id};

         _liv_periportal_pls_id = flatContainerRepo.ContainerFrom(liverPeriportalPrefix + "Plasma").Id;
         _liv_periportal_cell_id = flatContainerRepo.ContainerFrom(liverPeriportalPrefix + "Intracellular").Id;
         _liv_periportal_int_id = flatContainerRepo.ContainerFrom(liverPeriportalPrefix + "Interstitial").Id;
         _liv_periportal_endo_id = flatContainerRepo.ContainerFrom(liverPeriportalPrefix + "Endosome").Id;
         _liv_periportal_bc_id = flatContainerRepo.ContainerFrom(liverPeriportalPrefix + "BloodCells").Id;
         _liver_periportal_ids = new int[] {_liv_periportal_pls_id, _liv_periportal_cell_id, _liv_periportal_int_id, _liv_periportal_endo_id, _liv_periportal_bc_id};

         _liv_pericentral_pls_id = flatContainerRepo.ContainerFrom(liverPericentralPrefix + "Plasma").Id;
         _liv_pericentral_cell_id = flatContainerRepo.ContainerFrom(liverPericentralPrefix + "Intracellular").Id;
         _liv_pericentral_int_id = flatContainerRepo.ContainerFrom(liverPericentralPrefix + "Interstitial").Id;
         _liv_pericentral_endo_id = flatContainerRepo.ContainerFrom(liverPericentralPrefix + "Endosome").Id;
         _liv_pericentral_bc_id = flatContainerRepo.ContainerFrom(liverPericentralPrefix + "BloodCells").Id;
         _liver_pericentral_ids = new int[] { _liv_pericentral_pls_id, _liv_pericentral_cell_id, _liv_pericentral_int_id, _liv_pericentral_endo_id, _liv_pericentral_bc_id };

         salivagland_id=flatContainerRepo.ContainerFrom("Organism|Saliva|SalivaGland").Id;

         _molecules = new string[] {_drug, _fcRn, _fcRnComplex, _ligandEndo, _ligandEndoComplex};
         _moleculesWithoutDrug = new string[] {_fcRn, _fcRnComplex, _ligandEndo, _ligandEndoComplex };
         _moleculesWithoutLigandEndo =new string[] { _drug, _fcRn, _fcRnComplex, _ligandEndoComplex };
      }

      private void shouldContainOnly(string model, IEnumerable<int> ids, string[] molecules)
      {
         foreach (var id in ids)
         {
            foreach (var molecule in _molecules)
            {
               sut.IsPresent(model, id, molecule).ShouldBeEqualTo(molecules.Contains(molecule));
            }
         }
      }

      private void shouldReturnNegativeValuesAllowedFalseForAllMolecules(string model, ObjectPath path, IEnumerable<string> molecules)
      {
         foreach (var molecule in molecules)
         {
            sut.NegativeValuesAllowed(model, path, molecule).ShouldBeFalse();
         }
      }

      private void shouldReturnNegativeValuesAllowedFalseForAllMoleculesAndModels(ObjectPath path, string[] molecules)
      {
         shouldReturnNegativeValuesAllowedFalseForAllMolecules(CoreConstants.Model.FOUR_COMP, path, molecules);
         shouldReturnNegativeValuesAllowedFalseForAllMolecules(CoreConstants.Model.TWO_PORES, path, molecules);
      }

      [Observation]
      public void should_return_negative_values_allowed_true_only_for_predefined_compartments()
      {
         var pathIggSource = new ObjectPath("Organism", "EndogenousIgG", "IgG_Source");
         sut.NegativeValuesAllowed(CoreConstants.Model.TWO_PORES,pathIggSource,_ligandEndo).ShouldBeTrue();
         shouldReturnNegativeValuesAllowedFalseForAllMolecules(CoreConstants.Model.TWO_PORES, pathIggSource, _moleculesWithoutLigandEndo);

         var pathSalivaGland = new ObjectPath("Organism", "Saliva", "SalivaGland");
         sut.NegativeValuesAllowed(CoreConstants.Model.TWO_PORES,pathSalivaGland,_drug).ShouldBeTrue();
         sut.NegativeValuesAllowed(CoreConstants.Model.FOUR_COMP,pathSalivaGland,_drug).ShouldBeTrue();
         shouldReturnNegativeValuesAllowedFalseForAllMoleculesAndModels(pathSalivaGland, _moleculesWithoutDrug);

         var somePathsWhereAllMoleculesMustBePositive = new ObjectPath[]
         {
            new ObjectPath("Organism", "Bone", "Intracellular"),
            new ObjectPath("Organism", "Fat", "Plasma"),
            new ObjectPath("Organism", "EndogenousIgG", "Plasma"),
            new ObjectPath("Organism", "EndogenousIgG", "Interstitial"),
            new ObjectPath("Organism", "EndogenousIgG", "Endosome"),
            new ObjectPath("Organism", "Spleen", "Endosome")
         };

         foreach (var path in somePathsWhereAllMoleculesMustBePositive)
         {
            shouldReturnNegativeValuesAllowedFalseForAllMoleculesAndModels(path, _molecules);
         }
      }

      [Observation]
      public void salivagland_should_contain_drug_only()
      {
         shouldContainOnly(CoreConstants.Model.FOUR_COMP, new int[]{salivagland_id}, new[] { _drug });
         shouldContainOnly(CoreConstants.Model.TWO_PORES, new int[] { salivagland_id }, new[] { _drug });
      }

      [Observation]
      public void all_liver_zone_containers_in_4comp_should_contain_drug_only()
      {
         if (_liver_periportal_ids != null)
            shouldContainOnly(CoreConstants.Model.FOUR_COMP, _liver_periportal_ids, new[] {_drug});
         if (_liver_pericentral_ids != null)
            shouldContainOnly(CoreConstants.Model.FOUR_COMP, _liver_pericentral_ids, new[] { _drug });
      }

      [Observation]
      public void liver_zone_blood_cells_and_cells_in_2poren_should_contain_drug_only()
      {
         shouldContainOnly(CoreConstants.Model.TWO_PORES,
            new int[] {_liv_periportal_bc_id, _liv_periportal_cell_id},
            new string[] {_drug});

         shouldContainOnly(CoreConstants.Model.TWO_PORES,
            new int[] { _liv_pericentral_bc_id, _liv_pericentral_cell_id },
            new string[] { _drug });
      }

      [Observation]
      public void liver_zone_plasma_interstitial_endosome_in_2poren_should_contain_drug_and_FcRnKomplex()
      {
         shouldContainOnly(CoreConstants.Model.TWO_PORES,
            new[] {_liv_periportal_pls_id, _liv_periportal_int_id, _liv_periportal_endo_id},
            new[] {_drug, _fcRnComplex});

         shouldContainOnly(CoreConstants.Model.TWO_PORES,
            new[] { _liv_pericentral_pls_id, _liv_pericentral_int_id, _liv_pericentral_endo_id },
            new[] { _drug, _fcRnComplex });      
      }

      [Observation]
      public void endogenous_igg_plasma_interstitial_endosome_in_2poren_should_contain_FcRn_Ligand_and_LigandComplex()
      {
         shouldContainOnly(CoreConstants.Model.TWO_PORES,
            _igg_ids,
            new[] {_fcRn, _ligandEndo, _ligandEndoComplex});
      }
   }
}