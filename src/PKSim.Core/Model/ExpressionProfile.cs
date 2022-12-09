using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Utility.Visitor;
using static OSPSuite.Core.Domain.Constants.ContainerName;

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
         }
      }

      public virtual Species Species => Individual?.Species;

      public ExpressionProfile() : base(PKSimBuildingBlockType.ExpressionProfile)
      {
      }
      
      public override IReadOnlyList<T> GetAllChildren<T>()
      {
         return Individual.GetAllChildren<T>();
      }

      public virtual string MoleculeName => Molecule?.Name;

      public virtual string Category
      {
         get => _category;
         set => _category = value;
      }

      public override string Name
      {
         get => ExpressionProfileName(MoleculeName, Species?.DisplayName, Category);
         set
         {
            if (string.Equals(Name, value))
            {
               //Even if the name has not changed, we raise the even here as the underlying composite name may have changed
               //Small performance loss but easier to see updates in the UI when using the standard logic of updating the name directly
               OnPropertyChanged(() => Name);
               return;
            }

            var (moleculeName, _, category) = NamesFromExpressionProfileName(value);
            if (string.IsNullOrEmpty(moleculeName))
               return;

            _category = category;

            //This can happen when for instance the Molecule has not been deserialized yet
            if (Molecule != null)
               Molecule.Name = moleculeName;

            OnPropertyChanged(() => Name);
         }
      }

      public void Deconstruct(out IndividualMolecule molecule, out Individual individual)
      {
         molecule = Molecule;
         individual = Individual;
      }

      public virtual IndividualMolecule Molecule => Individual?.AllMolecules().FirstOrDefault() ?? new NullIndividualMolecule();

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