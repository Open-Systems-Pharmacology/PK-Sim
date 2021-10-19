using System;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Core.Domain.UnitSystem;

namespace PKSim.Core.Services
{
   public abstract class InteractionKineticUpdaterSpecificationBase : IInteractionKineticUpdaterSpecification
   {
      protected readonly IObjectPathFactory _objectPathFactory;
      protected readonly IDimensionRepository _dimensionRepository;
      private readonly IInteractionTask _interactionTask;
      private readonly InteractionType _interactionType;
      private readonly InteractionKineticModifications _kineticModifications;
      private readonly string _kiNumeratorParameter;
      private readonly string _kiDenominatorParameter;
      private readonly string _kiNumeratorAlias;
      private readonly string _kiDenominatorAlias;
      private readonly string _kWaterAlias;
      private readonly string _inhibitorAlias;

      protected InteractionKineticUpdaterSpecificationBase(IObjectPathFactory objectPathFactory, IDimensionRepository dimensionRepository, IInteractionTask interactionTask, InteractionType interactionType,
         string kiNumeratorParameter, string kiDenominatorParameter, string kiNumeratorAlias, string kiDenominatorAlias, string inhibitorAlias, string kWaterAlias)
      {
         _objectPathFactory = objectPathFactory;
         _dimensionRepository = dimensionRepository;
         _interactionTask = interactionTask;
         _interactionType = interactionType;
         _kineticModifications = modificationsFrom(interactionType);
         _kiNumeratorParameter = kiNumeratorParameter;
         _kiDenominatorParameter = kiDenominatorParameter;
         _kiNumeratorAlias = kiNumeratorAlias;
         _kiDenominatorAlias = kiDenominatorAlias;
         _inhibitorAlias = inhibitorAlias;
         _kWaterAlias = kWaterAlias;
      }

      public bool UpdateRequiredFor(string moleculeName, string compoundName, Simulation simulation)
      {
         return AllInteractionProcessesFor(moleculeName, compoundName, simulation).Any();
      }

      public void UpdateKmFactorReferences(IParameter kmFactor, string moleculeName, string compoundName, Simulation simulation, IContainer processParameterContainer)
      {
         updateReferences(kmFactor, moleculeName, compoundName, simulation, processParameterContainer, InteractionKineticModifications.KmNumerator);
         updateReferences(kmFactor, moleculeName, compoundName, simulation, processParameterContainer, InteractionKineticModifications.KmDenominator);
      }

      public void UpdateKcatFactorReferences(IParameter vmaxFactor, string moleculeName, string compoundName, Simulation simulation, IContainer processParameterContainer)
      {
         updateReferences(vmaxFactor, moleculeName, compoundName, simulation, processParameterContainer, InteractionKineticModifications.KcatDenominator);
      }

      public void UpdateCLSpecFactorReferences(IParameter clspecFactor, string moleculeName, string compoundName, Simulation simulation, IContainer processParameterContainer)
      {
         updateReferences(clspecFactor, moleculeName, compoundName, simulation, processParameterContainer, InteractionKineticModifications.CLSpecDenominator);
      }

      public void UpdateModifiers(IReactionBuilder reaction, string moleculeName, string compoundName, Simulation simulation)
      {
         AllInteractionProcessesFor(moleculeName, compoundName, simulation).Each((process, i) =>
         {
            var inhibitor = process.ParentCompound;
            if (!reaction.ModifierNames.Contains(inhibitor.Name))
               reaction.AddModifier(inhibitor.Name);
         });
      }

      public string KmNumeratorTerm(string moleculeName, string compoundName, Simulation simulation)
      {
         return inhibitionTermsFor(moleculeName, compoundName, simulation, InteractionKineticModifications.KmNumerator);
      }

      public string CLSpecDenominatorTerm(string moleculeName, string compoundName, Simulation simulation)
      {
         return inhibitionTermsFor(moleculeName, compoundName, simulation, InteractionKineticModifications.CLSpecDenominator);
      }

      public string KmDenominatorTerm(string moleculeName, string compoundName, Simulation simulation)
      {
         return inhibitionTermsFor(moleculeName, compoundName, simulation, InteractionKineticModifications.KmDenominator);
      }

      public string KcatDenominatorTerm(string moleculeName, string compoundName, Simulation simulation)
      {
         return inhibitionTermsFor(moleculeName, compoundName, simulation, InteractionKineticModifications.KcatDenominator);
      }

      private string inhibitionTermsFor(string moleculeName, string compoundName, Simulation simulation, InteractionKineticModifications kineticModification)
      {
         if (!_kineticModifications.Is(kineticModification))
            return string.Empty;

         var kiAliasBase = kiAliasFrom(kineticModification);
         var inhibitionTerms = new List<string>();

         doOverAllInteractions(moleculeName, compoundName, simulation, kiAliasBase,
            (interactionProcess, inhibitorAlias, kiAlias, kwaterAlias) => inhibitionTerms.Add($"{kwaterAlias}*{inhibitorAlias}/{kiAlias}"));

         return inhibitionTerms.ToString(" + ");
      }

