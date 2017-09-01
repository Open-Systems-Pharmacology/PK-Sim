using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core.Model
{
   public interface ISchemaItem : IContainer
   {
      ApplicationType ApplicationType { get; set; }
      string FormulationKey { get; set; }
      string TargetOrgan { get; set; }
      string TargetCompartment { get; set; }
      bool NeedsFormulation { get; }
      IParameter StartTime { get; }
      IParameter Dose { get; }
   }

   public class SchemaItem : Container, ISchemaItem
   {
      private ApplicationType _applicationType;
      private string _formulationKey;
      private string _targetCompartment;
      private string _targetOrgan;

      public SchemaItem()
      {
         Rules.AddRange(SchemaItemRules.All());
      }

      public ApplicationType ApplicationType
      {
         get => _applicationType;
         set => SetProperty(ref _applicationType, value);
      }

      public string FormulationKey
      {
         get => _formulationKey;
         set => SetProperty(ref _formulationKey, value);
      }

      public string TargetCompartment
      {
         get => _targetCompartment;
         set => SetProperty(ref _targetCompartment, value);
      }

      public string TargetOrgan
      {
         get => _targetOrgan;
         set => SetProperty(ref _targetOrgan, value);
      }

      public bool NeedsFormulation => ApplicationType.NeedsFormulation;

      public virtual IParameter StartTime => this.Parameter(Constants.Parameters.START_TIME);

      public virtual IParameter Dose => this.Parameter(CoreConstants.Parameter.INPUT_DOSE);

      public override void UpdatePropertiesFrom(IUpdatable sourceObject, ICloneManager cloneManager)
      {
         base.UpdatePropertiesFrom(sourceObject, cloneManager);
         var sourceSchemaItem = sourceObject as ISchemaItem;
         if (sourceSchemaItem == null) return;
         ApplicationType = sourceSchemaItem.ApplicationType;
         FormulationKey = sourceSchemaItem.FormulationKey;
         TargetOrgan = sourceSchemaItem.TargetOrgan;
         TargetCompartment = sourceSchemaItem.TargetCompartment;
      }
   }
}