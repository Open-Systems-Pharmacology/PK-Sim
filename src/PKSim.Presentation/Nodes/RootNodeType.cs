using PKSim.Assets;
using OSPSuite.Assets;
using OSPSuite.Presentation.Presenters.Nodes;

namespace PKSim.Presentation.Nodes
{
   public static class PKSimRootNodeTypes
   {
      public static readonly RootNodeType Absorption = new RootNodeType(PKSimConstants.UI.Absorption, ApplicationIcons.Absorption);
      public static readonly RootNodeType InhibitionProcess = new RootNodeType(PKSimConstants.UI.Inhibition, ApplicationIcons.Inhibition);
      public static readonly RootNodeType InductionProcess = new RootNodeType(PKSimConstants.UI.Induction, ApplicationIcons.Induction);
      public static readonly RootNodeType Distribution = new RootNodeType(PKSimConstants.UI.Distribution, ApplicationIcons.Distribution);
      public static readonly RootNodeType IndividualFolder = new RootNodeType(PKSimConstants.UI.IndividualFolder, ApplicationIcons.IndividualFolder);
      public static readonly RootNodeType CompoundFolder = new RootNodeType(PKSimConstants.UI.CompoundFolder, ApplicationIcons.CompoundFolder);
      public static readonly RootNodeType ProtocolFolder = new RootNodeType(PKSimConstants.UI.AdministrationProtocolFolder, ApplicationIcons.ProtocolFolder);
      public static readonly RootNodeType FormulationFolder = new RootNodeType(PKSimConstants.UI.FormulationFolder, ApplicationIcons.FormulationFolder);
      public static readonly RootNodeType PopulationFolder = new RootNodeType(PKSimConstants.UI.PopulationFolder, ApplicationIcons.PopulationFolder);
      public static readonly RootNodeType EventFolder = new RootNodeType(PKSimConstants.UI.EventFolder, ApplicationIcons.EventFolder);
      public static readonly RootNodeType ObserversFolder = new RootNodeType(PKSimConstants.UI.ObserversFolder, ApplicationIcons.ObserverFolder);
      public static readonly RootNodeType IndividualMetabolizingEnzymes = new RootNodeType(PKSimConstants.UI.MetabolizingEnzymes, ApplicationIcons.Enzyme);
      public static readonly RootNodeType IndividualProteinBindingPartners = new RootNodeType(PKSimConstants.UI.ProteinBindingPartners, ApplicationIcons.Protein);
      public static readonly RootNodeType IndividualTransportProteins = new RootNodeType(PKSimConstants.UI.TransportProteins, ApplicationIcons.Transporter);
      public static readonly RootNodeType MetabolicProcesses = new RootNodeType(PKSimConstants.UI.MetabolicProcesses, ApplicationIcons.Metabolism);
      public static readonly RootNodeType SpecificBindingProcesses = new RootNodeType(PKSimConstants.UI.SpecificBindingProcesses, ApplicationIcons.SpecificBinding);
      public static readonly RootNodeType CompoundProteinBindingPartners = new RootNodeType(PKSimConstants.UI.ProteinBindingPartners, ApplicationIcons.Protein);
      public static readonly RootNodeType TransportAndExcretionProcesses = new RootNodeType(PKSimConstants.UI.TransportAndExcretionProcesses, ApplicationIcons.Transport);
      public static readonly RootNodeType CompoundMetabolizingEnzymes = new RootNodeType(PKSimConstants.UI.MetabolizingEnzymes, ApplicationIcons.Enzyme);
      public static readonly RootNodeType CompoundTransportProteins = new RootNodeType(PKSimConstants.UI.TransportProteins, ApplicationIcons.Transporter);

   }
}