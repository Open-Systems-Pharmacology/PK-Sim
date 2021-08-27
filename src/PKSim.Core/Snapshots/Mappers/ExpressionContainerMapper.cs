using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Services;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Core.Snapshots.Services;

namespace PKSim.Core.Snapshots.Mappers
{
   public class ExpressionContainerMapperContext
   {
      public IndividualMolecule Molecule { get; set; }

      //This is required to speed up process and ONLY for older format
      public ICache<string, IParameter> ExpressionParameters { get; set; }

      public IReadOnlyList<MoleculeExpressionContainer> MoleculeExpressionContainers { get; set; }
   }

   public class ExpressionContainerMapper : SnapshotMapperBase<MoleculeExpressionContainer, ExpressionContainer, ExpressionContainerMapperContext>
   {
      private readonly ParameterMapper _parameterMapper;
      private readonly ITransportContainerUpdater _transportContainerUpdater;
      private readonly IOSPSuiteLogger _logger;

      public ExpressionContainerMapper(
         ParameterMapper parameterMapper,
         ITransportContainerUpdater transportContainerUpdater,
         IOSPSuiteLogger logger)
      {
         _parameterMapper = parameterMapper;
         _transportContainerUpdater = transportContainerUpdater;
         _logger = logger;
      }

      public override async Task<ExpressionContainer> MapToSnapshot(MoleculeExpressionContainer expressionContainer)
      {
         var transportedExpressionContainer = expressionContainer as TransporterExpressionContainer;
         if (transportedExpressionContainer == null)
            return null;

         if (transportedExpressionContainer.TransportDirection == TransportDirectionId.None)
            return null;

         var snapshot = await SnapshotFrom(expressionContainer, x =>
         {
            x.Name = string.IsNullOrEmpty(expressionContainer.LogicalContainerName) ? expressionContainer.Name : expressionContainer.LogicalContainerName;
            x.CompartmentName = expressionContainer.CompartmentName;
            x.TransportDirection = transportedExpressionContainer.TransportDirection;
         });

         return snapshot;
      }

      public override async Task<MoleculeExpressionContainer> MapToModel(ExpressionContainer snapshot, ExpressionContainerMapperContext context)
      {
         if (snapshot == null)
            return null;

         var molecule = context.Molecule;
         var expressionParameterCache = context.ExpressionParameters;
         var expressionContainerParameters = context.MoleculeExpressionContainers;

         //Value was only defined for older version of the snapshot
         if (snapshot.Value.HasValue)
         {
            var relExp = expressionParameterCache[snapshot.Name];
            if (relExp == null)
            {
               _logger.AddWarning(PKSimConstants.Error.RelativeExpressionContainerNotFound(snapshot.Name));
               return null;
            }

            await _parameterMapper.MapToModel(snapshot, relExp);
         }


         //We do not return anything as container are created at construction time
         if (!(molecule is IndividualTransporter transporter))
            return null;

         var isV9Format = !snapshot.TransportDirection.HasValue;

         var expressionsContainers = expressionContainerParameters.OfType<TransporterExpressionContainer>()
            .Where(queryPredicate(isV9Format, snapshot))
            .Where(x => x.TransportDirection != TransportDirectionId.None)
            .ToList();

         if (expressionsContainers.Count == 0)
         {
            _logger.AddWarning(PKSimConstants.Error.RelativeExpressionContainerNotFound(snapshot.Name));
            return null;
         }

         var firstExpressionContainer = expressionsContainers[0];
         //This is the new format
         if (!isV9Format)
         {
            //we should only have one
            firstExpressionContainer.TransportDirection = snapshot.TransportDirection.Value;
            return firstExpressionContainer;
         }

         expressionsContainers.Each(x => x.TransportDirection = TransportDirections.DefaultDirectionFor(transporter.TransportType, x));

         //only one direction to take into consideration. all good, nothing else to do
         if (expressionsContainers.Count == 1)
            return firstExpressionContainer;

         //This is the old format. Basolateral was the default
         var membraneLocation = ModelValueFor(snapshot.MembraneLocation, MembraneLocation.Basolateral);

         MembraneLocationConverter.ConvertMembraneLocationToParameterFraction(expressionsContainers, membraneLocation);
         return firstExpressionContainer;
      }

      private Func<TransporterExpressionContainer, bool> queryPredicate(bool isV9Format, ExpressionContainer snapshot)
      {
         if (isV9Format)
            return x => x.LogicalContainerName == snapshot.Name || x.CompartmentName == snapshot.Name;

         //This is a real compartment
         return x => (x.LogicalContainerName == snapshot.Name && x.CompartmentName == snapshot.CompartmentName) ||
                     // this is a surrogate compartment. 
                     (string.IsNullOrEmpty(x.LogicalContainerName) && x.Name == snapshot.Name);
      }
   }
}