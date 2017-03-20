using System;
using PKSim.Assets;
using OSPSuite.Utility.Extensions;

namespace PKSim.Core.Services
{
   public class IndividualProteinFactoryNotFoundException : Exception
   {
      private static readonly string _errorMessage = PKSimConstants.Error.ProteinExpressionFactoryNotFound;

      public IndividualProteinFactoryNotFoundException(Type proteinExpressionType) : base(_errorMessage.FormatWith(proteinExpressionType))
      {
      }
   }
}