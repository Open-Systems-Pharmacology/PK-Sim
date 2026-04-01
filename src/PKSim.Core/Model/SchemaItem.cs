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
      /// <summary>
      ///    Identifier for the event placeholder (e.g. "EVENT_1") used to map this entry
      ///    to an actual PKSimEvent building block during simulation configuration.
      ///    Only applicable when ApplicationType is Event.
      /// </summary>
      string EventKey { get; set; }
      bool NeedsFormulation { get; }
      bool IsEvent { get; }
      IParameter StartTime { get; }
      IParameter Dose { get; }
   }

   public class SchemaItem : Container, ISchemaItem
   {
      private ApplicationType _applicationType;
      private string _formulationKey;
      private string _targetCompartment;
      private string _targetOrgan;
      private string _eventKey;

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

      public string EventKey
      {
         get => _eventKey;
         set => SetProperty(ref _eventKey, value);
      }

      public bool NeedsFormulation => ApplicationType.NeedsFormulation;

      public bool IsEvent => ApplicationType == ApplicationTypes.Event;

      public bool IsOral => ApplicationType == ApplicationTypes.Oral;

      public bool IsUserDefined => ApplicationType == ApplicationTypes.UserDefined;

      public bool IsIV => ApplicationType == ApplicationTypes.Intravenous || ApplicationType == ApplicationTypes.IntravenousBolus;

      public virtual IParameter StartTime => this.Parameter(Constants.Parameters.START_TIME);

      public virtual IParameter Dose => this.Parameter(CoreConstants.Parameters.INPUT_DOSE);

      public override void UpdatePropertiesFrom(IUpdatable sourceObject, ICloneManager cloneManager)
      {
         base.UpdatePropertiesFrom(sourceObject, cloneManager);
         var sourceSchemaItem = sourceObject as ISchemaItem;
         if (sourceSchemaItem == null) return;
         ApplicationType = sourceSchemaItem.ApplicationType;
         FormulationKey = sourceSchemaItem.FormulationKey;
         TargetOrgan = sourceSchemaItem.TargetOrgan;
         TargetCompartment = sourceSchemaItem.TargetCompartment;
         EventKey = sourceSchemaItem.EventKey;
      }
   }
}