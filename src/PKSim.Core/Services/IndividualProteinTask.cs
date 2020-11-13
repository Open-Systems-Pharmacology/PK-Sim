using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using static OSPSuite.Core.Domain.Constants.Dimension;
using static PKSim.Core.CoreConstants.Parameters;
using IParameterFactory = PKSim.Core.Model.IParameterFactory;

namespace PKSim.Core.Services
{
   public abstract class IndividualProteinTask<TMolecule> : IndividualMoleculeTask<TMolecule, MoleculeExpressionContainer>
      where TMolecule : IndividualProtein
   {
      private readonly IIndividualPathWithRootExpander _individualPathWithRootExpander;

      protected IndividualProteinTask(
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

         AddVascularSystemExpression(molecule,
            RelExpParam(REL_EXP_BLOOD_CELLS),
            fractionParam(FRACTION_EXPRESSED_BLOOD_CELLS, CoreConstants.Rate.ZERO_RATE),
            fractionParam(FRACTION_EXPRESSED_BLOOD_CELLS_MEMBRANE, CoreConstants.Rate.PARAM_F_EXP_BC_MEMBRANE, editable: false)
         );

         AddVascularSystemExpression(molecule, RelExpParam(REL_EXP_PLASMA));

         AddVascularSystemExpression(molecule,
            RelExpParam(REL_EXP_VASC_ENDO),
            fractionParam(FRACTION_EXPRESSED_VASC_ENDO_ENDOSOME, CoreConstants.Rate.ZERO_RATE),
            fractionParam(FRACTION_EXPRESSED_VASC_ENDO_APICAL, CoreConstants.Rate.ZERO_RATE),
            fractionParam(FRACTION_EXPRESSED_VASC_ENDO_BASOLATERAL, CoreConstants.Rate.PARAM_F_EXP_VASC_BASOLATERAL, editable: false)
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
            initialConcentrationParam(CoreConstants.Rate.INITIAL_CONCENTRATION_BLOOD_CELLS)
         );

         AddContainerExpression(organ.Container(CoreConstants.Compartment.Plasma), moleculeName,
            initialConcentrationParam(CoreConstants.Rate.INITIAL_CONCENTRATION_PLASMA)
         );

         AddContainerExpression(organ.Container(CoreConstants.Compartment.Endosome), moleculeName,
            initialConcentrationParam(CoreConstants.Rate.INITIAL_CONCENTRATION_ENDOSOME)
         );

         AddContainerExpression(organ.Container(CoreConstants.Compartment.Intracellular), moleculeName,
            RelExpParam(REL_EXP),
            fractionParam(FRACTION_EXPRESSED_INTRACELLULAR, CoreConstants.Rate.PARAM_F_EXP_INTRACELLULAR, editable: false),
            initialConcentrationParam(CoreConstants.Rate.INITIAL_CONCENTRATION_INTRACELLULAR)
         );

         AddContainerExpression(organ.Container(CoreConstants.Compartment.Interstitial), moleculeName,
            fractionParam(FRACTION_EXPRESSED_INTERSTITIAL, CoreConstants.Rate.ZERO_RATE),
            initialConcentrationParam(CoreConstants.Rate.INITIAL_CONCENTRATION_INTERSTITIAL)
         );
      }

      private ParameterRateMetaData fractionParam(string paramName, string rate,
         bool editable = true) =>
         new ParameterRateMetaData
         {
            ParameterName = paramName,
            Rate = rate,
            CalculationMethod = CoreConstants.CalculationMethod.EXPRESSION_PARAMETERS,
            BuildingBlockType = PKSimBuildingBlockType.Individual,
            CanBeVaried = true,
            CanBeVariedInPopulation = false,
            ReadOnly = !editable,
            Dimension = CoreConstants.Dimension.Fraction,
            GroupName = CoreConstants.Groups.RELATIVE_EXPRESSION,
            IsDefault = true,
            MinValue = 0,
            MaxValue = 1,
            MinIsAllowed = true,
            MaxIsAllowed = true,
         };

      private ParameterRateMetaData initialConcentrationParam(string rate) =>
         new ParameterRateMetaData
         {
            ParameterName = INITIAL_CONCENTRATION,
            Rate = rate,
            CalculationMethod = CoreConstants.CalculationMethod.EXPRESSION_PARAMETERS,
            BuildingBlockType = PKSimBuildingBlockType.Individual,
            CanBeVaried = true,
            CanBeVariedInPopulation = true,
            Dimension = MOLAR_CONCENTRATION,
            GroupName = CoreConstants.Groups.RELATIVE_EXPRESSION,
            MinValue = 0,
            MinIsAllowed =  true
         };

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
               initialConcentrationParam(CoreConstants.Rate.INITIAL_CONCENTRATION_LUMEN));
         }
      }

      protected virtual MoleculeExpressionContainer AddContainerExpression(IContainer parentContainer, string moleculeName,
         params ParameterMetaData[] parameters)
      {
         var expressionContainer = _objectBaseFactory.Create<MoleculeExpressionContainer>()
            .WithName(moleculeName)
            .WithParentContainer(parentContainer);
         parameters.Each(p => AddParameterIn(expressionContainer, p, moleculeName));
         return expressionContainer;
      }

      protected void AddVascularSystemExpression(IContainer moleculeContainer, params ParameterMetaData[] parameters)
      {
         parameters.Each(p => AddParameterIn(moleculeContainer, p, moleculeContainer.Name));
      }
   }
}