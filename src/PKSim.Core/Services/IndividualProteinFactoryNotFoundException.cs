using System;
using PKSim.Assets;

namespace PKSim.Core.Services
{
   public class IndividualProteinFactoryNotFoundException : Exception
   {
      public IndividualProteinFactoryNotFoundException(Type proteinExpressionType) : base(PKSimConstants.Error.ProteinExpressionFactoryNotFound(proteinExpressionType.ToString()))
      {
      }
   }
}