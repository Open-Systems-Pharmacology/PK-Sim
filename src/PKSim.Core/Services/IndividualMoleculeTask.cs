using System;
using OSPSuite.Assets;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Utility;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
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

      public virtual IndividualMolecule CreateEmpty() => CreateEmptyMolecule();

      public abstract IndividualMolecule AddMoleculeTo(ISimulationSubject simulationSubject, string moleculeName);

      protected ParameterValueMetaData RelExpParam(string paramName) => new ParameterValueMetaData
      {
         ParameterName = paramName,
         Dimension = Constants.Dimension.DIMENSIONLESS,
         DefaultValue = 0,
         GroupName = CoreConstants.Groups.RELATIVE_EXPRESSION,
         BuildingBlockType = PKSimBuildingBlockType.Individual,
         IsInput = true,
      };


      protected TMolecule CreateEmptyMolecule()
      {
         var molecule = _objectBaseFactory.Create<TMolecule>().WithIcon(Icon.IconName);
         CreateMoleculeParameterIn(molecule, CoreConstants.Parameters.REFERENCE_CONCENTRATION, CoreConstants.DEFAULT_REFERENCE_CONCENTRATION_VALUE,
            Constants.Dimension.MOLAR_CONCENTRATION);
         CreateMoleculeParameterIn(molecule, CoreConstants.Parameters.HALF_LIFE_LIVER, CoreConstants.DEFAULT_MOLECULE_HALF_LIFE_LIVER_VALUE_IN_MIN,
            Constants.Dimension.TIME);
         CreateMoleculeParameterIn(molecule, CoreConstants.Parameters.HALF_LIFE_INTESTINE,
            CoreConstants.DEFAULT_MOLECULE_HALF_LIFE_INTESTINE_VALUE_IN_MIN, Constants.Dimension.TIME);

         foreach (var parameterName in CoreConstants.Parameters.OntogenyFactors)
         {
            CreateMoleculeParameterIn(molecule, parameterName, 1, Constants.Dimension.DIMENSIONLESS, CoreConstants.Groups.ONTOGENY_FACTOR,
               canBeVariedInPopulation: false);
         }

         return molecule;
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