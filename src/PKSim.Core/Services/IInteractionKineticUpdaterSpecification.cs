using PKSim.Core.Model;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;

namespace PKSim.Core.Services
{
   public interface IInteractionKineticUpdaterSpecification
   {
      /// <summary>
      /// Returns <c>true</c> if specific interactions are defined in the <paramref name="simulation"/> for the molecule named <paramref name="moleculeName"/> otherwise false
      /// </summary>
      bool UpdateRequiredFor(string moleculeName, string compoundName, Simulation simulation);

      /// <summary>
      /// Updates the references to object dynamically added to the formula of the Km_interaction_factor
      /// </summary>
      void UpdateKmFactorReferences(IParameter kmFactor, string moleculeName, string compoundName, Simulation simulation, IContainer processParameterContainer);

      /// <summary>
      /// Updates the references to object dynamically added to the formula of the Vmax_interaction_factor
      /// </summary>
      void UpdateKcatFactorReferences(IParameter vmaxFactor, string moleculeName, string compoundName, Simulation simulation, IContainer processParameterContainer);

      /// <summary>
      /// Updates the references to object dynamically added to the formula of the CL_spec_interaction_factor
      /// </summary>
      void UpdateCLSpecFactorReferences(IParameter clspecFactor, string moleculeName, string compoundName, Simulation simulation, IContainer processParameterContainer);

      /// <summary>
      /// Returns the term modifying the numerator of the Km factor or an empty string if no modification is required
      /// </summary>
      string KmNumeratorTerm(string moleculeName, string compoundName, Simulation simulation);

      /// <summary>
      /// Returns the term modifying the denominator of the Km factor or an empty string if no modification is required
      /// </summary>
      string KmDenominatorTerm(string moleculeName, string compoundName, Simulation simulation);

      /// <summary>
      /// Returns the term modifying the denominator of the Vmax factor or an empty string if no modification is required
      /// </summary>
      string KcatDenominatorTerm(string moleculeName, string compoundName, Simulation simulation);

      /// <summary>
      /// Returns the term modifying the denominator of the CL spec factor or an empty string if no modification is required
      /// </summary>
      string CLSpecDenominatorTerm(string moleculeName, string compoundName, Simulation simulation);

      /// <summary>
      /// Add all inhibitors as modifier of the given <paramref name="reaction"/>
      /// </summary>
      void UpdateModifiers(IReactionBuilder reaction, string moleculeName, string compoundName, Simulation simulation);
   }
}