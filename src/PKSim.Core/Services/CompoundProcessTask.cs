using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using PKSim.Core.Commands;
using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public class CompoundProcessTask : ICompoundProcessTask
   {
      private readonly IExecutionContext _executionContext;

      public CompoundProcessTask(IExecutionContext executionContext)
      {
         _executionContext = executionContext;
      }

      public IPKSimBuildingBlock GetBuildingBlockForProcess(CompoundProcess process)
      {
         return _executionContext.BuildingBlockContaining(process);
      }

      public IPKSimCommand RemoveProcess(Compound compound, CompoundProcess processToRemove)
      {
         return new RemoveProcessFromCompoundCommand(processToRemove, compound, _executionContext).Run(_executionContext);
      }

      public IPKSimCommand SetSpeciesForProcess(CompoundProcess compoundProcess, Species species)
      {
         if (species == null)
            return new PKSimEmptyCommand();
         return new SetSpeciesInCompoundProcessCommand(compoundProcess, species).Run(_executionContext);
      }

      public TProcess CreateProcessFromTemplate<TProcess>(TProcess processTemplate, Compound compound) where TProcess : CompoundProcess
      {
         var newProcess = _executionContext.Clone(processTemplate).WithName(string.Empty);
         updateFractionUnbound(newProcess, compound);
         updateLipophilicity(newProcess, compound);
         return newProcess;
      }

      private void updateFractionUnbound(CompoundProcess newProcess, Compound compound)
      {
         updateProcessParameterFromDefaultAlternative(newProcess, compound, CoreConstants.Parameters.FRACTION_UNBOUND_EXPERIMENT,
            CoreConstants.Parameters.FRACTION_UNBOUND_PLASMA_REFERENCE_VALUE,
            CoreConstants.Groups.COMPOUND_FRACTION_UNBOUND);
      }

      private void updateLipophilicity(CompoundProcess newProcess, Compound compound)
      {
         updateProcessParameterFromDefaultAlternative(newProcess, compound, CoreConstants.Parameters.LIPOPHILICITY_EXPERIMENT,
            CoreConstants.Parameters.LIPOPHILICITY,
            CoreConstants.Groups.COMPOUND_LIPOPHILICITY);
      }

      private void updateProcessParameterFromDefaultAlternative(CompoundProcess newProcess, Compound compound, string processParameterName, string compoundParameterName, string groupName)
      {
         var processParameter = newProcess.Parameter(processParameterName);
         if (processParameter == null)
            return;

         var parameterAlternative = compound.ParameterAlternativeGroup(groupName).DefaultAlternative;
         if (parameterAlternative == null)
            return;

         processParameter.Value = parameterAlternative.Parameter(compoundParameterName).Value;
         processParameter.IsDefault = false;
      }

      public IPKSimCommand SetMetaboliteForEnzymaticProcess(EnzymaticProcess process, string newMetabolite)
      {
         return getChangeEnzymaticProcessMetaboliteCommand(process, newMetabolite, _executionContext).Run(_executionContext);
      }

      private static IPKSimCommand getChangeEnzymaticProcessMetaboliteCommand(EnzymaticProcess process, string newMetabolite, IExecutionContext executionContext)
      {
         return new ChangeEnzymaticProcessMetaboliteNameCommand(process, newMetabolite, executionContext);
      }
   }
}