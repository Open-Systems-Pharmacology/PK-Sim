using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core.Model
{
   public abstract class CompoundProcess : Container, ISpeciesDependentEntity
   {
      /// <summary>
      /// This is the internal name of the process as defined in the database. This should never be changed programmatically
      /// </summary>
      public string InternalName { get; set; }
      private Species _species;
      private string _dataSource;

      protected CompoundProcess()
      {
         _dataSource = string.Empty;
         InternalName = string.Empty;
      }

      public Compound ParentCompound
      {
         get
         {
            var container = ParentContainer;

            while (container != null)
            {
               var compound = container as Compound;
               if (compound != null)
                  return compound;

               container = container.ParentContainer;
            }

            return null;
         }
      }

      /// <summary>
      ///    Source (reference) where the process was measured
      /// </summary>
      public string DataSource
      {
         get => _dataSource;
         set => SetProperty(ref _dataSource,value);
      }

      /// <summary>
      ///    Update name from DataSource
      /// </summary>
      public abstract void RefreshName();

      public override void UpdatePropertiesFrom(IUpdatable sourceObject, ICloneManager cloneManager)
      {
         base.UpdatePropertiesFrom(sourceObject, cloneManager);
         var sourceProcess = sourceObject as CompoundProcess;
         if (sourceProcess == null) return;
         InternalName = sourceProcess.InternalName;
         Species = sourceProcess.Species;
         DataSource = sourceProcess.DataSource;
      }

      public Species Species
      {
         get => _species;
         set => SetProperty(ref _species, value);
      }
   }

   public interface ISpeciesDependentCompoundProcess : ISpeciesDependentEntity
   {
   }
}