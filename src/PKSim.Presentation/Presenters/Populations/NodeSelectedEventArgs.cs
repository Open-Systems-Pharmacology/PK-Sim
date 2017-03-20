using System;
using OSPSuite.Presentation.Nodes;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Presentation.Nodes;
using OSPSuite.Core.Domain;

namespace PKSim.Presentation.Presenters.Populations
{
   public class NodeSelectedEventArgs<TNode> : EventArgs where TNode : ITreeNode
   {
      public TNode Node { get; protected set; }

      public NodeSelectedEventArgs(TNode node)
      {
         Node = node;
      }
   }

   public class NodeSelectedEventArgs : NodeSelectedEventArgs<ITreeNode>
   {
      public NodeSelectedEventArgs(ITreeNode node) : base(node)
      {
      }
   }

   public class CovariateNodeSelectedEventArgs : NodeSelectedEventArgs<CovariateNode>
   {
      public CovariateNodeSelectedEventArgs(CovariateNode node) : base(node)
      {
      }

      public string Covariate => Node.Id;
   }

   public class ParameterNodeSelectedEventArgs : NodeSelectedEventArgs<ITreeNode<IParameter>>
   {
      public IParameter Parameter => Node.Tag;

      public ParameterNodeSelectedEventArgs(ITreeNode<IParameter> node)
         : base(node)
      {
      }
   }

   public class ParameterFieldSelectedEventArgs : EventArgs
   {
      public IParameter Parameter { get; private set; }
      public PopulationAnalysisParameterField ParameterField { get; private set; }

      public ParameterFieldSelectedEventArgs(IParameter parameter, PopulationAnalysisParameterField parameterField)
      {
         Parameter = parameter;
         ParameterField = parameterField;
      }
   }

   public class PKParameterDoubleClickedEventArgs : EventArgs
   {
      public QuantityPKParameter QuantityPKParameter { get; private set; }

      public PKParameterDoubleClickedEventArgs(QuantityPKParameter quantityPKParameter)
      {
         QuantityPKParameter = quantityPKParameter;
      }
   }

   public class PKParameterSelectedEventArgs : EventArgs
   {
      public QuantityPKParameter PKParameter { get; private set; }

      public PKParameterSelectedEventArgs(QuantityPKParameter pkParameter)
      {
         PKParameter = pkParameter;
      }
   }

   public class PKParameterFieldSelectedEventArgs : PKParameterSelectedEventArgs
   {
      public PopulationAnalysisPKParameterField PKParameterField { get; private set; }

      public PKParameterFieldSelectedEventArgs(QuantityPKParameter pkParameter, PopulationAnalysisPKParameterField pkParameterField) : base(pkParameter)
      {
         PKParameterField = pkParameterField;
      }
   }

   public class CovariateFieldSelectedEventArgs : EventArgs
   {
      public string Covariate { get; private set; }
      public PopulationAnalysisCovariateField CovariateField { get; private set; }

      public CovariateFieldSelectedEventArgs(string covariate, PopulationAnalysisCovariateField covariateField)
      {
         Covariate = covariate;
         CovariateField = covariateField;
      }
   }

   public class DerivedFieldSelectedEventArgs : EventArgs
   {
      public PopulationAnalysisDerivedField DerivedField { get; private set; }

      public DerivedFieldSelectedEventArgs(PopulationAnalysisDerivedField populationAnalysisDerivedField)
      {
         DerivedField = populationAnalysisDerivedField;
      }
   }
}