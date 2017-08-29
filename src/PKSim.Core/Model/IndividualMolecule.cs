using System.Collections.Generic;
using PKSim.Assets;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;

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

      public IParameter OntogenyFactorParameter => this.Parameter(CoreConstants.Parameter.ONTOGENY_FACTOR);

      public virtual double OntogenyFactorGI
      {
         get => OntogenyFactorGIParameter.Value;
         set => OntogenyFactorGIParameter.Value = value;
      }

      public IParameter OntogenyFactorGIParameter => this.Parameter(CoreConstants.Parameter.ONTOGENY_FACTOR_GI);

      public virtual bool HasContainerNamed(string expressionContainerName)
      {
         return ExpressionContainer(expressionContainerName) != null;
      }
         
      public virtual MoleculeExpressionContainer ExpressionContainer(string expressionContainerName)
      {
         return AllExpressionsContainers().FindByName(expressionContainerName);
      }

      public virtual bool HasQuery()
      {
         return !string.IsNullOrEmpty(QueryConfiguration);
      }

      public virtual IReadOnlyList<MoleculeExpressionContainer> AllExpressionsContainers() => GetAllChildren<MoleculeExpressionContainer>();

      public virtual IParameter ReferenceConcentration => this.Parameter(CoreConstants.Parameter.REFERENCE_CONCENTRATION);

      public virtual IParameter HalfLifeLiver => this.Parameter(CoreConstants.Parameter.HALF_LIFE_LIVER);

      public virtual IParameter HalfLifeIntestine => this.Parameter(CoreConstants.Parameter.HALF_LIFE_INTESTINE);

      public virtual IParameter GetRelativeExpressionParameterFor(string expressionContainerName)
      {
         return ExpressionContainer(expressionContainerName)?.RelativeExpressionParameter;
      }

      public virtual IParameter GetRelativeExpressionNormParameterFor(string expressionContainerName)
      {
         return ExpressionContainer(expressionContainerName)?.RelativeExpressionNormParameter;
      }

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