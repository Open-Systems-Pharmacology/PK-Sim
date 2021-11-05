using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core.Model
{
   public class ExpressionProfile : PKSimBuildingBlock
   {
      public const string MOLECULE_NAME = "<MOLECULE>";

      private string _category;
      private string _moleculeName;
      public virtual Species Species { get; set; }

      public ExpressionProfile() : base(PKSimBuildingBlockType.ExpressionProfile)
      {
      }

      public virtual string MoleculeName
      {
         get => _moleculeName;
         set
         {
            _moleculeName = value;
            RefreshName();
         }
      }

      public virtual string Category
      {
         get => _category;
         set
         {
            _category = value;
            RefreshName();
         }
      }

      public override string Name
      {
         set
         {
            if (string.Equals(Name, value))
               return;

            var names = CoreConstants.NamesFromCompositeName(value);
            if (names.Count != 2)
               return;

            _moleculeName = names[0];
            _category = names[1];
            base.Name = value;
         }
      }

      public Individual Individual { get; set; }

      public IndividualMolecule Molecule => Individual?.MoleculeByName(MOLECULE_NAME) ?? new NullIndividualMolecule();

      public virtual void RefreshName()
      {
         Name = CoreConstants.CompositeNameFor(MoleculeName, Category);
      }

      public override string Icon => Molecule?.Icon ?? "";

      public override void UpdatePropertiesFrom(IUpdatable sourceObject, ICloneManager cloneManager)
      {
         base.UpdatePropertiesFrom(sourceObject, cloneManager);
         var sourceExpressionProfile = sourceObject as ExpressionProfile;
         if (sourceExpressionProfile == null) return;
         MoleculeName = sourceExpressionProfile.MoleculeName;
         Category = sourceExpressionProfile.Category;
         Species = sourceExpressionProfile.Species;
         Individual = cloneManager.Clone(sourceExpressionProfile.Individual);
      }
   }
}