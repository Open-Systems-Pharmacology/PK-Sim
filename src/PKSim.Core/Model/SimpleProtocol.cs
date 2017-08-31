using System.Collections.Generic;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Domain.UnitSystem;

namespace PKSim.Core.Model
{
   public class SimpleProtocol : Protocol, ISchemaItem
   {
      private ApplicationType _applicationType;
      private string _formulationKey;
      private DosingInterval _dosingInterval;
      private string _targetCompartment;
      private string _targetOrgan;

      public SimpleProtocol()
      {
         Rules.AddRange(SchemaItemRules.All());
      }

      public virtual ApplicationType ApplicationType
      {
         get => _applicationType;
         set => SetProperty(ref _applicationType, value);
      }

      public virtual DosingInterval DosingInterval
      {
         get => _dosingInterval;
         set => SetProperty(ref _dosingInterval, value);
      }

      public virtual string FormulationKey
      {
         get => _formulationKey;
         set => SetProperty(ref _formulationKey, value);
      }

      public virtual string TargetCompartment
      {
         get => _targetCompartment;
         set => SetProperty(ref _targetCompartment, value);
      }

      public virtual string TargetOrgan
      {
         get => _targetOrgan;
         set => SetProperty(ref _targetOrgan, value);
      }

      public virtual bool NeedsFormulation => ApplicationType.NeedsFormulation;

      public virtual IParameter StartTime => this.Parameter(Constants.Parameters.START_TIME);

      public virtual IParameter Dose => this.Parameter(CoreConstants.Parameter.INPUT_DOSE);

      public virtual IParameter EndTimeParameter => this.Parameter(Constants.Parameters.END_TIME);

      public virtual bool IsSingleDosing => DosingInterval == DosingIntervals.Single;

      public virtual void AddParameter(IParameter parameter)
      {
         Add(parameter);
      }

      public virtual Unit DoseUnit => Dose.DisplayUnit;

      public override Unit TimeUnit
      {
         get => EndTimeParameter.DisplayUnit;
         set => EndTimeParameter.DisplayUnit = value;
      }

      public override IEnumerable<string> UsedFormulationKeys => new[] {_formulationKey};

      public override ApplicationType ApplicationTypeUsing(string formulationKey)
      {
         return _applicationType;
      }

      public override double EndTime => EndTimeParameter.Value;

      public override void UpdatePropertiesFrom(IUpdatable sourceObject, ICloneManager cloneManager)
      {
         base.UpdatePropertiesFrom(sourceObject, cloneManager);
         var simpleProtocol = sourceObject as SimpleProtocol;
         if (simpleProtocol == null) return;
         FormulationKey = simpleProtocol.FormulationKey;
         ApplicationType = simpleProtocol.ApplicationType;
         DosingInterval = simpleProtocol.DosingInterval;
         TargetOrgan = simpleProtocol.TargetOrgan;
         TargetCompartment = simpleProtocol.TargetCompartment;
      }
   }
}