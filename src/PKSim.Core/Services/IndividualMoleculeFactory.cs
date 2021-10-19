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
   public interface IIndividualMoleculeFactory : ISpecification<Type>
   {
      /// <summary>
      ///    Returns an empty <see cref="IndividualMolecule" />  (only parameters are defined in the protein, no protein
      ///    container)
      /// </summary>
      IndividualMolecule CreateEmpty();

      IndividualMolecule AddMoleculeTo(ISimulationSubject simulationSubject, string moleculeName);
   }

   public abstract class IndividualMoleculeFactory<TMolecule, TMoleculeExpressionContainer> : IIndividualMoleculeFactory
      where TMolecule : IndividualMolecule
      where TMoleculeExpressionContainer : MoleculeExpressionContainer
   {
      protected readonly IEntityPathResolver _entityPathResolver;
      private readonly IIdGenerator _idGenerator;
      protected readonly IObjectBaseFactory _objectBaseFactory;
      protected readonly IObjectPathFactory _objectPathFactory;
      protected readonly IParameterFactory _parameterFactory;

      protected IndividualMoleculeFactory(
         IObjectBaseFactory objectBaseFactory,
         IParameterFactory parameterFactory,
         IObjectPathFactory objectPathFactory,
         IEntityPathResolver entityPathResolver,
         IIdGenerator idGenerator)
      {
         _objectBaseFactory = objectBaseFactory;
         _parameterFactory = parameterFactory;
         _objectPathFactory = objectPathFactory;
         _entityPathResolver = entityPathResolver;
         _idGenerator = idGenerator;
      }

      public bool IsSatisfiedBy(Type item) => item.IsAnImplementationOf<TMolecule>();

      protected abstract ApplicationIcon Icon { get; }

      public virtual IndividualMolecule CreateEmpty() => CreateMolecule(string.Empty);

      public abstract IndividualMolecule AddMoleculeTo(ISimulationSubject simulationSubject, string moleculeName);

      protected ParameterValueMetaData RelExpParam(string paramName, double defaultValue = 0) => new ParameterValueMetaData
      {
         ParameterName = paramName,
         Dimension = DIMENSIONLESS,
         DefaultValue = defaultValue,
         GroupName = CoreConstants.Groups.RELATIVE_EXPRESSION,
         BuildingBlockType = PKSimBuildingBlockType.Individual,
         IsDefault = true,
         MinValue = 0,
         MinIsAllowed = true
      };

      protected ParameterRateMetaData FractionParam(string paramName, string rate,
         bool editable = true, bool visible = true) =>
         new ParameterRateMetaData
         {
            ParameterName = paramName,
            Rate = rate,
            CalculationMethod = CoreConstants.CalculationMethod.EXPRESSION_PARAMETERS,
            BuildingBlockType = PKSimBuildingBlockType.Individual,
            CanBeVaried = true,
            CanBeVariedInPopulation = false,
            ReadOnly = !editable,
            Visible = visible,
            Dimension = CoreConstants.Dimension.Fraction,
            GroupName = CoreConstants.Groups.RELATIVE_EXPRESSION,
            IsDefault = true,
            MinValue = 0,
            MaxValue = 1,
            MinIsAllowed = true,
            MaxIsAllowed = true,
         };

      protected ParameterRateMetaData InitialConcentrationParam(string rate) =>
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
            MinIsAllowed = true
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

         //Because we update the formula of the parameter, we need to make sure we also reset the formulaId so that it will appear as being unique
         parameter.Formula.ReplaceKeywordsInObjectPaths(new[] {ObjectPathKeywords.MOLECULE}, new[] {moleculeName});
         parameter.Formula.Id = _idGenerator.NewId();

         //All constant parameters do not have default values
         parameter.DefaultValue = null;
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

         //All constant parameters do not have default values
         parameter.DefaultValue = null;
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

      protected void AddGlobalExpression(IContainer moleculeContainer, params ParameterMetaData[] parameters)
      {
         parameters.Each(p => AddParameterIn(moleculeContainer, p, moleculeContainer.Name));
      }

      protected virtual TMoleculeExpressionContainer AddContainerExpression(IContainer parentContainer, string moleculeName,
         params ParameterMetaData[] parameters)
      {
         var expressionContainer = _objectBaseFactory.Create<TMoleculeExpressionContainer>()
            .WithName(moleculeName)
            .WithParentContainer(parentContainer);
         parameters.Each(p => AddParameterIn(expressionContainer, p, moleculeName));
         return expressionContainer;
      }

   }
}