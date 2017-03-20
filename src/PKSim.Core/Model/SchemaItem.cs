using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core.Model
{
   public interface ISchemaItem : IContainer
   {
      ApplicationType ApplicationType { get; set; }
      bool NeedsFormulation { get; }
      IParameter StartTime { get; }
      string FormulationKey { get; set; }
      IParameter Dose { get; }
      string TargetOrgan { get; set; }
      string TargetCompartment { get; set; }
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
         get { return _applicationType; }
         set
         {
            _applicationType = value;
            OnPropertyChanged(() => ApplicationType);
         }
      }

      public string FormulationKey
      {
         get { return _formulationKey; }
         set
         {
            _formulationKey = value;
            OnPropertyChanged(() => FormulationKey);
         }
      }

      public string TargetCompartment
      {
         get { return _targetCompartment; }
         set
         {
            _targetCompartment = value;
            OnPropertyChanged(() => TargetCompartment);
         }
      }

      public string TargetOrgan
      {
         get { return _targetOrgan; }
         set
         {
            _targetOrgan = value;
            OnPropertyChanged(() => TargetOrgan);
         }
      }

      public bool NeedsFormulation => ApplicationType.NeedsFormulation;

      public IParameter StartTime => this.Parameter(Constants.Parameters.START_TIME);

      public IParameter Dose => this.Parameter(CoreConstants.Parameter.INPUT_DOSE);

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