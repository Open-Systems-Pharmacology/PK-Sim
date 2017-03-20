using PKSim.Core.Model;
using PKSim.Presentation.DTO.Parameters;
using OSPSuite.Presentation.DTO;

namespace PKSim.Presentation.DTO.Protocols
{
   public class SimpleProtocolDTO : ValidatableDTO<SimpleProtocol>
   {
      private readonly SimpleProtocol _simpleProtocol;
      public IParameterDTO Dose { get; set; }
      public IParameterDTO EndTime { get; set; }

      public SimpleProtocolDTO(SimpleProtocol simpleProtocol) : base(simpleProtocol)
      {
         _simpleProtocol = simpleProtocol;
      }

      public ApplicationType ApplicationType
      {
         get { return _simpleProtocol.ApplicationType; }
         set{/*nothing to do here*/}
      }

      public DosingInterval DosingInterval
      {
         get { return _simpleProtocol.DosingInterval; }
         set
         {
            /*nothing to do here*/
         }
      }

      public string TargetCompartment
      {
         get { return _simpleProtocol.TargetCompartment; }
         set
         {
            /*nothing to do here*/
         }
      }

      public string TargetOrgan
      {
         get { return _simpleProtocol.TargetOrgan; }
         set
         {
            /*nothing to do here*/
         }
      }
   }
}