using System.Threading.Tasks;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Services;
using OSPSuite.Utility.Collections;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Services;

namespace PKSim.Core.Snapshots.Mappers
{
   public class ExpressionContainerMapperContext
   {
      public IndividualMolecule Molecule { get; set; }
      public ISimulationSubject SimulationSubject { get; set; }

      //This is required to speed up process and ONLY for older format
      public ICache<string, IParameter> ExpressionParameters { get; set; }
   }

   public class ExpressionContainerMapper : SnapshotMapperBase<MoleculeExpressionContainer, ExpressionContainer, ExpressionContainerMapperContext>
   {
      private readonly ParameterMapper _parameterMapper;
      private readonly ITransportContainerUpdater _transportContainerUpdater;
      private readonly IOSPLogger _logger;

      public ExpressionContainerMapper(
         ParameterMapper parameterMapper,
         ITransportContainerUpdater transportContainerUpdater,
         IOSPLogger logger)
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

         var snapshot = await SnapshotFrom(expressionContainer, x => { x.Name = expressionContainer.Name; });
         mapTransporterExpressionProperties(snapshot, transportedExpressionContainer);

         return snapshot;
      }

      private void mapTransporterExpressionProperties(ExpressionContainer snapshot, TransporterExpressionContainer transporterExpressionContainer)
      {
         if (transporterExpressionContainer == null)
            return;

         snapshot.TransportDirection = transporterExpressionContainer.TransportDirection.TransportDirectionId;
      }

      public override async Task<MoleculeExpressionContainer> MapToModel(ExpressionContainer snapshot, ExpressionContainerMapperContext context)
      {
         if (snapshot == null)
            return null;

         var molecule = context.Molecule;
         var individual = context.SimulationSubject;
         var expressionParameterCache = context.ExpressionParameters;

         //Value was only defined for older version of the snapshot
         if (!snapshot.Value.HasValue)
            return null;

         var relExp = expressionParameterCache[snapshot.Name];
         if (relExp == null)
            return null;


         await _parameterMapper.MapToModel(snapshot, relExp);

         //TODO 
         return null;
         // if (!(molecule is IndividualTransporter transporter))
         //    return expressionContainer;


         // var expressionContainer = molecule.ExpressionContainer(snapshot.Name);
         // if (expressionContainer == null)
         // {
         //    _logger.AddWarning(PKSimConstants.Error.RelativeExpressionContainerNotFound(snapshot.Name));
         //    return null;
         // }
         //
         // var expressionParameter = expressionContainer.RelativeExpressionParameter;
         // await _parameterMapper.MapToModel(snapshot, expressionParameter);
         //
         // if (!(molecule is IndividualTransporter transporter))
         //    return expressionContainer;
         //
         // var species = individual.Species.Name;
         // var transporterExpressionContainer = expressionContainer.DowncastTo<TransporterExpressionContainer>();
         // var membraneLocation = ModelValueFor(snapshot.MembraneLocation, MembraneLocation.Basolateral);
         // _transportContainerUpdater.UpdateTransporterFromTemplate(transporterExpressionContainer, species, membraneLocation, transporter.TransportType);
         //
         // return expressionContainer;
      }
   }
}