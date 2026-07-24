using OSPSuite.Core.Domain;

namespace PKSim.Core.Model;

public class ClassifiableProtocol : Classifiable<Protocol>
{
   public ClassifiableProtocol() : base(ClassificationType.Protocol)
   {
   }

   public Protocol Protocol => Subject;
}
