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
      private readonly IIndividualPathWithRootExpander _individualPathWithRootExpander;

      protected IndividualProteinFactory(
         IObjectBaseFactory objectBaseFactory,
         IParameterFactory parameterFactory,
         IObjectPathFactory objectPathFactory,
         IEntityPathResolver entityPathResolver,
         IIndividualPathWithRootExpander individualPathWithRootExpander) : base(objectBaseFactory, parameterFactory, objectPathFactory,
         entityPathResolver)
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
            FractionParam(FRACTION_EXPRESSED_VASC_ENDO_APICAL, CoreConstants.Rate.ZERO_RATE),
            FractionParam(FRACTION_EXPRESSED_VASC_ENDO_BASOLATERAL, CoreConstants.Rate.PARAM_F_EXP_VASC_BASOLATERAL, editable: false)
         );

         AddTissueOrgansExpression(simulationSubject, moleculeName);
         AddLumenExpression(simulationSubject, moleculeName);
         AddMucosaExpression(simulationSubject, moleculeName);

         simulationSubject.AddMolecule(molecule);

         _individualPathWithRootExpander.AddRootToPathIn(simulationSubject, moleculeName);
         return molecule;
      }

      protected void AddTissueOrgansExpression(ISimulationSubject simulationSubject, string moleculeName)
      {
         var organism = simulationSubject.Organism;
         organism.NonGITissueContainers.Each(x => AddTissueParameters(x, moleculeName));
         organism.GITissueContainers.Each(x => AddTissueParameters(x, moleculeName));
      }

      protected void AddTissueParameters(IContainer organ, string moleculeName)
      {
         AddContainerExpression(organ.Container(CoreConstants.Compartment.BloodCells), moleculeName,
            InitialConcentrationParam(CoreConstants.Rate.INITIAL_CONCENTRATION_BLOOD_CELLS)
         );

         AddContainerExpression(organ.Container(CoreConstants.Compartment.Plasma), moleculeName,
            InitialConcentrationParam(CoreConstants.Rate.INITIAL_CONCENTRATION_PLASMA)
         );

         AddContainerExpression(organ.Container(CoreConstants.Compartment.Endosome), moleculeName,
            InitialConcentrationParam(CoreConstants.Rate.INITIAL_CONCENTRATION_ENDOSOME)
         );

         AddContainerExpression(organ.Container(CoreConstants.Compartment.Intracellular), moleculeName,
            RelExpParam(REL_EXP),
            FractionParam(FRACTION_EXPRESSED_INTRACELLULAR, CoreConstants.Rate.ONE_RATE),
            InitialConcentrationParam(CoreConstants.Rate.INITIAL_CONCENTRATION_INTRACELLULAR)
         );

         AddContainerExpression(organ.Container(CoreConstants.Compartment.Interstitial), moleculeName,
            FractionParam(FRACTION_EXPRESSED_INTERSTITIAL, CoreConstants.Rate.PARAM_F_EXP_INTERSTITIAL, editable: false),
            InitialConcentrationParam(CoreConstants.Rate.INITIAL_CONCENTRATION_INTERSTITIAL)
         );
      }
      
      protected void AddMucosaExpression(ISimulationSubject simulationSubject, string moleculeName)
      {
         foreach (var organ in simulationSubject.Organism.OrgansByName(CoreConstants.Organ.SmallIntestine, CoreConstants.Organ.LargeIntestine))
         {
            var organMucosa = organ.Container(CoreConstants.Compartment.Mucosa);
            organMucosa.GetChildren<Compartment>().Each(x => AddTissueParameters(x, moleculeName));
         }
      }

      protected void AddLumenExpression(ISimulationSubject simulationSubject, string moleculeName)
      {
         var lumen = simulationSubject.Organism.Organ(CoreConstants.Organ.Lumen);
         //Only visible compartments as we do not want to create RelExp for example in Feces
         foreach (var segment in lumen.Compartments.Where(x => x.Visible))
         {
            AddContainerExpression(segment, moleculeName, RelExpParam(REL_EXP),
               InitialConcentrationParam(CoreConstants.Rate.INITIAL_CONCENTRATION_LUMEN));
         }
      }

    
   }
}