using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using PKSim.Assets;
using static PKSim.Core.CoreConstants.Parameters;

namespace PKSim.Core.Model
{
   public abstract class IndividualMolecule : Container
   {
      public virtual string QueryConfiguration { get; set; }
      private Ontogeny _ontogeny;
      public virtual QuantityType MoleculeType { get; protected set; }

      protected IndividualMolecule()
      {
         Ontogeny = new NullOntogeny();
         ContainerType = ContainerType.Molecule;
      }

      public virtual double OntogenyFactor
      {
         get => OntogenyFactorParameter.Value;
         set => OntogenyFactorParameter.Value = value;
      }

      public IParameter OntogenyFactorParameter => this.Parameter(ONTOGENY_FACTOR);

      public virtual double OntogenyFactorGI
      {
         get => OntogenyFactorGIParameter.Value;
         set => OntogenyFactorGIParameter.Value = value;
      }

      public IParameter OntogenyFactorGIParameter => this.Parameter(ONTOGENY_FACTOR_GI);

      public IParameter DiseaseFactorParameter => this.Parameter(DISEASE_FACTOR);

      public virtual double DiseaseFactor
      {
         get => DiseaseFactorParameter.Value;
         set => DiseaseFactorParameter.Value = value;
      }

      public virtual bool HasQuery()
      {
         return !string.IsNullOrEmpty(QueryConfiguration);
      }

      public virtual IParameter ReferenceConcentration => this.Parameter(REFERENCE_CONCENTRATION);

      public virtual IParameter HalfLifeLiver => this.Parameter(HALF_LIFE_LIVER);

      public virtual IParameter HalfLifeIntestine => this.Parameter(HALF_LIFE_INTESTINE);

      public IReadOnlyList<IParameter> AllGlobalMoleculeParameters => new[] {ReferenceConcentration, HalfLifeLiver, HalfLifeIntestine};

      public IReadOnlyList<IParameter> AllOntogenyParameters => new[] {OntogenyFactorParameter, OntogenyFactorGIParameter};

      public IReadOnlyList<IParameter> AllDiseaseStateParameters => new[] {DiseaseFactorParameter};

      public IReadOnlyList<IParameter> AllGlobalExpressionParameters => 
         this.AllParameters()
            .Except(AllGlobalMoleculeParameters)
            .Except(AllOntogenyParameters)
            .Except(AllDiseaseStateParameters)
            .ToList();

      public override void UpdatePropertiesFrom(IUpdatable sourceObject, ICloneManager cloneManager)
      {
         base.UpdatePropertiesFrom(sourceObject, cloneManager);
         var sourceMolecule = sourceObject as IndividualMolecule;
         if (sourceMolecule == null) return;

         QueryConfiguration = sourceMolecule.QueryConfiguration;
         MoleculeType = sourceMolecule.MoleculeType;
         var sourceOntogeny = sourceMolecule.Ontogeny;

         Ontogeny = sourceOntogeny.IsUserDefined() ? cloneManager.Clone(sourceOntogeny) : sourceMolecule.Ontogeny;
      }

      public virtual Ontogeny Ontogeny
      {
         get => _ontogeny;
         set => SetProperty(ref _ontogeny, value);
      }
   }

   public class NullIndividualMolecule : IndividualMolecule
   {
      public override string ToString()
      {
         return PKSimConstants.UI.None;
      }
   }
}