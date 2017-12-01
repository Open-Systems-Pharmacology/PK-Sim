using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core.Model
{
   public enum CompoundType
   {
      Acid = CoreConstants.Compound.CompoundTypeAcid,
      Base = CoreConstants.Compound.CompoundTypeBase,
      Neutral = CoreConstants.Compound.CompoundTypeNeutral
   }

   public class Compound : PKSimBuildingBlock, IWithCalculationMethods
   {
      public CalculationMethodCache CalculationMethodCache { get; private set; }

      public Compound() : base(PKSimBuildingBlockType.Compound)
      {
         CalculationMethodCache = new CalculationMethodCache();
      }

      public virtual void AddProcess(CompoundProcess process)
      {
         Add(process);
      }

      public virtual void RemoveProcess(CompoundProcess process)
      {
         RemoveChild(process);
      }

      /// <summary>
      ///    Returns the parameter that are not defined in alternatives
      /// </summary>
      public virtual IEnumerable<IParameter> AllSimpleParameters()
      {
         return this.AllParameters(p => !CoreConstants.Groups.GroupsWithAlternative.Contains(p.GroupName));
      }

      /// <summary>
      ///    returns true if the parameter is neutral otherwise false
      /// </summary>
      public virtual bool IsNeutral
      {
         get { return this.Parameter(CoreConstants.Parameter.IS_NEUTRAL).Value == 1; }
      }

      /// <summary>
      ///    returns true if the parameter is small molecule otherwise false
      /// </summary>
      public virtual bool IsSmallMolecule
      {
         get { return this.Parameter(CoreConstants.Parameter.IS_SMALL_MOLECULE).Value == 1; }
      }

      public virtual IEnumerable<ParameterAlternativeGroup> AllParameterAlternativeGroups()
      {
         return GetChildren<ParameterAlternativeGroup>();
      }

      public virtual void AddParameterAlternativeGroup(ParameterAlternativeGroup parameterAlternativeGroup)
      {
         Add(parameterAlternativeGroup);
      }

      /// <summary>
      ///    Return a parameter group for which alternatives might be defined
      /// </summary>
      /// <param name="parameterGroupName">Name of a parameters with alteratives</param>
      public virtual ParameterAlternativeGroup ParameterAlternativeGroup(string parameterGroupName)
      {
         return this.GetSingleChildByName<ParameterAlternativeGroup>(parameterGroupName);
      }

      public virtual IEnumerable<T> AllProcesses<T>() where T : CompoundProcess
      {
         return GetChildren<T>();
      }

      /// <summary>
      ///    Returns <c>true</c> if at least one proces is defined in the compound otherwise false
      /// </summary>
      public virtual bool HasProcesses()
      {
         return HasProcesses<CompoundProcess>(); 
      }

      /// <summary>
      ///    Returns <c>true</c> if at least one proces of type <typeparamref name="TCompoundProcess"/> is defined in the compound otherwise false
      /// </summary>
      public virtual bool HasProcesses<TCompoundProcess>() where TCompoundProcess : CompoundProcess
      {
         return AllProcesses<TCompoundProcess>().Any();
      }
      
      public virtual IEnumerable<SystemicProcess> AllSystemicProcessesOfType(SystemicProcessType systemicProcessType)
      {
         return GetChildren<SystemicProcess>(proc => proc.SystemicProcessType == systemicProcessType);
      }

      public virtual TPartialProcess ProcessByName<TPartialProcess>(string processName) where TPartialProcess : CompoundProcess
      {
         return this.GetSingleChildByName<TPartialProcess>(processName);
      }

      public virtual CompoundProcess ProcessByName(string processName)
      {
         return ProcessByName<CompoundProcess>(processName);
      }

      public virtual bool ProcessExistsByName(string processName)
      {
         return ProcessByName(processName) != null;
      }

      public virtual double MolWeight
      {
         get { return this.Parameter(Constants.Parameters.MOL_WEIGHT).Value; }
      }

      public override void UpdatePropertiesFrom(IUpdatable sourceObject, ICloneManager cloneManager)
      {
         base.UpdatePropertiesFrom(sourceObject, cloneManager);
         var sourceCompound = sourceObject as Compound;
         if (sourceCompound == null) return;
         CalculationMethodCache.UpdatePropertiesFrom(sourceCompound.CalculationMethodCache, cloneManager);
      }
   }
}