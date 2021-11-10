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
      private string _moleculeName;

      //Individual is set in factory and we can assume it will never be null
      public Individual Individual { get; set; }

      public virtual Species Species => Individual.Species;

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
            if (names.Count != 3)
               return;

            _moleculeName = names[0];
            _category = names[2];
            base.Name = value;
         }
      }


      public virtual IndividualMolecule Molecule => Individual.AllMolecules().FirstOrDefault() ?? new NullIndividualMolecule();

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
         MoleculeName = sourceExpressionProfile.MoleculeName;
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
            if(Individual!=null)
               Individual.HasChanged = value;
         }
      }
   }
}