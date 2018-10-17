using System.Xml.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Extensions;
using OSPSuite.Utility.Visitor;
using PKSim.Core;
using PKSim.Core.Model;

namespace PKSim.Infrastructure.ProjectConverter.v7_4
{
   public class Converter730To740 : IObjectConverter,
      IVisitor<Simulation>
   {
      private bool _converted;

      public bool IsSatisfiedBy(int version)
      {
         return version == ProjectVersions.V7_3_0;
      }

      public (int convertedToVersion, bool conversionHappened) Convert(object objectToConvert, int originalVersion)
      {
         _converted = false;
         this.Visit(objectToConvert);
         return (ProjectVersions.V7_4_0, _converted);
      }

      public (int convertedToVersion, bool conversionHappened) ConvertXml(XElement element, int originalVersion)
      {
         return (ProjectVersions.V7_4_0, false);
      }

      public void Visit(Simulation simulation)
      {
         convertSimulation(simulation);
      }

      private void convertSimulation(Simulation simulation)
      {
         var allTabletTimeDelayFactorParameters = simulation.Model.Root.GetAllChildren<IParameter>(x => x.IsNamed(ConverterConstants.Parameter.TabletTimeDelayFactor));
         allTabletTimeDelayFactorParameters.Each(p =>
         {
            p.Visible = true;
            p.Info.ReadOnly = false;
            //just to make sure it's the last parmaeter visible 
            p.Sequence = 20;
            _converted = true;

         });
      }
   }
}