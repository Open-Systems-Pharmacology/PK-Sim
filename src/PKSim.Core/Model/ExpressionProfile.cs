using System.Linq;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Model
{
   public class ExpressionProfile : PKSimBuildingBlock
   {
      public const string DUMMY_MOLECULE_NAME = "%DUMMY%";

      private string _category;
      public virtual Species Species { get; set; }

      public ExpressionProfile() : base(PKSimBuildingBlockType.ExpressionProfile)
      {
      }

      public virtual string MoleculeName => Molecule?.Name;

      public virtual string Category
      {
         get => _category;
         set
         {
            _category = value;
            RefreshName();
         }
      }

      public Individual Individual { get; set; }

      public IndividualMolecule Molecule => Individual?.AllMolecules().FirstOrDefault() ?? new NullIndividualMolecule();

      public virtual void RefreshName()
      {
         Name = CoreConstants.CompositeNameFor(MoleculeName, Category);
      }

      public override string Icon => Molecule?.Icon ?? "";
   }
}