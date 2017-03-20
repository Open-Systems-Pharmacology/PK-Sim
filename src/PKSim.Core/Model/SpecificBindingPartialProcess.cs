namespace PKSim.Core.Model
{
   public class SpecificBindingPartialProcess : PartialProcess
   {
      public override string GetProcessClass()
      {
         return CoreConstants.ProcessClasses.SPECIFIC_BINDING;
      }
   }
}