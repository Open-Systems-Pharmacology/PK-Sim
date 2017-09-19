using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core.Model
{
   public class EnzymaticProcessSelection : ProcessSelection
   {
      /// <summary>
      ///    Name of metabolite associated with the enzymatic process. If not set, no metabolite was selected
      /// </summary>
      public string MetaboliteName { get; set; }

      public bool IsSink => string.IsNullOrEmpty(MetaboliteName);

      public string ProductName()
      {
         return IsSink ? ProductName(CoreConstants.Molecule.Metabolite) : MetaboliteName; 
      }

      public override ProcessSelection Clone(ICloneManager cloneManager)
      {
         var clone = new EnzymaticProcessSelection();
         clone.UpdatePropertiesFrom(this, cloneManager);
         return clone;
      }

      public override void UpdatePropertiesFrom(IUpdatable source, ICloneManager cloneManager)
      {
         base.UpdatePropertiesFrom(source, cloneManager);
         var sourceEnymaticProcessSelection = source as EnzymaticProcessSelection;
         if (sourceEnymaticProcessSelection == null) return;
         MetaboliteName = sourceEnymaticProcessSelection.MetaboliteName;
      }
   }
}