using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Utility.Visitor;
using static PKSim.Core.CoreConstants.ContainerName;

namespace PKSim.Core.Model
{
   public class ExpressionProfile : PKSimBuildingBlock
   {
      private string _category;
      private Individual _individual;

      //Individual is set in factory and we can assume it will never be null
      public Individual Individual
      {
         get => _individual;
         set
         {
            _individual = value;
            _individual.OwnedBy = this;
            RefreshName();
         }
      }

      public virtual Species Species => Individual?.Species;

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

      public override string Name
      {
         set
         {
            if (string.Equals(Name, value))
               return;

            var (moleculeName, _, category) = NamesFromExpressionProfileName(value);
            if (string.IsNullOrEmpty(moleculeName))
               return;

            _category = category;

            //This can happen when for instance the Molecule has not been deserialized yet
            if (Molecule != null)
               Molecule.Name = moleculeName;

            base.Name = value;
         }
      }

      public void Deconstruct(out IndividualMolecule molecule, out Individual individual)
      {
         molecule = Molecule;
         individual = Individual;
      }

      public virtual IndividualMolecule Molecule => Individual?.AllMolecules().FirstOrDefault() ?? new NullIndividualMolecule();

      public virtual void RefreshName()
      {
         Name = ExpressionProfileName(MoleculeName, Species, Category);
      }

      public override string Icon => Molecule?.Icon ?? "";

      public override void UpdatePropertiesFrom(IUpdatable sourceObject, ICloneManager cloneManager)
      {
         base.UpdatePropertiesFrom(sourceObject, cloneManager);
         var sourceExpressionProfile = sourceObject as ExpressionProfile;
         if (sourceExpressionProfile == null) return;
         Category = sourceExpressionProfile.Category;
         Individual = cloneManager.Clone(sourceExpressionProfile.Individual);
      }

      public override void AcceptVisitor(IVisitor visitor)
      {
         base.AcceptVisitor(visitor);
         Individual.AcceptVisitor(visitor);
      }

      public override bool HasChanged
      {
         get => base.HasChanged || Individual.HasChanged;
         set
         {
            base.HasChanged = value;
            if (Individual != null)
               Individual.HasChanged = value;
         }
      }
   }
}