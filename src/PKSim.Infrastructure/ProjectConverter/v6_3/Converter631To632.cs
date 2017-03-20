using System.Xml.Linq;
using OSPSuite.Utility.Visitor;
using PKSim.Core;
using PKSim.Core.Model;
using OSPSuite.Core.Domain;

namespace PKSim.Infrastructure.ProjectConverter.v6_3
{
   public class Converter631To632 : IObjectConverter, IVisitor<Simulation>
   {
      public bool IsSatisfiedBy(int version)
      {
         return version == ProjectVersions.V6_3_1;
      }

      public int Convert(object objectToConvert, int originalVersion)
      {
         this.Visit(objectToConvert);
         return ProjectVersions.V6_3_2;
      }

      public void Visit(Simulation simulation)
      {
         foreach (var container in simulation.Model.Root.GetChildren<IContainer>(c=>c.ContainerType==ContainerType.Molecule))
         {
            var ontogenyFactor = container.Parameter(CoreConstants.Parameter.ONTOGENY_FACTOR);
            if (ontogenyFactor != null)
               ontogenyFactor.CanBeVaried = true;

            var ontogenyFactorGI = container.Parameter(CoreConstants.Parameter.ONTOGENY_FACTOR_GI);
            if (ontogenyFactorGI != null)
               ontogenyFactorGI.CanBeVaried = true;
         }
      }

      public int ConvertXml(XElement element, int originalVersion)
      {
         return ProjectVersions.V6_3_2;
      }
   }
}
