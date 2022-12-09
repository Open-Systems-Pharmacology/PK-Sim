using System;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using static OSPSuite.Core.Domain.Constants;

namespace PKSim.Core.Services
{
   /// <summary>
   ///    Updates the kinetic of the given process (reaction, transport etc.) to take the inhibition defined in the simulation
   ///    into account.
   /// </summary>
   public interface IInteractionKineticUpdater
   {
      /// <summary>
      ///    Updates the given <paramref name="reaction" /> taking place between the <paramref name="compoundName" /> (e.g. Drug)
      ///    and the <paramref name="enzymeName" /> (e.g. CYP3A4) with the required interaction terms
      ///    based on the interaction defined in the <paramref name="simulation" />
      /// </summary>
      void UpdateReaction(IReactionBuilder reaction, string enzymeName, string compoundName, Simulation simulation, IFormulaCache formulaCache);

      /// <summary>
      ///    Updates the given transport process <paramref name="transporterMoleculeContainer" /> triggered by the
      ///    <paramref name="transporterName" />  and the <paramref name="transportedMolecule" /> (e.g. Drug) with the required
      ///    interaction terms
      ///    based on the interaction defined in the <paramref name="simulation" />
      /// </summary>
      void UpdateTransport(TransporterMoleculeContainer transporterMoleculeContainer, string transporterName, string transportedMolecule, Simulation simulation, IFormulaCache formulaCache);
   }

   public class InteractionKineticUpdater : IInteractionKineticUpdater
   {
      private readonly IObjectBaseFactory _objectBaseFactory;
      private readonly IReadOnlyCollection<IInteractionKineticUpdaterSpecification> _allKineticUpdaterSpecifications;

      public InteractionKineticUpdater(IRepository<IInteractionKineticUpdaterSpecification> repository, IObjectBaseFactory objectBaseFactory)
      {
         _objectBaseFactory = objectBaseFactory;
         _allKineticUpdaterSpecifications = repository.All().ToList();
      }

      public void UpdateReaction(IReactionBuilder reaction, string enzymeName, string compoundName, Simulation simulation, IFormulaCache formulaCache)
      {
         updateProcess(reaction, enzymeName, compoundName, simulation, formulaCache);
      }

      public void UpdateTransport(TransporterMoleculeContainer transporterMoleculeContainer, string transporterName, string transportedMolecule, Simulation simulation, IFormulaCache formulaCache)
      {
         updateProcess(transporterMoleculeContainer, transporterName, transportedMolecule, simulation, formulaCache);
      }

      private void updateProcess(IContainer processParameterContainer, string moleculeName, string compoundName, Simulation simulation, IFormulaCache formulaCache)
      {
         var allUpdatingKinetics = _allKineticUpdaterSpecifications.Where(x => x.UpdateRequiredFor(moleculeName, compoundName, simulation)).ToList();
         updateModifiers(processParameterContainer, allUpdatingKinetics, moleculeName, compoundName, simulation);
         updateKmFactor(processParameterContainer, allUpdatingKinetics, moleculeName, compoundName, simulation, formulaCache);
         updateKcatFactor(processParameterContainer, allUpdatingKinetics, moleculeName, compoundName, simulation, formulaCache);
         updateCLSpecFactor(processParameterContainer, allUpdatingKinetics, moleculeName, compoundName, simulation, formulaCache);
         updateKinactFactor(processParameterContainer, allUpdatingKinetics, moleculeName, compoundName, simulation, formulaCache);
         updateKKinactHalfFactor(processParameterContainer, allUpdatingKinetics, moleculeName, compoundName, simulation, formulaCache);
      }

      private void updateModifiers(IContainer processParameterContainer, IEnumerable<IInteractionKineticUpdaterSpecification> allUpdatingKinetics, string moleculeName, string compoundName, Simulation simulation)
      {
         var reaction = processParameterContainer as IReactionBuilder;
         if (reaction == null)
            return;

         allUpdatingKinetics.Each(x => x.UpdateModifiers(reaction, moleculeName, compoundName, simulation));
      }

      private void updateKmFactor(IContainer processParameterContainer, IReadOnlyList<IInteractionKineticUpdaterSpecification> allUpdatingKinetics, string moleculeName, string activatedMolecule, Simulation simulation, IFormulaCache formulaCache)
      {
         updateKmLikeFactor(processParameterContainer, allUpdatingKinetics, moleculeName, activatedMolecule, simulation, formulaCache, CoreConstants.Parameters.KM_INTERACTION_FACTOR);
      }

      private void updateKKinactHalfFactor(IContainer processParameterContainer, IReadOnlyList<IInteractionKineticUpdaterSpecification> allUpdatingKinetics, string moleculeName, string activatedMolecule, Simulation simulation, IFormulaCache formulaCache)
      {
         updateKmLikeFactor(processParameterContainer, allUpdatingKinetics, moleculeName, activatedMolecule, simulation, formulaCache, CoreConstants.Parameters.K_KINACT_HALF_INTERACTION_FACTOR);
      }

      private void updateKmLikeFactor(IContainer processParameterContainer, IReadOnlyList<IInteractionKineticUpdaterSpecification> allUpdatingKinetics, string moleculeName,
         string activatedMolecule, Simulation simulation, IFormulaCache formulaCache, string kmLikeParameterName)
      {
         updateInteractionFactor(processParameterContainer, activatedMolecule, allUpdatingKinetics, formulaCache,
            kmLikeParameterName, kmLikeFactor =>
            {
               var formulaStringNumerator = new List<string>();
               var formulaStringDenominator = new List<string>();

               allUpdatingKinetics.Each(x =>
               {
                  x.UpdateKmFactorReferences(kmLikeFactor, moleculeName, activatedMolecule, simulation, processParameterContainer);
                  formulaStringNumerator.Add(x.KmNumeratorTerm(moleculeName, activatedMolecule, simulation));
                  formulaStringDenominator.Add(x.KmDenominatorTerm(moleculeName, activatedMolecule, simulation));
               });

               return createFormula(formulaStringNumerator, formulaStringDenominator);
            });
      }

