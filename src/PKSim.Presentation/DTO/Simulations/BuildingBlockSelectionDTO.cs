using OSPSuite.Utility.Reflection;
using PKSim.Core.Model;

namespace PKSim.Presentation.DTO.Simulations
{
   public class BuildingBlockSelectionDTO<TBuildingBlock> : Notifier
   {
      protected TBuildingBlock _buildingBlock;

      public TBuildingBlock BuildingBlock
      {
         get => _buildingBlock;
         set => SetProperty(ref _buildingBlock, value);
      }
   }

   public class ProtocolSelectionDTO : BuildingBlockSelectionDTO<Protocol>
   {
   }

   public class SimulationSubjectDTO : BuildingBlockSelectionDTO<ISimulationSubject>
   {
      private bool _allowAging;

      public bool AllowAging
      {
         get => _allowAging;
         set => SetProperty(ref _allowAging, value);
      }
   }

   public class CompoundSelectionDTO : BuildingBlockSelectionDTO<Compound>
   {
      public string DisplayName { get; set; }
      private bool _selected;

      public bool Selected
      {
         get => _selected;
         set => SetProperty(ref _selected, value);
      }
   }

   public class FormulationSelectionDTO : BuildingBlockSelectionDTO<Formulation>
   {
      public string DisplayName { get; set; }

      public override bool Equals(object obj)
      {
         return Equals(obj as FormulationSelectionDTO);
      }

      public bool Equals(FormulationSelectionDTO other)
      {
         if (ReferenceEquals(null, other)) return false;
         if (ReferenceEquals(this, other)) return true;
         return Equals(BuildingBlock, other.BuildingBlock);
      }

      public override int GetHashCode()
      {
         return BuildingBlock?.GetHashCode() ?? 0;
      }

      public override string ToString()
      {
         return DisplayName;
      }
   }
}