using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core.Model
{
   public abstract class PartialProcess : CompoundProcess
   {
      private string _moleculeName;

      public override void RefreshName()
      {
         Name = CoreConstants.CompositeNameFor(MoleculeName, DataSource);
      }

      public override void UpdatePropertiesFrom(IUpdatable sourceObject, ICloneManager cloneManager)
      {
         base.UpdatePropertiesFrom(sourceObject, cloneManager);
         var sourcePartialProcess = sourceObject as PartialProcess;
         if (sourcePartialProcess == null) return;

         MoleculeName = sourcePartialProcess.MoleculeName;
      }

      /// <summary>
      ///    Name of the molecule responsible for triggering the processs
      /// </summary>
      public string MoleculeName
      {
         get => _moleculeName;
         set => SetProperty(ref _moleculeName, value);
      }

      public abstract string GetProcessClass();

   }
}