      private void updateReferences(IUsingFormula usingFormula, string moleculeName, string compoundName, Simulation simulation,
         IContainer processParameterContainer, InteractionKineticModifications kineticModification)
      {
         if (!_kineticModifications.Is(kineticModification))
            return;

         var formula = usingFormula.Formula;
         var kiAliasBase = kiAliasFrom(kineticModification);
         var kiParameter = kiParameterFrom(kineticModification);

         doOverAllInteractions(moleculeName, compoundName, simulation, kiAliasBase, (interactionProcess, inhibitorAlias, kiAlias, kwaterAlias) =>
         {
            var inhibitor = interactionProcess.ParentCompound;

            if (formula.FormulaUsablePathBy(inhibitorAlias) == null)
               formula.AddObjectPath(inhibitorConcentrationPath(inhibitor, inhibitorAlias, processParameterContainer));

            if (formula.FormulaUsablePathBy(kwaterAlias) == null)
               formula.AddObjectPath(kwaterPath(inhibitor, kwaterAlias, processParameterContainer));

            if (formula.FormulaUsablePathBy(kiAlias) == null)
               formula.AddObjectPath(kiPath(kiParameter, inhibitor, interactionProcess, kiAlias));

         });
      }

      private void doOverAllInteractions(string moleculeName, string compoundName, Simulation simulation, string kiAliasBase, Action<InteractionProcess, string, string, string> action)
      {
         AllInteractionProcessesFor(moleculeName, compoundName, simulation).Each((interactionProcess, i) =>
         {
            var inhibitorIndex = i + 1;
            var inhibitorAlias = $"{_inhibitorAlias}{inhibitorIndex}";
            var kiAlias = $"{kiAliasBase}{inhibitorIndex}";
            var kwaterAlias = $"{_kWaterAlias}{inhibitorIndex}";
            action(interactionProcess, inhibitorAlias, kiAlias, kwaterAlias);
         });
      }

      protected virtual IReadOnlyList<InteractionProcess> AllInteractionProcessesFor(string moleculeName, string compoundName, Simulation simulation)
      {
         return _interactionTask.AllInteractionProcessesFor(moleculeName, _interactionType, simulation, compoundName);
      }

      private IFormulaUsablePath kiPath(string kiParameter, Compound inhibitor, InteractionProcess process, string kiAlias)
      {
         return _objectPathFactory.CreateFormulaUsablePathFrom(inhibitor.Name, process.Name, kiParameter)
            .WithAlias(kiAlias)
            .WithDimension(_dimensionRepository.MolarConcentration);
      }

      private IFormulaUsablePath inhibitorConcentrationPath(Compound inhibitor, string inhibitorAlias, IContainer processParameterContainer)
      {
         return localInhibitorParameterPath(inhibitor, Constants.Parameters.CONCENTRATION, inhibitorAlias, processParameterContainer, _dimensionRepository.MolarConcentration);
      }

      private IFormulaUsablePath kwaterPath(Compound inhibitor, string kwaterAlias, IContainer processParameterContainer)
      {
         return localInhibitorParameterPath(inhibitor, CoreConstants.Parameters.K_WATER, kwaterAlias, processParameterContainer, _dimensionRepository.NoDimension);
      }

      private IFormulaUsablePath localInhibitorParameterPath(Compound inhibitor, string parameterName, string parameterAlias, IContainer processParameterContainer, IDimension dimension)
      {
         var objectPath = _objectPathFactory.CreateFormulaUsablePathFrom(inhibitor.Name, parameterName)
            .WithAlias(parameterAlias)
            .WithDimension(dimension);

         return processParameterContainer.IsAnImplementationOf<IReactionBuilder>()
            ? objectPath.AndAddAtFront(ObjectPath.PARENT_CONTAINER).AndAddAtFront(ObjectPath.PARENT_CONTAINER)
            : objectPath.AndAddAtFront(ObjectPathKeywords.SOURCE);
      }

      private static InteractionKineticModifications modificationsFrom(InteractionType interactionType)
      {
         return EnumHelper.ParseValue<InteractionKineticModifications>(interactionType.ToString());
      }

      private string kiAliasFrom(InteractionKineticModifications kineticModification)
      {
         return kiInfoFrom(kineticModification).kiAlias;
      }

      private string kiParameterFrom(InteractionKineticModifications kineticModification)
      {
         return kiInfoFrom(kineticModification).kiParameter;
      }

      private (string kiAlias, string kiParameter) kiInfoFrom(InteractionKineticModifications kineticModification)
      {
         switch (kineticModification)
         {
            case InteractionKineticModifications.KmDenominator:
            case InteractionKineticModifications.KcatDenominator:
               return (_kiDenominatorAlias, _kiDenominatorParameter);
            case InteractionKineticModifications.KmNumerator:
            case InteractionKineticModifications.CLSpecDenominator:
               return (_kiNumeratorAlias, _kiNumeratorParameter);
            default:
               throw new ArgumentOutOfRangeException(nameof(kineticModification));
         }
      }
   }
}