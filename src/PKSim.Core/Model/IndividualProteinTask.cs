using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Extensions;
using PKSim.Core.Services;
using static OSPSuite.Core.Domain.Constants.Dimension;
using static PKSim.Core.CoreConstants.Parameters;
using FormulaCache = OSPSuite.Core.Domain.Formulas.FormulaCache;

namespace PKSim.Core.Model
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
         IIndividualPathWithRootExpander individualPathWithRootExpander) : base(objectBaseFactory, parameterFactory, objectPathFactory, entityPathResolver)
      {
         _individualPathWithRootExpander = individualPathWithRootExpander;
      }

      public override IndividualMolecule CreateFor(ISimulationSubject simulationSubject)
      {
         var protein = CreateEmptyMolecule();

         AddVascularSystemExpression(protein, CoreConstants.Compartment.BloodCells);
         AddVascularSystemExpression(protein, CoreConstants.Compartment.Plasma);
         AddVascularSystemExpression(protein, CoreConstants.Compartment.VascularEndothelium);

         AddTissueOrgansExpression(simulationSubject, protein);
         AddLumenExpressions(simulationSubject, protein);
         AddMucosaExpression(simulationSubject, protein);

         protein.TissueLocation = TissueLocation.Intracellular;

         return protein;
      }

      protected void AddTissueOrgansExpressionNew(ISimulationSubject simulationSubject, string moleculeName)
      {
         foreach (var container in simulationSubject.Organism.NonGITissueContainers)
         {
            addTissueParameters(container, moleculeName, CoreConstants.Groups.ORGANS_AND_TISSUES);
         }

         foreach (var organ in simulationSubject.Organism.GITissueContainers)
         {
            addTissueParameters(organ, moleculeName, CoreConstants.Groups.GI_NON_MUCOSA_TISSUE);
         }
      }

      private void addTissueParameters(IContainer organ, string moleculeName, string groupName)
      {
         AddContainerExpressionNew(organ.Container(CoreConstants.Compartment.BloodCells), organ.Name, moleculeName, groupName,
            initialConcParam(CoreConstants.Rate.INITIAL_CONCENTRATION_BLOOD_CELLS)
         );

         AddContainerExpressionNew(organ.Container(CoreConstants.Compartment.Intracellular), organ.Name, moleculeName, groupName,
            relExpParam(REL_EXP),
            fractionParam(FRACTION_EXPRESSED_INTRACELLULAR, CoreConstants.Rate.ZERO_RATE),
            initialConcParam(CoreConstants.Rate.INITIAL_CONCENTRATION_INTRACELLULAR)
         );

         AddContainerExpressionNew(organ.Container(CoreConstants.Compartment.Interstitial), organ.Name, moleculeName, groupName,
            fractionParam(FRACTION_EXPRESSED_INTERSTITIAL, CoreConstants.Rate.ZERO_RATE),
            initialConcParam(CoreConstants.Rate.INITIAL_CONCENTRATION_INTERSTITIAL)
         );
      }

      private ParameterMetaData relExpParam(string paramName) => new ParameterValueMetaData
         {ParameterName = paramName, Dimension = DIMENSIONLESS, DefaultValue = 0, GroupName = CoreConstants.Groups.RELATIVE_EXPRESSION};

      private ParameterRateMetaData fractionParam(string paramName, string rate, string groupName = CoreConstants.Groups.RELATIVE_EXPRESSION) =>
         new ParameterRateMetaData
         {
            ParameterName = paramName,
            Rate = rate,
            CalculationMethod = CoreConstants.CalculationMethod.EXPRESSION_PARAMETERS,
            CanBeVaried = true,
            CanBeVariedInPopulation = true,
            Dimension = CoreConstants.Dimension.Fraction,
            GroupName = groupName
         };

      private ParameterRateMetaData initialConcParam(string rate) =>
         new ParameterRateMetaData
         {
            ParameterName = INITIAL_CONCENTRATION,
            Rate = rate,
            CalculationMethod = CoreConstants.CalculationMethod.EXPRESSION_PARAMETERS,
            CanBeVaried = true,
            CanBeVariedInPopulation = true,
            Dimension = CoreConstants.Dimension.Fraction,
            GroupName = CoreConstants.Groups.RELATIVE_EXPRESSION
         };


      protected IParameter createFormulaParameterIn(IContainer parameterContainer, ParameterRateMetaData parameterRateMetaData, string moleculeName,
         string groupName = null)
      {
         var parameter = _parameterFactory.CreateFor(parameterRateMetaData, new FormulaCache());
         parameterContainer.Add(parameter);
         if (parameterRateMetaData.DefaultUnit != null)
            parameter.DisplayUnit = parameter.Dimension.Unit(parameterRateMetaData.DefaultUnit);

         if (!string.IsNullOrEmpty(groupName))
            parameter.GroupName = groupName;

         parameter.Formula.ReplaceKeywordsInObjectPaths(new[] {ObjectPathKeywords.MOLECULE}, new[] {moleculeName});
         return parameter;
      }

      public override IndividualMolecule AddMoleculeTo(ISimulationSubject simulationSubject, string moleculeName)
      {
         var globalContainer = CreateGlobalMoleculeContainer(moleculeName);
         AddVascularSystemExpressionNew(globalContainer, CoreConstants.Groups.VASCULAR_SYSTEM,
            relExpParam(REL_EXP_BLOOD_CELL),
            fractionParam(FRACTION_EXPRESSED_BLOOD_CELLS, CoreConstants.Rate.ZERO_RATE),
            fractionParam(FRACTION_EXPRESSED_BLOOD_CELLS_MEMBRANE, CoreConstants.Rate.ZERO_RATE)
         );

         AddVascularSystemExpressionNew(globalContainer, CoreConstants.Groups.VASCULAR_SYSTEM, relExpParam(REL_EXP_PLASMA));

         AddVascularSystemExpressionNew(globalContainer, CoreConstants.Groups.VASCULAR_SYSTEM,
            relExpParam(REL_EXP_VASC_ENDO),
            fractionParam(FRACTION_EXPRESSED_ENDOSOME, CoreConstants.Rate.ZERO_RATE),
            fractionParam(FRACTION_EXPRESSED_PLASMA_SIDE_APICAL, CoreConstants.Rate.ZERO_RATE),
            fractionParam(FRACTION_EXPRESSED_TISSUE_SIDE_BASOLATERAL, CoreConstants.Rate.ZERO_RATE)
         );

         AddTissueOrgansExpressionNew(simulationSubject, moleculeName);
         AddLumenExpressionsNew(simulationSubject, moleculeName);
         AddMucosaExpressionNew(simulationSubject, moleculeName);

         simulationSubject.AddGlobalMolecule(globalContainer);

         _individualPathWithRootExpander.AddRootToPathIn(simulationSubject, moleculeName);
         return globalContainer;
      }

      protected void AddMucosaExpressionNew(ISimulationSubject simulationSubject, string moleculeName)
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

      protected void AddLumenExpressionsNew(ISimulationSubject simulationSubject, string moleculeName)
      {
         var lumen = simulationSubject.Organism.Organ(CoreConstants.Organ.Lumen);
         foreach (var segment in lumen.Compartments.Where(c => c.Visible))
         {
            AddContainerExpressionNew(segment, segment.Name, moleculeName, CoreConstants.Groups.GI_LUMEN, 
               relExpParam(REL_EXP), 
               initialConcParam(CoreConstants.Rate.ZERO_RATE));
         }
      }

      protected virtual MoleculeExpressionContainer AddContainerExpressionNew(IContainer parentContainer, string containerName, string moleculeName,
         string groupingName,
         params ParameterMetaData[] parameters)
      {
         return addContainerExpressionNew(parentContainer, containerName, moleculeName, groupingName, parameters);
      }

      protected IParameter createMoleculeParameterInNew(IContainer parameterContainer, string parameterName, double defaultValue,
         string dimensionName,
         string groupName = CoreConstants.Groups.RELATIVE_EXPRESSION,
         bool canBeVaried = true,
         bool canBeVariedInPopulation = true,
         bool visible = true,
         string displayUnit = null)
      {
         var parameter = _parameterFactory.CreateFor(parameterName, defaultValue, dimensionName, PKSimBuildingBlockType.Individual);
         parameter.GroupName = groupName;
         parameter.CanBeVaried = canBeVaried;
         parameter.CanBeVariedInPopulation = canBeVariedInPopulation;
         parameter.Visible = visible;
         parameterContainer.Add(parameter);
         if (displayUnit != null)
            parameter.DisplayUnit = parameter.Dimension.Unit(displayUnit);

         return parameter;
      }

      private MoleculeExpressionContainer addContainerExpressionNew(IContainer parentContainer, string containerName, string moleculeName,
         string groupingName,
         IEnumerable<ParameterMetaData> parameters)
      {
         var expressionContainer = createContainerExpressionForNew(parentContainer, moleculeName);
         expressionContainer.GroupName = groupingName;
         expressionContainer.ContainerName = containerName;
         parameters.Each(p => addParameterIn(expressionContainer, p, moleculeName));
         return expressionContainer;
      }

      protected void addParameterIn(IContainer container, ParameterMetaData parameterMetaData, string moleculeName, string groupName = null)
      {
         switch (parameterMetaData)
         {
            case ParameterRateMetaData rateMetaData:
               createFormulaParameterIn(container, rateMetaData, moleculeName, groupName);
               break;
            default:
               createMoleculeParameterInNew(container, parameterMetaData.ParameterName, 0, parameterMetaData.Dimension, groupName);
               break;
         }

      }
      protected void AddVascularSystemExpressionNew(IContainer moleculeContainer, string groupName, params ParameterMetaData[] parameters)
      {
         parameters.Each(p => addParameterIn(moleculeContainer, p, moleculeContainer.Name, groupName));
      }

      private MoleculeExpressionContainer createContainerExpressionForNew(IContainer parentContainer, string containerName)
      {
         var container = _objectBaseFactory.Create<MoleculeExpressionContainer>().WithName(containerName);
         parentContainer.Add(container);
         return container;
      }

      protected TMolecule CreateGlobalMoleculeContainer(string moleculeName)
      {
         var molecule = _objectBaseFactory.Create<TMolecule>().WithIcon(Icon.IconName).WithName(moleculeName);
         createMoleculeParameterIn(molecule, REFERENCE_CONCENTRATION, CoreConstants.DEFAULT_REFERENCE_CONCENTRATION_VALUE, MOLAR_CONCENTRATION);
         createMoleculeParameterIn(molecule, HALF_LIFE_LIVER, CoreConstants.DEFAULT_MOLECULE_HALF_LIFE_LIVER_VALUE_IN_MIN, TIME);
         createMoleculeParameterIn(molecule, HALF_LIFE_INTESTINE, CoreConstants.DEFAULT_MOLECULE_HALF_LIFE_INTESTINE_VALUE_IN_MIN, TIME);

         foreach (var parameterName in OntogenyFactors)
         {
            createMoleculeParameterIn(molecule, parameterName, 1, DIMENSIONLESS, CoreConstants.Groups.ONTOGENY_FACTOR,
               canBeVariedInPopulation: false);
         }

         return molecule;
      }
   }
}