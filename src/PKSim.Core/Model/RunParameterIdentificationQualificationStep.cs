using OSPSuite.Core.Domain.ParameterIdentifications;
using PKSim.Assets;

namespace PKSim.Core.Model
{
   public class RunParameterIdentificationQualificationStep : IQualificationStep
   {
      public ParameterIdentification ParameterIdentification { get; set; }

      public string Display => PKSimConstants.QualificationSteps.RunParameterIdentification(ParameterIdentification.Name);
   }
}