      private void updateKcatFactor(IContainer processParameterContainer, IReadOnlyList<IInteractionKineticUpdaterSpecification> allUpdatingKinetics, string moleculeName, string activatedMolecule, Simulation simulation, IFormulaCache formulaCache)
      {
         updateKcatLikeFactor(processParameterContainer, allUpdatingKinetics, moleculeName, activatedMolecule, simulation, formulaCache, CoreConstants.Parameters.KCAT_INTERACTION_FACTOR);
      }

      private void updateKinactFactor(IContainer processParameterContainer, IReadOnlyList<IInteractionKineticUpdaterSpecification> allUpdatingKinetics, string moleculeName, string activatedMolecule, Simulation simulation, IFormulaCache formulaCache)
      {
         updateKcatLikeFactor(processParameterContainer, allUpdatingKinetics, moleculeName, activatedMolecule, simulation, formulaCache, CoreConstants.Parameters.KINACT_INTERACTION_FACTOR);
      }

      private void updateKcatLikeFactor(IContainer processParameterContainer, IReadOnlyList<IInteractionKineticUpdaterSpecification> allUpdatingKinetics, string moleculeName,
         string activatedMolecule, Simulation simulation, IFormulaCache formulaCache, string kcatLikeParameterName)
      {
         updateInteractionFactor(processParameterContainer, activatedMolecule, allUpdatingKinetics, formulaCache,
            kcatLikeParameterName, kcatLikeFactor =>
            {
               var formulaStringDenominator = new List<string>();

               allUpdatingKinetics.Each(x =>
               {
                  x.UpdateKcatFactorReferences(kcatLikeFactor, moleculeName, activatedMolecule, simulation, processParameterContainer);
                  formulaStringDenominator.Add(x.KcatDenominatorTerm(moleculeName, activatedMolecule, simulation));
               });

               return createFormula(new List<string>(), formulaStringDenominator);
            });
      }

      private void updateCLSpecFactor(IContainer processParameterContainer, IReadOnlyList<IInteractionKineticUpdaterSpecification> allUpdatingKinetics, string moleculeName, string activatedMolecule, Simulation simulation, IFormulaCache formulaCache)
      {
         updateInteractionFactor(processParameterContainer, activatedMolecule, allUpdatingKinetics, formulaCache,
            CoreConstants.Parameters.CL_SPEC_PER_ENZYME_INTERACTION_FACTOR, CL_spec_factor =>
            {
               var formulaStringDenominator = new List<string>();
               allUpdatingKinetics.Each(x =>
               {
                  x.UpdateCLSpecFactorReferences(CL_spec_factor, moleculeName, activatedMolecule, simulation, processParameterContainer);
                  formulaStringDenominator.Add(x.CLSpecDenominatorTerm(moleculeName, activatedMolecule, simulation));
               });

               return createFormula(new List<string>(), formulaStringDenominator);
            });
      }

      private void updateInteractionFactor(IContainer processParameterContainer, string compoundName,
         IEnumerable<IInteractionKineticUpdaterSpecification> allUpdatingKinetics,
         IFormulaCache formulaCache,
         string parameterName, Func<IParameter, string> createFormulaAction)
      {
         var interactionFactor = processParameterContainer.Parameter(parameterName);
         if (!allUpdatingKinetics.Any() || interactionFactor == null)
            return;

         var formulaName = CompositeNameFor(processParameterContainer.Name, compoundName, parameterName);
         if (formulaCache.FindByName(formulaName) is ExplicitFormula formula)
         {
            interactionFactor.Formula = formula;
            return;
         }

         formula = _objectBaseFactory.Create<ExplicitFormula>()
            .WithName(formulaName)
            .WithDimension(interactionFactor.Dimension);

         interactionFactor.Formula = formula;
         formula.FormulaString = createFormulaAction(interactionFactor);
         formulaCache.Add(formula);
      }

      private string createFormula(List<string> formulaStringNumerator, List<string> formulaStringDenominator)
      {
         removeEmptyTerms(formulaStringNumerator);
         removeEmptyTerms(formulaStringDenominator);

         if (!formulaStringNumerator.Any() && !formulaStringDenominator.Any())
            return "1";

         if (!formulaStringNumerator.Any())
            return denominatorOnly(formulaStringDenominator);

         if (!formulaStringDenominator.Any())
            return numeratorOnly(formulaStringNumerator);

         return standardFormula(formulaStringNumerator, formulaStringDenominator);
      }

      private void removeEmptyTerms(List<string> formulaParts) => formulaParts.RemoveAll(string.IsNullOrEmpty);

      private string standardFormula(IEnumerable<string> formulaStringNumeratorParts, IEnumerable<string> formulaStringDenominatorParts) => $"{addOnePlusInParenthesis(formulaStringNumeratorParts)}/{addOnePlusInParenthesis(formulaStringDenominatorParts)}";

      private string denominatorOnly(IEnumerable<string> formulaStringDenominatorParts) => $"1/{addOnePlusInParenthesis(formulaStringDenominatorParts)}";

      private string numeratorOnly(IEnumerable<string> formulaStringNumeratorParts) => addOnePlus(formulaStringNumeratorParts);

      private string addOnePlusInParenthesis(IEnumerable<string> formulaParts) => $"({addOnePlus(formulaParts)})";

      private string addOnePlus(IEnumerable<string> formulaParts) => $"1 + {formulaParts.ToString(" + ")}";
   }
}