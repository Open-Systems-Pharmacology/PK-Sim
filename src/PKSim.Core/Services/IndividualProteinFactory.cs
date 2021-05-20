using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using static PKSim.Core.CoreConstants.Parameters;
using IParameterFactory = PKSim.Core.Model.IParameterFactory;

namespace PKSim.Core.Services
{
   public abstract class IndividualProteinFactory<TMolecule> : IndividualMoleculeFactory<TMolecule, MoleculeExpressionContainer>
      where TMolecule : IndividualProtein
   {
      protected readonly IIndividualPathWithRootExpander _individualPathWithRootExpander;

      protected IndividualProteinFactory(IObjectBaseFactory objectBaseFactory,
         IParameterFactory parameterFactory,
         IObjectPathFactory objectPathFactory,
         IEntityPathResolver entityPathResolver,
         IIndividualPathWithRootExpander individualPathWithRootExpander, IIdGenerator idGenerator) : base(objectBaseFactory, parameterFactory, objectPathFactory,
         entityPathResolver, idGenerator)
      {
         _individualPathWithRootExpander = individualPathWithRootExpander;
      }

      public override IndividualMolecule AddMoleculeTo(ISimulationSubject simulationSubject, string moleculeName)
      {
         var molecule = CreateMolecule(moleculeName);

         //default localization
         molecule.Localization = Localization.Intracellular;

         AddGlobalExpression(molecule,
            RelExpParam(REL_EXP_BLOOD_CELLS),
            FractionParam(FRACTION_EXPRESSED_BLOOD_CELLS, CoreConstants.Rate.ZERO_RATE),
            FractionParam(FRACTION_EXPRESSED_BLOOD_CELLS_MEMBRANE, CoreConstants.Rate.PARAM_F_EXP_BC_MEMBRANE, editable: false)
         );

         AddGlobalExpression(molecule, RelExpParam(REL_EXP_PLASMA));

         AddGlobalExpression(molecule,
            RelExpParam(REL_EXP_VASCULAR_ENDOTHELIUM),
            FractionParam(FRACTION_EXPRESSED_VASC_ENDO_ENDOSOME, CoreConstants.Rate.ZERO_RATE),
            FractionParam(FRACTION_EXPRESSED_VASC_ENDO_PLASMA_SIDE, CoreConstants.Rate.ZERO_RATE),
            FractionParam(FRACTION_EXPRESSED_VASC_ENDO_TISSUE_SIDE, CoreConstants.Rate.PARAM_F_EXP_VASC_TISSUE_SIDE, editable: false)
         );

         addVascularSystemInitialConcentration(simulationSubject, moleculeName);
         AddTissueOrgansExpression(simulationSubject, moleculeName);
         AddLumenExpression(simulationSubject, moleculeName);
         AddMucosaExpression(simulationSubject, moleculeName);

         simulationSubject.AddMolecule(molecule);

         _individualPathWithRootExpander.AddRootToPathIn(simulationSubject, moleculeName);
         return molecule;
      }

      private void addVascularSystemInitialConcentration(ISimulationSubject simulationSubject, string moleculeName)
      {
         var organism = simulationSubject.Organism;
         organism.OrgansByType(OrganType.VascularSystem).Each(organ =>
         {
            AddContainerExpression(organ.Container(CoreConstants.Compartment.BLOOD_CELLS), moleculeName,
               InitialConcentrationParam(CoreConstants.Rate.INITIAL_CONCENTRATION_BLOOD_CELLS)
            );

            AddContainerExpression(organ.Container(CoreConstants.Compartment.PLASMA), moleculeName,
               InitialConcentrationParam(CoreConstants.Rate.INITIAL_CONCENTRATION_PLASMA_VASCULAR_SYSTEM)
            );
         });
      }

      protected void AddTissueOrgansExpression(ISimulationSubject simulationSubject, string moleculeName)
      {
         var organism = simulationSubject.Organism;
         organism.NonGITissueContainers.Each(x => AddTissueParameters(x, moleculeName));
         organism.GITissueContainers.Each(x => AddTissueParameters(x, moleculeName));
      }

      protected void AddTissueParameters(IContainer organ, string moleculeName)
      {
         AddContainerExpression(organ.Container(CoreConstants.Compartment.BLOOD_CELLS), moleculeName,
            InitialConcentrationParam(CoreConstants.Rate.INITIAL_CONCENTRATION_BLOOD_CELLS)
         );

         AddContainerExpression(organ.Container(CoreConstants.Compartment.PLASMA), moleculeName,
            InitialConcentrationParam(CoreConstants.Rate.INITIAL_CONCENTRATION_PLASMA)
         );

         AddContainerExpression(organ.Container(CoreConstants.Compartment.ENDOSOME), moleculeName,
            InitialConcentrationParam(CoreConstants.Rate.INITIAL_CONCENTRATION_ENDOSOME)
         );

         AddContainerExpression(organ.Container(CoreConstants.Compartment.INTRACELLULAR), moleculeName,
            RelExpParam(REL_EXP),
            FractionParam(FRACTION_EXPRESSED_INTRACELLULAR, CoreConstants.Rate.ONE_RATE),
            InitialConcentrationParam(CoreConstants.Rate.INITIAL_CONCENTRATION_INTRACELLULAR)
         );

         AddContainerExpression(organ.Container(CoreConstants.Compartment.INTERSTITIAL), moleculeName,
            FractionParam(FRACTION_EXPRESSED_INTERSTITIAL, CoreConstants.Rate.PARAM_F_EXP_INTERSTITIAL, editable: false),
            InitialConcentrationParam(CoreConstants.Rate.INITIAL_CONCENTRATION_INTERSTITIAL)
         );
      }

      protected void AddMucosaExpression(ISimulationSubject simulationSubject, string moleculeName)
      {
         foreach (var organ in simulationSubject.Organism.OrgansByName(CoreConstants.Organ.SMALL_INTESTINE, CoreConstants.Organ.LARGE_INTESTINE))
         {
            var organMucosa = organ.Container(CoreConstants.Compartment.MUCOSA);
            organMucosa.GetChildren<Compartment>().Each(x => AddTissueParameters(x, moleculeName));
         }
      }

      protected void AddLumenExpression(ISimulationSubject simulationSubject, string moleculeName)
      {
         var lumen = simulationSubject.Organism.Organ(CoreConstants.Organ.LUMEN);
         //Only visible compartments as we do not want to create RelExp for example in Feces
         foreach (var segment in lumen.Compartments.Where(x => x.Visible))
         {
            AddContainerExpression(segment, moleculeName, RelExpParam(REL_EXP),
               InitialConcentrationParam(CoreConstants.Rate.INITIAL_CONCENTRATION_LUMEN));
         }
      }
   }
}