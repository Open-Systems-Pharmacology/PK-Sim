using System;
using OSPSuite.Assets;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Utility;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Extensions;
using PKSim.Core.Model;
using static PKSim.Core.CoreConstants.Parameters;
using static OSPSuite.Core.Domain.Constants.Dimension;
using FormulaCache = OSPSuite.Core.Domain.Formulas.FormulaCache;
using IParameterFactory = PKSim.Core.Model.IParameterFactory;

namespace PKSim.Core.Services
{
   public interface IIndividualMoleculeTask : ISpecification<Type>
   {
      /// <summary>
      ///    Returns an empty <see cref="IndividualMolecule" />  (only parameters are defined in the protein, no protein
      ///    container)
      /// </summary>
      IndividualMolecule CreateEmpty();

      IndividualMolecule AddMoleculeTo(ISimulationSubject simulationSubject, string moleculeName);
   }

   public abstract class IndividualMoleculeTask<TMolecule, TMoleculeExpressionContainer> : IIndividualMoleculeTask
      where TMolecule : IndividualMolecule
      where TMoleculeExpressionContainer : MoleculeExpressionContainer
   {
      protected readonly IEntityPathResolver _entityPathResolver;
      protected readonly IObjectBaseFactory _objectBaseFactory;
      protected readonly IObjectPathFactory _objectPathFactory;
      protected readonly IParameterFactory _parameterFactory;

      protected IndividualMoleculeTask(
         IObjectBaseFactory objectBaseFactory,
         IParameterFactory parameterFactory,
         IObjectPathFactory objectPathFactory,
         IEntityPathResolver entityPathResolver)
      {
         _objectBaseFactory = objectBaseFactory;
         _parameterFactory = parameterFactory;
         _objectPathFactory = objectPathFactory;
         _entityPathResolver = entityPathResolver;
      }

      public bool IsSatisfiedBy(Type item) => item.IsAnImplementationOf<TMolecule>();

      protected abstract ApplicationIcon Icon { get; }

      public virtual IndividualMolecule CreateEmpty() => CreateMolecule(string.Empty);

      public abstract IndividualMolecule AddMoleculeTo(ISimulationSubject simulationSubject, string moleculeName);

      protected ParameterValueMetaData RelExpParam(string paramName) => new ParameterValueMetaData
      {
         ParameterName = paramName,
         Dimension = DIMENSIONLESS,
         DefaultValue = 0,
         GroupName = CoreConstants.Groups.RELATIVE_EXPRESSION,
         BuildingBlockType = PKSimBuildingBlockType.Individual,
         IsInput = true,
      };

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

      protected IParameter CreateFormulaParameterIn(
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
               CreateFormulaParameterIn(container, rateMetaData, moleculeName, groupName);
               break;
            case ParameterValueMetaData parameterValueMetaData:
               CreateConstantParameterIn(container, parameterValueMetaData, groupName);
               break;
         }
      }

      protected IParameter CreateConstantParameterIn(IContainer parameterContainer,
         ParameterValueMetaData parameterValueDefinition,
         string groupName = CoreConstants.Groups.RELATIVE_EXPRESSION)
      {
         var parameter = _parameterFactory.CreateFor(parameterValueDefinition);
         parameterContainer.Add(parameter);
         if (!string.IsNullOrEmpty(groupName))
            parameter.GroupName = groupName;

         return parameter;
      }

      protected IParameter CreateMoleculeParameterIn(IContainer parameterContainer, string parameterName, double defaultValue, string dimensionName,
         string groupName = CoreConstants.Groups.RELATIVE_EXPRESSION,
         bool canBeVaried = true,
         bool canBeVariedInPopulation = true,
         bool visible = true,
         string displayUnit = null,
         PKSimBuildingBlockType buildingBlockType = PKSimBuildingBlockType.Individual)
      {
         var parameterValue = new ParameterValueMetaData
         {
            ParameterName = parameterName,
            DefaultValue = defaultValue,
            Dimension = dimensionName,
            GroupName = groupName,
            CanBeVaried = canBeVaried,
            CanBeVariedInPopulation = canBeVariedInPopulation,
            Visible = visible,
            DefaultUnit = displayUnit,
            BuildingBlockType = buildingBlockType
         };

         return CreateConstantParameterIn(parameterContainer, parameterValue);
      }
   }
}