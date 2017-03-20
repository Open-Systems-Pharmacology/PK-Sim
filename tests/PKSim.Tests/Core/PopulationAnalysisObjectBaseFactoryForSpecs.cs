using System;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model.PopulationAnalyses;
using OSPSuite.Core.Domain;

namespace PKSim.Core
{
   internal class PopulationAnalysisObjectBaseFactoryForSpecs : IObjectBaseFactory
   {
      public T Create<T>() where T : IObjectBase
      {
         //nothing to do here
         return default(T);
      }

      public T Create<T>(string id) where T : IObjectBase
      {
         return Create<T>();
      }

      public T CreateObjectBaseFrom<T>(Type objectType)
      {
         if (objectType.IsAnImplementationOf<PopulationAnalysisPKParameterField>())
            return new PopulationAnalysisPKParameterField().DowncastTo<T>();

         if (objectType.IsAnImplementationOf<PopulationAnalysisParameterField>())
            return new PopulationAnalysisParameterField().DowncastTo<T>();

         return default(T);
      }

      public T CreateObjectBaseFrom<T>(Type objectType, string id)
      {
         return default(T);
      }

      public T CreateObjectBaseFrom<T>(T sourceObject)
      {
         return CreateObjectBaseFrom<T>(sourceObject.GetType());
      }
   }
}