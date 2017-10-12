using System;
using System.Linq;
using OSPSuite.Assets;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Utility;
using OSPSuite.Utility.Extensions;

namespace PKSim.Core.Model
{
   public interface IIndividualMoleculeFactory : ISpecification<Type>
   {
      /// <summary>
      ///    Returns a <see cref="IndividualMolecule" /> filled with the containes defined in the
      ///    <paramref name="simulationSubject" />
      /// </summary>
      IndividualMolecule CreateFor(ISimulationSubject simulationSubject);

      /// <summary>
      ///    Returns an empty <see cref="IndividualMolecule" />  (only parameters are defined in the protein, no protein
      ///    container)
      /// </summary>
      IndividualMolecule CreateEmpty();
   }


   public abstract class IndividualMoleculeFactory<TMolecule, TMoleculeExpressionContainer> : IIndividualMoleculeFactory
      where TMolecule : IndividualMolecule
      where TMoleculeExpressionContainer : MoleculeExpressionContainer
   {
      private readonly IEntityPathResolver _entityPathResolver;
      protected readonly IObjectBaseFactory _objectBaseFactory;
      private readonly IObjectPathFactory _objectPathFactory;
      protected readonly IParameterFactory _parameterFactory;

      protected IndividualMoleculeFactory(IObjectBaseFactory objectBaseFactory, IParameterFactory parameterFactory, IObjectPathFactory objectPathFactory, IEntityPathResolver entityPathResolver)
      {
         _objectBaseFactory = objectBaseFactory;
         _parameterFactory = parameterFactory;
         _objectPathFactory = objectPathFactory;
         _entityPathResolver = entityPathResolver;
      }

      public bool IsSatisfiedBy(Type item)
      {
         return item.IsAnImplementationOf<TMolecule>();
      }

      public abstract IndividualMolecule CreateFor(ISimulationSubject simulationSubject);

      protected abstract ApplicationIcon Icon { get; }

      public virtual IndividualMolecule CreateEmpty()
      {
         return CreateEmptyMolecule();
      }

      protected void AddTissueOrgansExpression(ISimulationSubject simulationSubject, TMolecule molecule)
      {
         foreach (var container in simulationSubject.Organism.NonGITissueContainers)
         {
            AddContainerExpression(simulationSubject, molecule, container, CoreConstants.Groups.ORGANS_AND_TISSUES);
         }

         foreach (var organ in simulationSubject.Organism.GITissueContainers)
         {
            AddContainerExpression(simulationSubject, molecule, organ, CoreConstants.Groups.GI_NON_MUCOSA_TISSUE);
         }
      }

      protected TMolecule CreateEmptyMolecule()
      {
         var molecule = _objectBaseFactory.Create<TMolecule>().WithIcon(Icon.IconName);
         createMoleculeParameterIn(molecule, CoreConstants.Parameter.REFERENCE_CONCENTRATION, CoreConstants.DEFAULT_REFERENCE_CONCENTRATION_VALUE, Constants.Dimension.MOLAR_CONCENTRATION);
         createMoleculeParameterIn(molecule, CoreConstants.Parameter.HALF_LIFE_LIVER, CoreConstants.DEFAULT_MOLECULE_HALF_LIFE_LIVER_VALUE_IN_MIN, Constants.Dimension.TIME);
         createMoleculeParameterIn(molecule, CoreConstants.Parameter.HALF_LIFE_INTESTINE, CoreConstants.DEFAULT_MOLECULE_HALF_LIFE_INTESTINE_VALUE_IN_MIN, Constants.Dimension.TIME);

         foreach (var parameterName in CoreConstants.Parameter.OntogenyFactors)
         {
            createMoleculeParameterIn(molecule, parameterName, 1, Constants.Dimension.DIMENSIONLESS, CoreConstants.Groups.ONTOGENY_FACTOR, canBeVariedInPopulation: false);
         }

         return molecule;
      }

      private IParameter createMoleculeParameterIn(IContainer parameteContainer, string parameterName, double defaultValue, string dimensionName,
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
         parameteContainer.Add(parameter);
         if (displayUnit != null)
            parameter.DisplayUnit = parameter.Dimension.Unit(displayUnit);

         return parameter;
      }

      private TMoleculeExpressionContainer addContainerExpression(TMolecule protein, IContainer container, string name, string groupingName)
      {
         var expressionContainer = addContainerExpression(protein, name, groupingName);
         expressionContainer.OrganPath = _entityPathResolver.ObjectPathFor(container);
         return expressionContainer;
      }

      protected void AddMucosaExpression(ISimulationSubject simulationSubject, TMolecule molecule)
      {
         foreach (var organ in simulationSubject.Organism.OrgansByName(CoreConstants.Organ.SmallIntestine, CoreConstants.Organ.LargeIntestine))
         {
            var organMucosa = organ.Compartment(CoreConstants.Compartment.Mucosa);
            foreach (var compartment in organMucosa.GetChildren<Compartment>().Where(c => c.Visible))
            {
               AddContainerExpression(simulationSubject, molecule, compartment, CoreConstants.Groups.GI_MUCOSA);
            }
         }
      }

      protected void AddLumenExpressions(ISimulationSubject simulationSubject, TMolecule protein)
      {
         var lumen = simulationSubject.Organism.Organ(CoreConstants.Organ.Lumen);
         foreach (var segment in lumen.Compartments.Where(c => c.Visible))
         {
            var container = addContainerExpression(protein, lumen, CoreConstants.ContainerName.LumenSegmentNameFor(segment.Name), CoreConstants.Groups.GI_LUMEN);
            container.ContainerName = segment.Name;
         }
      }

      protected virtual TMoleculeExpressionContainer AddContainerExpression(ISimulationSubject simulationSubject, TMolecule protein, IContainer container, string groupingName)
      {
         return addContainerExpression(protein, container, container.Name, groupingName);
      }

      protected TMoleculeExpressionContainer AddVascularSystemExpression(TMolecule protein, string surrogateName)
      {
         var expressionContainer = addContainerExpression(protein, surrogateName, CoreConstants.Groups.VASCULAR_SYSTEM);
         expressionContainer.OrganPath = _objectPathFactory.CreateObjectPathFrom(surrogateName);
         return expressionContainer;
      }

      private TMoleculeExpressionContainer addContainerExpression(TMolecule protein, string containerName, string groupingName)
      {
         var expressionContainer = createContainerExpressionFor(protein, containerName);
         expressionContainer.GroupName = groupingName;
         expressionContainer.ContainerName = containerName;
         createMoleculeParameterIn(expressionContainer, CoreConstants.Parameter.REL_EXP, 0, Constants.Dimension.DIMENSIONLESS);
         createMoleculeParameterIn(expressionContainer, CoreConstants.Parameter.REL_EXP_NORM, 0, CoreConstants.Dimension.Fraction);
         return expressionContainer;
      }

      private TMoleculeExpressionContainer createContainerExpressionFor(TMolecule protein, string containerName)
      {
         var container = _objectBaseFactory.Create<TMoleculeExpressionContainer>().WithName(containerName);
         protein.Add(container);
         return container;
      }
   }
}