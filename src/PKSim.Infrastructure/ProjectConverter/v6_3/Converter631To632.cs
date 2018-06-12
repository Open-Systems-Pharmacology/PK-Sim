using System.Xml.Linq;
using OSPSuite.Utility.Visitor;
using PKSim.Core;
using PKSim.Core.Model;
using OSPSuite.Core.Domain;

namespace PKSim.Infrastructure.ProjectConverter.v6_3
{
   public class Converter631To632 : IObjectConverter, IVisitor<Simulation>
   {
      private bool _converted;

      public bool IsSatisfiedBy(int version) => version == ProjectVersions.V6_3_1;

      public (int convertedToVersion, bool conversionHappened) Convert(object objectToConvert, int originalVersion)
      {
         _converted = false;
         this.Visit(objectToConvert);
         return (ProjectVersions.V6_3_2, _converted);
      }

      public void Visit(Simulation simulation)
      {
         foreach (var container in simulation.Model.Root.GetChildren<IContainer>(c=>c.ContainerType==ContainerType.Molecule))
         {
            var ontogenyFactor = container.Parameter(CoreConstants.Parameters.ONTOGENY_FACTOR);
            if (ontogenyFactor != null)
               ontogenyFactor.CanBeVaried = true;

            var ontogenyFactorGI = container.Parameter(CoreConstants.Parameters.ONTOGENY_FACTOR_GI);
            if (ontogenyFactorGI != null)
               ontogenyFactorGI.CanBeVaried = true;
         }
         _converted = true;
      }

      public (int convertedToVersion, bool conversionHappened) ConvertXml(XElement element, int originalVersion)
      {
         return (ProjectVersions.V6_3_2, false);
      }
   }
}
