namespace PKSim.Core.Model
{
   public class TransportPartialProcess : PartialProcess
   {
      public override string GetProcessClass()
      {
         return CoreConstants.ProcessClasses.TRANSPORT;
      }
   }

   public class TransportPartialProcessWithSpecies : TransportPartialProcess, ISpeciesDependentCompoundProcess
   {
   }
}