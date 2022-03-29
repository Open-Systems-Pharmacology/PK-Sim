using OSPSuite.Utility.Extensions;

namespace PKSim.Core.Model
{
   public class ParameterDistributionMetaData : ParameterMetaData, IDistributionMetaData
   {
      public string ParameterValueVersion { get; set; }
      public string Population { get; set; }
      public string Gender { get; set; }
      public double Age { get; set; }
      public double GestationalAge { get; set; }
      public string DistributionType { get; set; }
      public double Mean { get; set; }
      public double Deviation { get; set; }

      public DistributionType Distribution
      {
         get => DistributionTypes.ById(DistributionType);
         set => DistributionType = value.Id;
      }

      public static ParameterDistributionMetaData From(ParameterDistributionMetaData source)
      {
         return source.MemberwiseClone().DowncastTo<ParameterDistributionMetaData>();
      }

      public void UpdateFrom(IDistributionMetaData distributionMetaData)
      {
         Mean = distributionMetaData.Mean;
         Deviation = distributionMetaData.Deviation;
         Distribution = distributionMetaData.Distribution;
      }

      public override void UpdatePropertiesFrom(ParameterMetaData parameterMetaData)
      {
         base.UpdatePropertiesFrom(parameterMetaData);
         if (!(parameterMetaData is ParameterDistributionMetaData parameterDistributionMetaData)) return;
         ParameterValueVersion = parameterDistributionMetaData.ParameterValueVersion;
         Population = parameterDistributionMetaData.Population;
         Gender = parameterDistributionMetaData.Gender;
         Age = parameterDistributionMetaData.Age;
         GestationalAge = parameterDistributionMetaData.GestationalAge;
         DistributionType = parameterDistributionMetaData.DistributionType;
         Mean = parameterDistributionMetaData.Mean;
         Deviation = parameterDistributionMetaData.Deviation;
      }
   }
}