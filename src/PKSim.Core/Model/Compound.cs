using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core.Model
{
   public enum CompoundType
   {
      Acid = CoreConstants.Compound.COMPOUND_TYPE_ACID,
      Base = CoreConstants.Compound.COMPOUND_TYPE_BASE,
      Neutral = CoreConstants.Compound.COMPOUND_TYPE_NEUTRAL
   }

   public enum PlasmaProteinBindingPartner
   {
      Glycoprotein = CoreConstants.Compound.BINDING_PARTNER_AGP,
      Albumin = CoreConstants.Compound.BINDING_PARTNER_ALBUMIN,
      Unknown = CoreConstants.Compound.BINDING_PARTNER_UNKNOWN
   }

   public class Compound : PKSimBuildingBlock, IWithCalculationMethods
   {
      public CalculationMethodCache CalculationMethodCache { get; }

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
      public virtual bool IsNeutral => this.Parameter(CoreConstants.Parameter.IS_NEUTRAL).Value == 1;

      /// <summary>
      ///    returns true if the parameter is small molecule otherwise false
      /// </summary>
      public virtual bool IsSmallMolecule
      {
         get => this.Parameter(CoreConstants.Parameter.IS_SMALL_MOLECULE).Value == 1;
         set => this.Parameter(CoreConstants.Parameter.IS_SMALL_MOLECULE).Value = value ? 1: 0;
      }

      public virtual PlasmaProteinBindingPartner PlasmaProteinBindingPartner
      {
         get => (PlasmaProteinBindingPartner)this.Parameter(CoreConstants.Parameter.PLASMA_PROTEIN_BINDING_PARTNER).Value;
         set => this.Parameter(CoreConstants.Parameter.PLASMA_PROTEIN_BINDING_PARTNER).Value = (int) value;
      }

      public virtual IEnumerable<ParameterAlternativeGroup> AllParameterAlternativeGroups() => GetChildren<ParameterAlternativeGroup>();

      public virtual void AddParameterAlternativeGroup(ParameterAlternativeGroup parameterAlternativeGroup) => Add(parameterAlternativeGroup);

      /// <summary>
      ///    Return a parameter group for which alternatives might be defined
      /// </summary>
      /// <param name="parameterGroupName">Name of a parameters with alteratives</param>
      public virtual ParameterAlternativeGroup ParameterAlternativeGroup(string parameterGroupName)
      {
         return this.GetSingleChildByName<ParameterAlternativeGroup>(parameterGroupName);
      }

      public virtual IEnumerable<T> AllProcesses<T>() where T : CompoundProcess => GetChildren<T>();

      public virtual IEnumerable<CompoundProcess> AllProcesses() => AllProcesses<CompoundProcess>();

      /// <summary>
      ///    Returns <c>true</c> if at least one proces is defined in the compound otherwise false
      /// </summary>
      public virtual bool HasProcesses() => HasProcesses<CompoundProcess>();

      /// <summary>
      ///    Returns <c>true</c> if at least one proces of type <typeparamref name="TCompoundProcess"/> is defined in the compound otherwise false
      /// </summary>
      public virtual bool HasProcesses<TCompoundProcess>() where TCompoundProcess : CompoundProcess => AllProcesses<TCompoundProcess>().Any();

      public virtual IEnumerable<SystemicProcess> AllSystemicProcessesOfType(SystemicProcessType systemicProcessType)
      {
         return GetChildren<SystemicProcess>(proc => proc.SystemicProcessType == systemicProcessType);
      }

      public virtual TCompoundProcess ProcessByName<TCompoundProcess>(string processName) where TCompoundProcess : CompoundProcess
      {
         return this.GetSingleChildByName<TCompoundProcess>(processName);
      }

      public virtual CompoundProcess ProcessByName(string processName) => ProcessByName<CompoundProcess>(processName);

      public virtual bool ProcessExistsByName(string processName) => ProcessByName(processName) != null;

      public virtual double MolWeight => this.Parameter(Constants.Parameters.MOL_WEIGHT).Value;

      public override void UpdatePropertiesFrom(IUpdatable sourceObject, ICloneManager cloneManager)
      {
         base.UpdatePropertiesFrom(sourceObject, cloneManager);
         var sourceCompound = sourceObject as Compound;
         if (sourceCompound == null) return;
         CalculationMethodCache.UpdatePropertiesFrom(sourceCompound.CalculationMethodCache, cloneManager);
      }
   }
}