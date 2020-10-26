using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Extensions;
using PKSim.Core.Model;
using static OSPSuite.Core.Domain.Constants.Dimension;
using static PKSim.Core.CoreConstants.Parameters;
using FormulaCache = OSPSuite.Core.Domain.Formulas.FormulaCache;
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

      protected void AddTissueOrgansExpression(ISimulationSubject simulationSubject, string moleculeName)
      {
         var organism = simulationSubject.Organism;
         organism.NonGITissueContainers.Each(x=>addTissueParameters(x, moleculeName, CoreConstants.Groups.ORGANS_AND_TISSUES));
         organism.GITissueContainers.Each(x=>addTissueParameters(x, moleculeName, CoreConstants.Groups.GI_NON_MUCOSA_TISSUE));
      }

      private void addTissueParameters(IContainer organ, string moleculeName, string groupName)
      {
         AddContainerExpression(organ.Container(CoreConstants.Compartment.BloodCells), moleculeName, groupName,
            initialConcentrationParam(CoreConstants.Rate.INITIAL_CONCENTRATION_BLOOD_CELLS)
         );

         AddContainerExpression(organ.Container(CoreConstants.Compartment.Plasma), moleculeName, groupName,
            initialConcentrationParam(CoreConstants.Rate.INITIAL_CONCENTRATION_PLASMA)
         );

         AddContainerExpression(organ.Container(CoreConstants.Compartment.Endosome), moleculeName, groupName,
            initialConcentrationParam(CoreConstants.Rate.INITIAL_CONCENTRATION_ENDOSOME)
         );

         AddContainerExpression(organ.Container(CoreConstants.Compartment.Intracellular), moleculeName, groupName,
            RelExpParam(REL_EXP),
            fractionParam(FRACTION_EXPRESSED_INTRACELLULAR, CoreConstants.Rate.PARAM_F_EXP_INTRACELLULAR, editable: false),
            initialConcentrationParam(CoreConstants.Rate.INITIAL_CONCENTRATION_INTRACELLULAR)
         );

         AddContainerExpression(organ.Container(CoreConstants.Compartment.Interstitial), moleculeName, groupName,
            fractionParam(FRACTION_EXPRESSED_INTERSTITIAL, CoreConstants.Rate.ZERO_RATE),
            initialConcentrationParam(CoreConstants.Rate.INITIAL_CONCENTRATION_INTERSTITIAL)
         );
      }

      private ParameterRateMetaData fractionParam(string paramName, string rate, string groupName = CoreConstants.Groups.RELATIVE_EXPRESSION,
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
            GroupName = groupName,
            IsInput = editable
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
         };

      public override IndividualMolecule AddMoleculeTo(ISimulationSubject simulationSubject, string moleculeName)
      {
         var globalContainer = CreateMolecule(moleculeName);
         AddVascularSystemExpression(globalContainer, CoreConstants.Groups.VASCULAR_SYSTEM,
            RelExpParam(REL_EXP_BLOOD_CELLS),
            fractionParam(FRACTION_EXPRESSED_BLOOD_CELLS, CoreConstants.Rate.ZERO_RATE),
            fractionParam(FRACTION_EXPRESSED_BLOOD_CELLS_MEMBRANE, CoreConstants.Rate.PARAM_F_EXP_BC_MEMBRANE, editable: false)
         );

         AddVascularSystemExpression(globalContainer, CoreConstants.Groups.VASCULAR_SYSTEM, RelExpParam(REL_EXP_PLASMA));

         AddVascularSystemExpression(globalContainer, CoreConstants.Groups.VASCULAR_SYSTEM,
            RelExpParam(REL_EXP_VASC_ENDO),
            fractionParam(FRACTION_EXPRESSED_VASC_ENDO_ENDOSOME, CoreConstants.Rate.ZERO_RATE),
            fractionParam(FRACTION_EXPRESSED_VASC_ENDO_APICAL, CoreConstants.Rate.ZERO_RATE),
            fractionParam(FRACTION_EXPRESSED_VASC_ENDO_BASOLATERAL, CoreConstants.Rate.PARAM_F_EXP_VASC_BASOLATERAL, editable: false)
         );

         AddTissueOrgansExpression(simulationSubject, moleculeName);
         AddLumenExpression(simulationSubject, moleculeName);
         AddMucosaExpression(simulationSubject, moleculeName);

         simulationSubject.AddMolecule(globalContainer);

         _individualPathWithRootExpander.AddRootToPathIn(simulationSubject, moleculeName);
         return globalContainer;
      }

      protected void AddMucosaExpression(ISimulationSubject simulationSubject, string moleculeName)
      {
         foreach (var organ in simulationSubject.Organism.OrgansByName(CoreConstants.Organ.SmallIntestine, CoreConstants.Organ.LargeIntestine))
         {
            var organMucosa = organ.Compartment(CoreConstants.Compartment.Mucosa);
            foreach (var compartment in organMucosa.GetChildren<Compartment>().Where(c => c.Visible))
            {
               addTissueParameters(compartment, moleculeName, CoreConstants.Groups.GI_MUCOSA);
            }
         }
      }

      protected void AddLumenExpression(ISimulationSubject simulationSubject, string moleculeName)
      {
         var lumen = simulationSubject.Organism.Organ(CoreConstants.Organ.Lumen);
         foreach (var segment in lumen.Compartments.Where(c => c.Visible))
         {
            AddContainerExpression(segment, moleculeName, CoreConstants.Groups.GI_LUMEN,
               RelExpParam(REL_EXP),
               initialConcentrationParam(CoreConstants.Rate.INITIAL_CONCENTRATION_LUMEN));
         }
      }

      protected virtual MoleculeExpressionContainer AddContainerExpression(IContainer parentContainer, string moleculeName, string groupingName,
         params ParameterMetaData[] parameters)
      {
         var expressionContainer = _objectBaseFactory.Create<MoleculeExpressionContainer>()
            .WithName(moleculeName)
            .WithParentContainer(parentContainer);
         expressionContainer.GroupName = groupingName;
         parameters.Each(p => AddParameterIn(expressionContainer, p, moleculeName));
         return expressionContainer;
      }

      private IParameter createFormulaParameterIn(
         IContainer parameterContainer,
         ParameterRateMetaData parameterRateMetaData,
         string moleculeName,
         string groupName = null)
      {
         var parameter = _parameterFactory.CreateFor(parameterRateMetaData, new FormulaCache());
         parameterContainer.Add(parameter);

         if (!string.IsNullOrEmpty(groupName))
            parameter.GroupName = groupName;

         parameter.Formula.ReplaceKeywordsInObjectPaths(new[] {ObjectPathKeywords.MOLECULE}, new[] {moleculeName});
         return parameter;
      }

      protected void AddParameterIn(IContainer container, ParameterMetaData parameterMetaData, string moleculeName, string groupName = null)
      {
         switch (parameterMetaData)
         {
            case ParameterRateMetaData rateMetaData:
               createFormulaParameterIn(container, rateMetaData, moleculeName, groupName);
               break;
            case ParameterValueMetaData parameterValueMetaData:
               CreateConstantParameterIn(container, parameterValueMetaData, groupName);
               break;
         }
      }

      protected void AddVascularSystemExpression(IContainer moleculeContainer, string groupName, params ParameterMetaData[] parameters)
      {
         parameters.Each(p => AddParameterIn(moleculeContainer, p, moleculeContainer.Name, groupName));
      }

      protected TMolecule CreateMolecule(string moleculeName)
      {
         var molecule = _objectBaseFactory.Create<TMolecule>().WithIcon(Icon.IconName).WithName(moleculeName);
         CreateMoleculeParameterIn(molecule, REFERENCE_CONCENTRATION, CoreConstants.DEFAULT_REFERENCE_CONCENTRATION_VALUE, MOLAR_CONCENTRATION);
         CreateMoleculeParameterIn(molecule, HALF_LIFE_LIVER, CoreConstants.DEFAULT_MOLECULE_HALF_LIFE_LIVER_VALUE_IN_MIN, TIME);
         CreateMoleculeParameterIn(molecule, HALF_LIFE_INTESTINE, CoreConstants.DEFAULT_MOLECULE_HALF_LIFE_INTESTINE_VALUE_IN_MIN, TIME);

         OntogenyFactors.Each(parameterName => CreateMoleculeParameterIn(molecule, parameterName, 1, DIMENSIONLESS,
            CoreConstants.Groups.ONTOGENY_FACTOR,
            canBeVariedInPopulation: false));

         return molecule;
      }
   }
}