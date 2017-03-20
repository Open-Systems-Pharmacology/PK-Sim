using PKSim.Core.Model;

namespace PKSim.Infrastructure.Reporting.TeX.Items
{
   public class Histogram
   {
      public IVectorialParametersContainer ParameterContainer { get; private set; }
      public ParameterDistributionSettings ParameterDistributionSettings { get; private set; }

      public Histogram(IVectorialParametersContainer parameterContainer, ParameterDistributionSettings parameterDistributionSettings)
      {
         ParameterContainer = parameterContainer;
         ParameterDistributionSettings = parameterDistributionSettings;
      }
   }
}
