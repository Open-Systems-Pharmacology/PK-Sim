using System;
using System.Linq;
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
      // /// <summary>
      // ///    Returns a <see cref="IndividualMolecule" /> filled with the contains defined in the
      // ///    <paramref name="simulationSubject" />
      // /// </summary>
      // IndividualMolecule CreateFor(ISimulationSubject simulationSubject);

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

      public abstract IndividualMolecule AddMoleculeTo(ISimulationSubject simulationSubject, string moleculeName);

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
         createMoleculeParameterIn(molecule, CoreConstants.Parameters.REFERENCE_CONCENTRATION, CoreConstants.DEFAULT_REFERENCE_CONCENTRATION_VALUE,
            Constants.Dimension.MOLAR_CONCENTRATION);
         createMoleculeParameterIn(molecule, CoreConstants.Parameters.HALF_LIFE_LIVER, CoreConstants.DEFAULT_MOLECULE_HALF_LIFE_LIVER_VALUE_IN_MIN,
            Constants.Dimension.TIME);
         createMoleculeParameterIn(molecule, CoreConstants.Parameters.HALF_LIFE_INTESTINE,
            CoreConstants.DEFAULT_MOLECULE_HALF_LIFE_INTESTINE_VALUE_IN_MIN, Constants.Dimension.TIME);

         foreach (var parameterName in CoreConstants.Parameters.OntogenyFactors)
         {
            createMoleculeParameterIn(molecule, parameterName, 1, Constants.Dimension.DIMENSIONLESS, CoreConstants.Groups.ONTOGENY_FACTOR,
               canBeVariedInPopulation: false);
         }

         return molecule;
      }

      protected IParameter createMoleculeParameterIn(IContainer parameterContainer, string parameterName, double defaultValue, string dimensionName,
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

      private TMoleculeExpressionContainer addContainerExpression(TMolecule protein, IContainer container, string name, string groupingName)
      {
         var expressionContainer = addContainerExpression(protein, name, groupingName);
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
            var container = addContainerExpression(protein, lumen, CoreConstants.ContainerName.LumenSegmentNameFor(segment.Name),
               CoreConstants.Groups.GI_LUMEN);
         }
      }

      protected virtual TMoleculeExpressionContainer AddContainerExpression(ISimulationSubject simulationSubject, TMolecule protein,
         IContainer container, string groupingName)
      {
         return addContainerExpression(protein, container, container.Name, groupingName);
      }

      protected TMoleculeExpressionContainer AddVascularSystemExpression(TMolecule protein, string surrogateName)
      {
         var expressionContainer = addContainerExpression(protein, surrogateName, CoreConstants.Groups.VASCULAR_SYSTEM);
         return expressionContainer;
      }

      private TMoleculeExpressionContainer addContainerExpression(TMolecule protein, string containerName, string groupingName)
      {
         var expressionContainer = createContainerExpressionFor(protein, containerName);
         expressionContainer.GroupName = groupingName;
         createMoleculeParameterIn(expressionContainer, CoreConstants.Parameters.REL_EXP, 0, Constants.Dimension.DIMENSIONLESS);
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