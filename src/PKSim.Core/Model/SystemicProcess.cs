using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using PKSim.Assets;

namespace PKSim.Core.Model
{
   public class SystemicProcess : CompoundProcess, ISpeciesDependentCompoundProcess
   {
      public SystemicProcessType SystemicProcessType { get; set; }

      private bool _isDefault;

      public override void RefreshName()
      {
         Name = CoreConstants.CompositeNameFor(SystemicProcessType.DisplayName, DataSource);
      }

      public bool IsDefault
      {
         get => _isDefault;
         set => SetProperty(ref _isDefault, value);
      }

      public override void UpdatePropertiesFrom(IUpdatable sourceObject, ICloneManager cloneManager)
      {
         base.UpdatePropertiesFrom(sourceObject, cloneManager);
         var sourceSystemicProcess = sourceObject as SystemicProcess;
         if (sourceSystemicProcess == null) return;
         SystemicProcessType = sourceSystemicProcess.SystemicProcessType;
         Species = sourceSystemicProcess.Species;
      }

      public override string ToString()
      {
         return DataSource;
      }
   }

   public class NullSystemicProcess : SystemicProcess
   {
   }

   public class NotSelectedSystemicProcess : NullSystemicProcess
   {
      public override string ToString()
      {
         return PKSimConstants.UI.None;
      }
   }

   public class NotAvailableSystemicProcess : NullSystemicProcess
   {
      public override string ToString()
      {
         return PKSimConstants.UI.NoSystemicProcessAvailable;
      }
   }
}