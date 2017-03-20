using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using OSPSuite.Utility.Extensions;
using OSPSuite.Utility.Visitor;
using PKSim.Core;
using PKSim.Core.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Presenters.ProteinExpression;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;

namespace PKSim.Infrastructure.ProjectConverter.v6_0
{
   public class Converter601To602 : IObjectConverter,
      IVisitor<Individual>,
      IVisitor<Simulation>,
      IVisitor<IPKSimProject>
   {
      private readonly INeighborhoodFinalizer _neighborhoodFinalizer;
      private readonly IIndividualUpdater _individualUpdater;

      public Converter601To602(INeighborhoodFinalizer neighborhoodFinalizer, IIndividualUpdater individualUpdater)
      {
         _neighborhoodFinalizer = neighborhoodFinalizer;
         _individualUpdater = individualUpdater;
      }

      public bool IsSatisfiedBy(int version)
      {
         return version == ProjectVersions.V6_0_1;
      }

      public int Convert(object objectToConvert, int originalVersion)
      {
         this.Visit(objectToConvert);
         return ProjectVersions.V6_0_2;
      }

      public int ConvertXml(XElement element, int originalVersion)
      {
         return ProjectVersions.V6_0_2;
      }

      private void updateDatabaseQueryStringToLiverZones(IndividualMolecule molecule)
      {
         var dataset = new DataSet();

         var rootElement = XElement.Load(new StringReader(molecule.QueryConfiguration));
         var expressionDataSetElement = rootElement.Element(CoreConstants.Serialization.ExpressionDataSet);
         if (expressionDataSetElement == null)
            return;

         dataset.ReadFromXmlString(expressionDataSetElement.Value);

         var mappingTable = dataset.Tables[DatabaseConfiguration.TableNames.MAPPING_DATA];
         if (mappingTable == null) return;

         var periporalRows = new List<DataRow>();

         //We find the old mapping for the organ liver 
         foreach (DataRow row in mappingTable.Rows)
         {
            if (row[DatabaseConfiguration.MappingColumns.COL_CONTAINER].ToString() == CoreConstants.Organ.Liver)
            {
               //store a new row for Periportal that will be added in the mapping
               var periportalRow = mappingTable.NewRow();
               periportalRow[DatabaseConfiguration.MappingColumns.COL_CONTAINER] = CoreConstants.Compartment.Periportal;
               periportalRow[DatabaseConfiguration.MappingColumns.COL_TISSUE] = row[DatabaseConfiguration.MappingColumns.COL_TISSUE];

               //this becomes now pericentral
               row[DatabaseConfiguration.MappingColumns.COL_CONTAINER] = CoreConstants.Compartment.Pericentral;

               periporalRows.Add(periportalRow);
            }
         }

         //add after the first iteration to avoid enumerating and changing the enumeration at the same time
         foreach (var row in periporalRows)
         {
            mappingTable.Rows.Add(row);
         }

         expressionDataSetElement.Value = dataset.SaveToXmlString();
         molecule.QueryConfiguration = rootElement.ToString(SaveOptions.DisableFormatting);
      }

      public void Visit(Individual individual)
      {
         convertIndividual(individual);
      }

      private void convertIndividual(Individual individual)
      {
         if (individual == null) return;

         individual.AllMolecules().SelectMany(m => m.AllExpressionsContainers())
            .Select(c => c.RelativeExpressionParameter)
            .Each(p => p.Dimension = Constants.Dimension.NO_DIMENSION);

         individual.AllMolecules().Where(x => x.HasQuery())
            .Each(updateDatabaseQueryStringToLiverZones);

         _individualUpdater.AddScalingExponentForFluidFlowTo(individual);
      }

      public void Visit(Simulation simulation)
      {
         convertIndividual(simulation.Individual);
         finalizeIndividual(simulation.Individual);
      }

      private void finalizeIndividual(Individual individual)
      {
         if (individual == null)
            return;

         var firstNeighborhood = individual.Neighborhoods.GetChildren<INeighborhood>().FirstOrDefault();
         if (firstNeighborhood == null || firstNeighborhood.FirstNeighbor != null) return;

         _neighborhoodFinalizer.SetNeighborsIn(individual);
      }

      public void Visit(IPKSimProject project)
      {
         foreach (var observedData in project.AllObservedData)
         {
            var baseGrid = observedData.BaseGrid;
            var baseGridName = baseGrid.Name.Replace(ObjectPath.PATH_DELIMITER, "\\");
            baseGrid.QuantityInfo = new QuantityInfo(baseGrid.Name, new[] {observedData.Name, baseGridName}, QuantityType.Time);
         }
      }
   }
}