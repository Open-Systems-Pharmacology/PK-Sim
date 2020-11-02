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

         AddVascularSystemExpression(molecule, CoreConstants.Groups.VASCULAR_SYSTEM,
            RelExpParam(REL_EXP_BLOOD_CELLS),
            fractionParam(FRACTION_EXPRESSED_BLOOD_CELLS, CoreConstants.Rate.ZERO_RATE),
            fractionParam(FRACTION_EXPRESSED_BLOOD_CELLS_MEMBRANE, CoreConstants.Rate.PARAM_F_EXP_BC_MEMBRANE, editable: false)
         );

         AddVascularSystemExpression(molecule, CoreConstants.Groups.VASCULAR_SYSTEM, RelExpParam(REL_EXP_PLASMA));

         AddVascularSystemExpression(molecule, CoreConstants.Groups.VASCULAR_SYSTEM,
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
         organism.NonGITissueContainers.Each(x => AddTissueParameters(x, moleculeName, CoreConstants.Groups.ORGANS_AND_TISSUES));
         organism.GITissueContainers.Each(x => AddTissueParameters(x, moleculeName, CoreConstants.Groups.GI_NON_MUCOSA_TISSUE));
      }

      protected void AddTissueParameters(IContainer organ, string moleculeName, string groupName)
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
            IsDefault = true
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

      protected void AddMucosaExpression(ISimulationSubject simulationSubject, string moleculeName)
      {
         foreach (var organ in simulationSubject.Organism.OrgansByName(CoreConstants.Organ.SmallIntestine, CoreConstants.Organ.LargeIntestine))
         {
            var organMucosa = organ.Compartment(CoreConstants.Compartment.Mucosa);
            foreach (var compartment in organMucosa.GetChildren<Compartment>().Where(c => c.Visible))
            {
               AddTissueParameters(compartment, moleculeName, CoreConstants.Groups.GI_MUCOSA);
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

   

      protected void AddVascularSystemExpression(IContainer moleculeContainer, string groupName, params ParameterMetaData[] parameters)
      {
         parameters.Each(p => AddParameterIn(moleculeContainer, p, moleculeContainer.Name, groupName));
      }
   }
}