using System;
using System.Linq;
using System.Threading.Tasks;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Extensions;
using OSPSuite.Utility.Container;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Infrastructure;
using IContainer = OSPSuite.Core.Domain.IContainer;
using ISimulationPersistableUpdater = PKSim.Core.Services.ISimulationPersistableUpdater;

namespace PKSim.IntegrationTests
{
   public abstract class concern_for_SimulationWithParticlesFormulation : concern_for_IndividualSimulation
   {
      protected Formulation _formulation;
      protected string[] _lumenSegments = CoreConstants.Compartment.LumenSegmentsStomachToRectum.ToArray();

      protected float _appliedDrugMass; //initial drug mass of the application

      //---- simulation outputs
      protected float[] _times;                              //simulation times
      protected float[][] _dissolvedDrugLumen;               //dissolved drug[lumen segment][time]
      protected float[][] _binSolidDrug;                     //solid drug[particles bin][time]
      protected float[][] _binInsolubleDrug;                 //insoluble drug[particles bin][time]
      protected float[][][] _binSolidDrugPerSegment;         //solid drug[particles bin][lumen segment][time]
      protected float[][][] _binParticlesFractionPerSegment; //no of particles fraction[particles bin][lumen segment][time]

      public override void GlobalContext()
      {
         base.GlobalContext();
         _compound.Name = "C1";

         _protocol = DomainFactoryForSpecs.CreateStandardOralProtocol();
         _formulation = DomainFactoryForSpecs.CreateParticlesFormulation(NumberOfBins);

         //user defined actions in the building blocks
         SetupBuildingBlocks();

         _simulation = DomainFactoryForSpecs.CreateModelLessSimulationWith(
            _individual, _compound, _protocol, allowAging:false, formulation: _formulation) as IndividualSimulation;

         //setup compound processes
         SetupProcesses();

         DomainFactoryForSpecs.AddModelToSimulation(_simulation);

         //user defined actions in simulations, different for every test scenario
         SetupSimulation();

         if (DisableIntestinalAbsorptionAndLuminalFlow)
         {
            //disable intestinal absorption and luminal flow to feces for easier mass balance checks
            MoleculeProperties(CoreConstants.Parameters.INTESTINAL_PERMEABILITY).Value = 0;
            IntestinalTransitRateFor(CoreConstants.Compartment.Rectum).Value = 0;
         }

         //store initial drug mass
         _appliedDrugMass = Application.Container(CoreConstants.ContainerName.ProtocolSchemaItem)
                            .Parameter(CoreConstantsForSpecs.Parameter.DRUG_MASS).Value.ToFloat();

         //Get paths for output quantities of interest
         (var lumenPaths, var binSolidDrugPaths, var binInsolubleDrugPaths,
            var binSolidDrugPerSegmentPaths, var binParticlesFractionPerSegmentPaths) = outputPaths(NumberOfBins);

         //add quantities of interest to the simulation outputs
         addOutputs(lumenPaths, binSolidDrugPaths, binInsolubleDrugPaths, binSolidDrugPerSegmentPaths, binParticlesFractionPerSegmentPaths);

         RunSimulation(_simulation).Wait();

         //fill simulation output times and values required for all further tests
         fillSimulationOutputs(lumenPaths, binSolidDrugPaths, binInsolubleDrugPaths, binSolidDrugPerSegmentPaths, binParticlesFractionPerSegmentPaths);
      }

      protected Task RunSimulation(IndividualSimulation individualSimulation)
      {
         var simulationPersistableUpdater = IoC.Resolve<ISimulationPersistableUpdater>();
         simulationPersistableUpdater.UpdatePersistableFromSettings(individualSimulation);

         var simulationEngine = IoC.Resolve<ISimulationEngine<IndividualSimulation>>();
         return simulationEngine.RunAsync(individualSimulation, _simulationRunOptions);
      }

      protected virtual bool DisableIntestinalAbsorptionAndLuminalFlow => true;

      protected IContainer Application => _simulation.Model.Root.Container("Applications").Container(_protocol.Name)
                                          .Container(_formulation.Name).Container("Application_1");

      protected IContainer ParticleBin(int binIndex)
      {
         return Application.Container($"ParticleBin_{binIndex + 1}"); //in the model bin inexation starts with 1
      }

      protected IParameter StartParticleRadius(int binIndex)
      {
         return ParticleBin(binIndex).Parameter(CoreConstants.Parameters.START_PARTICLE_RADIUS);
      }

      protected IContainer Lumen => _simulation.Model.Root.Container("Organism").Container(CoreConstants.Organ.Lumen);

      protected IParameter Solubility(string segment) => Lumen.Container(segment).Container(_compound.Name).Parameter(CoreConstants.Parameters.SOLUBILITY);

      protected abstract int NumberOfBins { get; }

      protected abstract void SetupSimulation();

      protected virtual void SetupBuildingBlocks() { }

      protected virtual void SetupProcesses() { }

      protected bool PrecipitatedDrugSoluble
      {
         get => MoleculeProperties(Constants.Parameters.PRECIPITATED_DRUG_SOLUBLE).Value == 1;
         set => MoleculeProperties(Constants.Parameters.PRECIPITATED_DRUG_SOLUBLE).Value = value ? 1 : 0;
      }

      protected double ParticleRadiusDissolved
      {
         get => MoleculeProperties(CoreConstantsForSpecs.Parameter.PARTICLE_RADIUS_DISSOLVED).Value;
         set => MoleculeProperties(CoreConstantsForSpecs.Parameter.PARTICLE_RADIUS_DISSOLVED).Value=value;
      }

      protected IParameter IntestinalTransitRateFor(string segment)
      {
         return Lumen.Container(segment).Parameter(CoreConstantsForSpecs.Parameter.INTESTINAL_TRANSIT_RATE_ABSOLUTE);
      }

      /// <summary>
      /// Get values in lumen segments for the combination {particles bin; time index}
      /// </summary>
      /// <returns></returns>
      protected float[] LumenValuesFor(float[][][] results, int binIdx, int timeIdx)
      {
         var values = new float[NumberOfLumenSegments];

         for (int segmentIdx = 0; segmentIdx < NumberOfLumenSegments; segmentIdx++)
            values[segmentIdx] = results[binIdx][segmentIdx][timeIdx];

         return values;
      }

      protected int NumberOfLumenSegments => _lumenSegments.Length;

      protected int NumberOfSimulatedtimePoints => _times.Length;

      protected virtual double ComparisonTolerance => 1e-5;

      private void fillSimulationOutputs(string[] lumenPaths, string[] binSolidDrugPaths, string[] binInsolubleDrugPaths,
                                         string[][] binSolidDrugPerSegmentPaths, string[][] binParticlesFractionPerSegmentPaths)
      {
         int segmentIdx, binIdx;
         _times = _simulation.Results.Time.Values;

         _dissolvedDrugLumen = new float[NumberOfLumenSegments][];
         for (segmentIdx=0; segmentIdx< NumberOfLumenSegments; segmentIdx++)
            _dissolvedDrugLumen[segmentIdx] = valuesFor(lumenPaths[segmentIdx]);

         _binSolidDrug = new float[NumberOfBins][];
         _binInsolubleDrug = new float[NumberOfBins][];
         _binSolidDrugPerSegment = new float[NumberOfBins][][];
         _binParticlesFractionPerSegment = new float[NumberOfBins][][];

         for (binIdx = 0; binIdx < NumberOfBins; binIdx++)
         {
            _binSolidDrug[binIdx] = valuesFor(binSolidDrugPaths[binIdx]);
            _binInsolubleDrug[binIdx] = valuesFor(binInsolubleDrugPaths[binIdx]);

            _binSolidDrugPerSegment[binIdx] = new float[NumberOfLumenSegments][];
            _binParticlesFractionPerSegment[binIdx] = new float[NumberOfLumenSegments][];
            for (segmentIdx = 0; segmentIdx < NumberOfLumenSegments; segmentIdx++)
            {
               _binSolidDrugPerSegment[binIdx][segmentIdx] = valuesFor(binSolidDrugPerSegmentPaths[binIdx][segmentIdx]);
               _binParticlesFractionPerSegment[binIdx][segmentIdx] = valuesFor(binParticlesFractionPerSegmentPaths[binIdx][segmentIdx]);
            }
         }
      }

      private float[] valuesFor(string outputQuantityPath) => ValuesFor(_simulation, outputQuantityPath);

      protected float[] ValuesFor(IndividualSimulation sim, string outputQuantityPath) => sim.Results.AllQuantityValuesFor(outputQuantityPath)[0].Values;

      private void addOutputs(string[] lumenPaths, string[] binSolidDrugPaths, string[] binInsolubleDrugPaths, 
                              string[][] binSolidDrugPerSegmentPaths, string[][] binParticlesFractionPerSegmentPaths)
      {
         var allPaths = lumenPaths.Concat(binSolidDrugPaths).Concat(binInsolubleDrugPaths);

         for (int binIdx = 0; binIdx < NumberOfBins; binIdx++)
         {
            allPaths = allPaths.Concat(binSolidDrugPerSegmentPaths[binIdx]).Concat(binParticlesFractionPerSegmentPaths[binIdx]);
         }

         foreach (var path in allPaths)
         {
            var quantityType = QuantityType.Drug;
            if (path.Contains("Number of particles fraction"))
               quantityType = QuantityType.Parameter;

            AddOutputTo(_simulation, path, quantityType);
         }
      }

      protected void AddOutputTo(IndividualSimulation simulation, string path, QuantityType quantityType = QuantityType.Drug)
      {
         simulation.SimulationSettings.OutputSelections.AddOutput(new QuantitySelection(path, quantityType));
      }

      /// <summary>
      /// Get paths for output quantities of interest. Assumption: we have 1 application with N bins
      /// </summary>
      /// <param name="numberOfBins">Number of bins (>=1)</param>
      /// <returns>
      ///   [lumenPaths] - paths to dissolved drug amounts in lumen segments
      ///   e.g. "Organism|Lumen|Duodenum|C1"
      /// 
      ///   [binSolidDrugPaths] - paths to total amount of solid drug per particles bin
      ///   e.g. "Applications|Protocol|Formulation_Particles|Application_1|ParticleBin_1|C1"
      /// 
      ///   [binInsolubleDrugPaths] - paths to total amount of insoluble drug per particles bin
      ///   e.g. "Applications|Protocol|Formulation_Particles|Application_1|ParticleBin_1|InsolubleDrug|C1"
      /// 
      ///   [binSolidDrugPerSegmentPaths] - paths to amount of solid drug per particles bin per lumen segment
      ///   e.g. "Applications|Protocol|Formulation_Particles|Application_1|ParticleBin_1|C1|ParticlesDrugReleaseDuodenum|Solid drug"
      /// 
      ///   [binParticlesFractionPerSegmentPaths] -paths to number of particles fraction per particles bin per lumen segment
      ///   e.g. "Applications|Protocol|Formulation_Particles|Application_1|ParticleBin_1|C1|ParticlesDrugReleaseDuodenum|Number of particles fraction"
      /// </returns>
      /// 
      private (string[] lumenPaths, string[] binSolidDrugPaths, string[] binInsolubleDrugPaths,
               string[][] binSolidDrugPerSegmentPaths, string[][] binParticlesFractionPerSegmentPaths) 
         outputPaths(int numberOfBins)
      {
         var compoundName = _compound.Name;

         var lumenPaths = LumenPaths(compoundName);

         var binPaths = new string[numberOfBins];
         var binSolidDrugPaths = new string[numberOfBins];
         var binInsolubleDrugPaths = new string[numberOfBins];
         var binSolidDrugPerSegmentPaths = new string[numberOfBins][];
         var binParticlesFractionPerSegmentPaths = new string[numberOfBins][];

         for (int binIdx = 0; binIdx < numberOfBins; binIdx++)
         {
            binPaths[binIdx] = binPathFor(binIdx);
            binSolidDrugPaths[binIdx] = binSolidDrugPathFor(binPaths[binIdx], compoundName);
            binInsolubleDrugPaths[binIdx] = binInsolubleDrugPathFor(binPaths[binIdx], compoundName);
            binSolidDrugPerSegmentPaths[binIdx] = binSolidDrugPerSegmentPathsFor(binSolidDrugPaths[binIdx]);
            binParticlesFractionPerSegmentPaths[binIdx] = binParticlesFractionPerSegmentPathsFor(binSolidDrugPaths[binIdx]);
         }

         return (lumenPaths, binSolidDrugPaths, binInsolubleDrugPaths, 
                 binSolidDrugPerSegmentPaths, binParticlesFractionPerSegmentPaths);
      }

      protected string[] LumenPaths(string compoundName)
      {
         return (from segment in _lumenSegments select $"Organism|Lumen|{segment}|{compoundName}").ToArray();
      }

      private string applicationPath => $"Applications|{_protocol.Name}|{_formulation.Name}|Application_1";

      private string binPathFor(int binNumber) => $"{applicationPath}|ParticleBin_{binNumber+1}";

      private string binSolidDrugPathFor(string binPath, string drugName) => $"{binPath}|{drugName}";

      private string binInsolubleDrugPathFor(string binPath, string drugName) => $"{binPath}|InsolubleDrug|{drugName}";

      private string[] binSolidDrugPerSegmentPathsFor(string binSolidDrugPath) => binParameterPerSegmentPaths(binSolidDrugPath, "Solid drug");

      private string[] binParticlesFractionPerSegmentPathsFor(string binSolidDrugPath)=> binParameterPerSegmentPaths(binSolidDrugPath, "Number of particles fraction");

      private string[] binParameterPerSegmentPaths(string binSolidDrugPath, string parameter)
      {
         return (from segment in _lumenSegments
                 select $"{binSolidDrugPath}|ParticlesDrugRelease{segment}|{parameter}").ToArray();
      }

      protected IParameter MoleculeProperties(string parameterName)
      {
         return _simulation.Model.Root.Container(_compound.Name).Parameter(parameterName);
      }

      /// <summary>
      /// for debug purposes only
      /// </summary>
      protected void ExportSimulation(IndividualSimulation simulation, string file)
      {
         var exporter = IoC.Resolve<IMoBiExportTask>();
         exporter.SaveSimulationToFile(simulation, file);
      }

      /// <summary>
      /// Set solubility in stomach  = high
      /// set solubility in "segment" = 0
      /// set transit rate from "segment" to the next segment (e.g. from duodenum to upper jejunum) = 0
      /// set threshold for particles radius immediately dissolved = 0 (dissolution process never stops)
      /// </summary>
      protected void SetSolubilitySchema1WithStopIn(string segment)
      {
         Solubility(CoreConstants.Compartment.Stomach).Value = 1e-3; //[kg/l]
         Solubility(CoreConstants.Compartment.Duodenum).Value = 0;
         ParticleRadiusDissolved = 0;
         IntestinalTransitRateFor(CoreConstants.Compartment.Duodenum).Value = 0;
      }

      /// <summary>
      /// additionally to schema 1:
      ///  - set solubility in all segments after stomach and before "segment" = high
      ///  - set flow rate "stomach=>duodenum" = "slow"
      /// </summary>
      protected void SetSolubilitySchema2WithStopIn(string segment)
      {
         var stopSegmentIdx = Array.IndexOf(_lumenSegments, segment);
         for (var segmentIdx = 1; segmentIdx < stopSegmentIdx; segmentIdx++)
            Solubility(_lumenSegments[segmentIdx]).Value = 100;

         IntestinalTransitRateFor(CoreConstants.Compartment.Stomach).Value = 1e-3;
         SetSolubilitySchema1WithStopIn(segment);
      }

      //--------------------------------------------------------------------------------------------
      // Test routins called by observations in all derived classes
      //--------------------------------------------------------------------------------------------

      /// <summary>
      /// For every bin and for every simulated time step: 
      /// check that fractions of particles in all lumen segments sum up to 1
      /// </summary>
      protected void CheckSumOfParticlesNumberFractionsPerBin()
      {
         for (int binIdx = 0; binIdx < NumberOfBins; binIdx++)
         {
            for (int timeIdx = 0; timeIdx < NumberOfSimulatedtimePoints; timeIdx++)
            {
               var sum = LumenValuesFor(_binParticlesFractionPerSegment, binIdx, timeIdx).Sum();
               sum.ShouldBeEqualTo(1.0f, ComparisonTolerance, $"Bin={binIdx + 1}; time[{timeIdx}]={_times[timeIdx]}: sum of particle fractions was {sum}");
            }
         }
      }

      /// <summary>
      /// Make sure that no drug precipitation occurs during simulation
      /// </summary>
      protected void CheckNoPrecipitation()
      {
         for (int binIdx = 0; binIdx < NumberOfBins; binIdx++)
         {
            _binInsolubleDrug[binIdx].Min().ShouldBeEqualTo(0,ComparisonTolerance);
            _binInsolubleDrug[binIdx].Max().ShouldBeEqualTo(0, ComparisonTolerance);
         }
      }

      protected void CheckPrecipitation()
      {
         TotalPrecipitatedDrug.ShouldBeGreaterThan(0);
      }

      /// <summary>
      /// Precipitated drug at the end of simulation
      /// </summary>
      protected float TotalPrecipitatedDrug => totalDrug(_binInsolubleDrug);

      /// <summary>
      /// Solid drug at the end of simulation
      /// </summary>
      protected float TotalSolidDrug => totalDrug(_binSolidDrug);


      /// <summary>
      /// solid or precipitated drug at the end of simulation
      /// </summary>
      /// <param name="drugValues"></param>
      private float totalDrug(float[][] drugValues)
      {
         float totalDrug = 0;

         for (int binIdx = 0; binIdx < NumberOfBins; binIdx++)
            totalDrug += drugValues[binIdx][NumberOfSimulatedtimePoints - 1];

         return totalDrug;
      }

      /// <summary>
      /// Following mass balance checks are performed
      ///   1: The overall sum of solid, dissolved and insoluble(precipitated) drug must be always equal to the initial (applied) drug mass.
      ///   2: for every particles bin: sum of solid drug in all segments must sum up to the total solid drug in the bin
      ///   3: At t=0 only solid drug is available and the whole solid drug is in stomach
      /// </summary>
      protected void CheckMassBalance()
      {
         _appliedDrugMass.ShouldBeGreaterThan(0f); //just to be sure we have really applied smthg :)

         for (int timeIdx = 0; timeIdx < NumberOfSimulatedtimePoints; timeIdx++)
         {
            var drugMass = 0f;

            //due to disabled intestinal absorption and flow to the feces: the total drug mass is:
            // sum_lumen_segment{dissolved_drug}+sum_bins{solid drug}+sum_bins{insoluble drug}

            for (int binIdx = 0; binIdx < NumberOfBins; binIdx++)
            {
               var solidDrugInBin = _binSolidDrug[binIdx][timeIdx];
               drugMass += solidDrugInBin + _binInsolubleDrug[binIdx][timeIdx];

               var solidDrugInSegments = LumenValuesFor(_binSolidDrugPerSegment, binIdx, timeIdx).Sum();

               //make sure sum of solid drug in all segments sums up to the total solid drug in the bin
               solidDrugInSegments.ShouldBeEqualTo(solidDrugInBin,ComparisonTolerance, $"Solid drug amount in bin {binIdx+1} at time[{timeIdx}]={_times[timeIdx]}:");
            }

            for (int segmentIdx = 0; segmentIdx < NumberOfLumenSegments; segmentIdx++)
               drugMass += _dissolvedDrugLumen[segmentIdx][timeIdx];

            drugMass.ShouldBeEqualTo(_appliedDrugMass,ComparisonTolerance,$"Total drug amount at time[{timeIdx}]={_times[timeIdx]}:");
         }

         //check that sum_bins{solid drug at t=0}=sum_bins{solid drug in stomach at t=0)=applied drug mass 
         float solidDrug0 = 0, solidDrugStomach0 = 0;
         for (int binIdx = 0; binIdx < NumberOfBins; binIdx++)
         {
            solidDrug0 += _binSolidDrug[binIdx][0];
            solidDrugStomach0 += LumenValuesFor(_binSolidDrugPerSegment, binIdx,timeIdx: 0)[0];
         }

         solidDrug0.ShouldBeEqualTo(_appliedDrugMass, ComparisonTolerance, $"Solid drug amount at t=0 ({solidDrug0}) differs from applied drug mass");
         solidDrugStomach0.ShouldBeEqualTo(_appliedDrugMass, ComparisonTolerance, $"Solid drug amount at t=0 ({solidDrug0}) differs from applied drug mass");
      }

      protected void CheckSolidDrugIncreasing(string segment)
      {
         checkSolidDrugMonotonous(segment, checkDecreasing: false);
      }

      protected void CheckSolidDrugDecreasing(string segment)
      {
         checkSolidDrugMonotonous(segment, checkDecreasing: true);
      }

      protected void CheckInsolubleDrugIncreasing()
      {
         for (int binIdx = 0; binIdx < NumberOfBins; binIdx++)
         {
            checkValuesMonotonous(_binInsolubleDrug[binIdx], checkDecreasing: false);
         }
      }

      private void checkSolidDrugMonotonous(string segment, bool checkDecreasing)
      {
         var segmentIdx = Array.IndexOf(_lumenSegments,segment);

         for (int binIdx = 0; binIdx < NumberOfBins; binIdx++)
         {
            var solidDrugSegmentValues = _binSolidDrugPerSegment[binIdx][segmentIdx];
            checkValuesMonotonous(solidDrugSegmentValues, checkDecreasing);
         }
      }

      protected void CheckSolidDrugZeroForTimeGreaterThanZero()
      {
         for (int timeIdx = 1; timeIdx < NumberOfSimulatedtimePoints; timeIdx++)
         {
            for (int binIdx = 0; binIdx < NumberOfBins; binIdx++)
            {
               _binSolidDrug[binIdx][timeIdx].ShouldBeEqualTo(0);
            }
         }
      }

      private void checkValuesMonotonous(float[] values, bool checkDecreasing)
      {
         var comparisonTolerance = _simulation.Solver.AbsTol;

         for (var timeIdx = 1; timeIdx < NumberOfSimulatedtimePoints; timeIdx++)
         {
            var previousValue = values[timeIdx - 1];
            var currentValue = values[timeIdx];

            if (previousValue < comparisonTolerance || currentValue < comparisonTolerance)
               continue;

            var diff = currentValue - previousValue;
            if (checkDecreasing)
               diff *= -1;

            diff += (100 * _simulation.Solver.AbsTol).ToFloat(); //ignore "small" negative differences

            diff.ShouldBeGreaterThanOrEqualTo(0);
         }
      }
   }

   public class when_comparing_with_prototype_simulation_in_mobi : concern_for_SimulationWithParticlesFormulation
   {
      private float[][] _prototypeSimulationDissolvedDrugLumen; //dissolved drug[lumen segment][time] in the prototype simulation
      private float[] _fractionAbsorbed, _prototypeSimulationFractionAbsorbed;
      private float[] _peripheralVenousbloodPls, _prototypeSimulationPeripheralVenousbloodPls;

      private IndividualSimulation _prototypeSimulation;

      public override void GlobalContext()
      {
         base.GlobalContext();

         //load prototype simulation
         var importSimulationTask = IoC.Resolve<IImportSimulationTask>();
         var pkmlFilePrototypeSimulation = DomainHelperForSpecs.DataFilePathFor("PrototypeParticlesDissolution_722.pkml");
         _prototypeSimulation = importSimulationTask.ImportIndividualSimulation(pkmlFilePrototypeSimulation);

         //add output paths
         var compoundName = _compound.Name;
         var lumenPaths = LumenPaths(compoundName);
         addOutputs(_prototypeSimulation, lumenPaths);

         RunSimulation(_prototypeSimulation).Wait();

         //fill simulation output times and values required for all further tests
         fillSimulationOutputs(lumenPaths);
      }

      private void fillSimulationOutputs(string[] lumenPaths)
      {
         _prototypeSimulationDissolvedDrugLumen = new float[NumberOfLumenSegments][];
         for (var segmentIdx = 0; segmentIdx < NumberOfLumenSegments; segmentIdx++)
            _prototypeSimulationDissolvedDrugLumen[segmentIdx] = ValuesFor(_prototypeSimulation, lumenPaths[segmentIdx]);

         _fractionAbsorbed = ValuesFor(_simulation, fractionAbsorbedPath);
         _prototypeSimulationFractionAbsorbed = ValuesFor(_prototypeSimulation, fractionAbsorbedPath);

         _peripheralVenousbloodPls = ValuesFor(_simulation, peripheralVenPlsPath);
         _prototypeSimulationPeripheralVenousbloodPls = ValuesFor(_prototypeSimulation, peripheralVenPlsPath);
      }

      private void addOutputs(IndividualSimulation prototypeSimulation, string[] lumenPaths)
      {
         foreach (var path in lumenPaths)
         {
            AddOutputTo(prototypeSimulation, path);
         }

         var allPaths = new[] {fractionAbsorbedPath, peripheralVenPlsPath};
         foreach (var path in allPaths)
         {
            AddOutputTo(_simulation,path,QuantityType.Observer);
            AddOutputTo(_prototypeSimulation, path, QuantityType.Observer);
         }
      }

      private string fractionAbsorbedPath => $"Organism|Lumen|{_compound.Name}|Fraction of oral drug mass absorbed into mucosa";

      private string peripheralVenPlsPath => $"Organism|PeripheralVenousBlood|{_compound.Name}|Plasma (Peripheral Venous Blood)";

      protected override double ComparisonTolerance => 1e-1;

      protected override int NumberOfBins => 1;

      protected override bool DisableIntestinalAbsorptionAndLuminalFlow => false;

      protected override void SetupBuildingBlocks()
      {
         _compound.Name = "Aciclovir";
         _formulation.Name = "IR tablet";
         _protocol.Name = "5mg tablet";

         var cloneManager = IoC.Resolve<ICloneManager>();
         var compoundProcessRepository = IoC.Resolve<ICompoundProcessRepository>();

         var transportProcess = cloneManager.Clone(compoundProcessRepository.ProcessByName("TubularSecretion_FirstOrder"))
            .WithName("Renal Clearances-Literatur TS");
         transportProcess.Parameter("Volume (kidney)").Value = 0.4377;
         transportProcess.Parameter("Tubular secretion").Value = 0.412;
         _compound.AddProcess(transportProcess);

         transportProcess = cloneManager.Clone(compoundProcessRepository.ProcessByName("GlomerularFiltration"))
            .WithName("Glomerular Filtration-Literature GFR");
         transportProcess.Parameter("GFR fraction").Value = 1;
         _compound.AddProcess(transportProcess);

         _formulation.Parameter("Particle radius (mean)").Value = 0.0001;
      }

      protected override void SetupProcesses()
      {
         foreach (var process in _compound.AllProcesses<SystemicProcess>())
         {
            var interactionSelection = new InteractionSelection { CompoundName = _compound.Name, ProcessName = process.Name };
            _simulation.InteractionProperties.AddInteraction(interactionSelection);
         }
      }

      protected override void SetupSimulation()
      {
         PrecipitatedDrugSoluble = true;

         MoleculeProperties(CoreConstants.Parameters.LIPOPHILICITY).Value = -0.097;
         MoleculeProperties(CoreConstants.Parameters.FRACTION_UNBOUND_PLASMA_REFERENCE_VALUE).Value = 0.85;
         MoleculeProperties(CoreConstants.Parameters.MOLECULAR_WEIGHT).Value = 2.2521E-07;
         MoleculeProperties(CoreConstants.Parameters.PARAMETER_PKA1).Value = 9.20;
         MoleculeProperties(Constants.Parameters.COMPOUND_TYPE1).Value = CoreConstants.Compound.COMPOUND_TYPE_ACID;
         MoleculeProperties(CoreConstants.Parameters.PARAMETER_PKA2).Value = 2.20;
         MoleculeProperties(Constants.Parameters.COMPOUND_TYPE2).Value = CoreConstants.Compound.COMPOUND_TYPE_BASE;
         MoleculeProperties(CoreConstants.Parameters.SOLUBILITY_AT_REFERENCE_PH).Value = 1E-05;
         MoleculeProperties("Reference pH").Value = 7;

         Application.Container(CoreConstants.ContainerName.ProtocolSchemaItem)
            .Parameter(CoreConstantsForSpecs.Parameter.DRUG_MASS).Value = 22.2015;
      }

      private void compareSimulatedValues(float[] newValues, float[] prototypeValues, string location)
      {
         var threshold = 100*_simulation.Solver.AbsTol;

         for (var timeIdx = 0; timeIdx < NumberOfSimulatedtimePoints; timeIdx++)
         {
            var newValue = newValues[timeIdx];
            var prototypeValue = prototypeValues[timeIdx];

            if (newValue < threshold || prototypeValue < threshold)
               continue;

            newValue.ShouldBeEqualTo(prototypeValue, ComparisonTolerance,$"At Times[{timeIdx}]={_times[timeIdx]}: {location}");
         }
      }

      [Observation]
      public void values_in_lumen_segments_should_be_equal()
      {
         for (var segmentIdx = 0; segmentIdx < NumberOfLumenSegments; segmentIdx++)
         {
            compareSimulatedValues(_dissolvedDrugLumen[segmentIdx], _prototypeSimulationDissolvedDrugLumen[segmentIdx], $"dissolved drug in {_lumenSegments[segmentIdx]}");
         }
      }

      [Observation]
      public void fraction_absorbed_values_should_be_equal()
      {
         compareSimulatedValues(_fractionAbsorbed, _prototypeSimulationFractionAbsorbed, "fraction absorbed into mucosa");
      }

      [Observation]
      public void peripheral_venous_blood_plasma_values_should_be_equal()
      {
         compareSimulatedValues(_peripheralVenousbloodPls, _prototypeSimulationPeripheralVenousbloodPls, "peripheral venous blood (plasma)");
      }
   }

   public class when_running_particles_simulation_with_two_bins_without_precipitation : concern_for_SimulationWithParticlesFormulation
   {
      protected override int NumberOfBins => 2;

      protected override void SetupSimulation()
      {
         //disable precipitation
         PrecipitatedDrugSoluble = true;
      }

      [Observation]
      public void sum_of_particles_number_fractions_must_be_one_for_every_bin()
      {
         CheckSumOfParticlesNumberFractionsPerBin();
      }

      [Observation]
      public void precipitation_should_not_occur()
      {
         CheckNoPrecipitation();
      }

      [Observation]
      public void mass_balance_of_drug_should_be_correct()
      {
         CheckMassBalance();
      }
   }

   public class when_running_particles_simulation_with_two_bins_with_precipitation : concern_for_SimulationWithParticlesFormulation
   {
      protected override int NumberOfBins => 2;

      protected override void SetupSimulation()
      {
         //enable precipitation
         PrecipitatedDrugSoluble = false;

         //set low solubility in duodenum to assure precipitation
         Solubility(CoreConstants.Compartment.Duodenum).Value = 1e-12;
      }

      [Observation]
      public void sum_of_particles_number_fractions_must_be_one_for_every_bin()
      {
         CheckSumOfParticlesNumberFractionsPerBin();
      }

      [Observation]
      public void precipitation_should_occur()
      {
         CheckPrecipitation();
      }

      [Observation]
      public void mass_balance_of_drug_should_be_correct()
      {
         CheckMassBalance();
      }


      [Observation]
      public void amount_of_precipitated_drug_must_be_increasing()
      {
         CheckInsolubleDrugIncreasing();
      }
   }

   public class when_running_particles_simulation_with_one_bin_without_precipitation : concern_for_SimulationWithParticlesFormulation
   {
      protected override int NumberOfBins => 1;

      protected override void SetupSimulation()
      {
         //disable precipitation
         PrecipitatedDrugSoluble = true;
      }

      [Observation]
      public void sum_of_particles_number_fractions_must_be_one_for_every_bin()
      {
         CheckSumOfParticlesNumberFractionsPerBin();
      }

      [Observation]
      public void precipitation_should_not_occur()
      {
         CheckNoPrecipitation();
      }

      [Observation]
      public void mass_balance_of_drug_should_be_correct()
      {
         CheckMassBalance();
      }
   }

   public class when_running_particles_simulation_with_one_bin_with_precipitation : concern_for_SimulationWithParticlesFormulation
   {
      protected override int NumberOfBins => 1;

      protected override void SetupSimulation()
      {
         //enable precipitation
         PrecipitatedDrugSoluble = false;

         //set low solubility in duodenum to assure precipitation
         Solubility(CoreConstants.Compartment.Duodenum).Value = 1e-12;
      }

      [Observation]
      public void sum_of_particles_number_fractions_must_be_one_for_every_bin()
      {
         CheckSumOfParticlesNumberFractionsPerBin();
      }

      [Observation]
      public void precipitation_should_occur()
      {
         CheckPrecipitation();
      }

      [Observation]
      public void mass_balance_of_drug_should_be_correct()
      {
         CheckMassBalance();
      }

      [Observation]
      public void amount_of_precipitated_drug_must_be_increasing()
      {
         CheckInsolubleDrugIncreasing();
      }
   }

   public class when_running_particles_simulation_with_two_bins_without_precipitation_solubility_schema_1 : concern_for_SimulationWithParticlesFormulation
   {
      protected override int NumberOfBins => 2;

      protected override void SetupSimulation()
      {
         //disable precipitation
         PrecipitatedDrugSoluble = true;

         //following this schema, most of drug will be dissolved in the stomach
         //in duodenum the drug will be accumulated and nearly all drug will turn into solid form again
         SetSolubilitySchema1WithStopIn(CoreConstants.Compartment.Duodenum);
      }

      [Observation]
      public void solid_drug_in_stomach_should_be_monotonically_decreasing()
      {
         CheckSolidDrugDecreasing(CoreConstants.Compartment.Stomach);
      }

      [Observation]
      public void solid_drug_in_duodenum_should_be_monotonically_increasing()
      {
         //due to solubility=0 and flow to the next segment=0 solid drug must increase over time
         CheckSolidDrugIncreasing(CoreConstants.Compartment.Duodenum);
      }

      [Observation]
      public void sum_of_particles_number_fractions_must_be_one_for_every_bin()
      {
         CheckSumOfParticlesNumberFractionsPerBin();
      }

      [Observation]
      public void precipitation_should_not_occur()
      {
         CheckNoPrecipitation();
      }

      [Observation]
      public void mass_balance_of_drug_should_be_correct()
      {
         CheckMassBalance();
      }
   }

   public class when_running_particles_simulation_with_two_bins_with_precipitation_solubility_schema_1 : concern_for_SimulationWithParticlesFormulation
   {
      protected override int NumberOfBins => 2;

      protected override void SetupSimulation()
      {
         //disable precipitation
         PrecipitatedDrugSoluble = false;

         //following this schema, nearly all drug will be dissolved in the stomach
         //in duodenum the drug will be accumulated and nearly all drug will turn into insoluble (precipitated) form
         SetSolubilitySchema1WithStopIn(CoreConstants.Compartment.Duodenum);
      }

      [Observation]
      public void solid_drug_in_stomach_should_be_monotonically_decreasing()
      {
         CheckSolidDrugDecreasing(CoreConstants.Compartment.Stomach);
      }

      [Observation]
      public void amount_of_precipitated_drug_must_be_increasing()
      {
         CheckInsolubleDrugIncreasing();
      }

      [Observation]
      public void solid_drug_in_duodenum_should_be_monotonically_increasing()
      {
         CheckSolidDrugIncreasing(CoreConstants.Compartment.Duodenum);
      }

      [Observation]
      public void sum_of_particles_number_fractions_must_be_one_for_every_bin()
      {
         CheckSumOfParticlesNumberFractionsPerBin();
      }

      [Observation]
      public void precipitation_should_occur()
      {
         CheckPrecipitation();
      }

      [Observation]
      public void mass_balance_of_drug_should_be_correct()
      {
         CheckMassBalance();
      }
   }

   public class when_running_particles_simulation_with_two_bins_without_precipitation_solubility_schema_2 : concern_for_SimulationWithParticlesFormulation
   {
      protected override int NumberOfBins => 2;

      protected override void SetupSimulation()
      {
         //disable precipitation
         PrecipitatedDrugSoluble = true;

         _simulation.SimulationSettings.OutputSchema.Intervals.Last().EndTime.Value = 10 * 24 * 60; //10 days

         //following this schema, most of the drug will be dissolved in the stomach
         //in rectum the drug will be accumulated and most of the drug will turn into insoluble (precipitated) form
         SetSolubilitySchema2WithStopIn(CoreConstants.Compartment.Rectum);
      }

      [Observation]
      public void solid_drug_in_stomach_should_be_monotonically_decreasing()
      {
         CheckSolidDrugDecreasing(CoreConstants.Compartment.Stomach);
      }

      [Observation]
      public void solid_drug_in_rectum_should_be_monotonically_increasing()
      {
         //due to solubility=0 and flow to the next segment=0 solid drug must increase over time
         CheckSolidDrugIncreasing(CoreConstants.Compartment.Rectum);
      }

      [Observation]
      public void solid_drug_in_duodenum_should_be_monotonically_increasing()
      {
         CheckSolidDrugIncreasing(CoreConstants.Compartment.Duodenum);
      }

      [Observation]
      public void sum_of_particles_number_fractions_must_be_one_for_every_bin()
      {
         CheckSumOfParticlesNumberFractionsPerBin();
      }

      [Observation]
      public void precipitation_should_not_occur()
      {
         CheckNoPrecipitation();
      }

      [Observation]
      public void mass_balance_of_drug_should_be_correct()
      {
         CheckMassBalance();
      }

      [Observation]
      public void nearly_all_drug_must_be_solid_after_long_simulation()
      {
         TotalSolidDrug.ShouldBeGreaterThan(0.99f * _appliedDrugMass);
      }
   }

   public class when_running_particles_simulation_with_two_bins_with_precipitation_solubility_schema_2 : concern_for_SimulationWithParticlesFormulation
   {
      protected override int NumberOfBins => 2;

      protected override void SetupSimulation()
      {
         //disable precipitation
         PrecipitatedDrugSoluble = false;

         _simulation.SimulationSettings.OutputSchema.Intervals.Last().EndTime.Value = 10 * 24 * 60; //10 days

         //following this schema, most of the drug will be dissolved in the stomach
         //in rectum the drug will be accumulated and most of the drug will turn into insoluble (precipitated) form
         SetSolubilitySchema2WithStopIn(CoreConstants.Compartment.Rectum);
      }

      [Observation]
      public void solid_drug_in_stomach_should_be_monotonically_decreasing()
      {
         CheckSolidDrugDecreasing(CoreConstants.Compartment.Stomach);
      }

      [Observation]
      public void amount_of_precipitated_drug_must_be_increasing()
      {
         CheckInsolubleDrugIncreasing();
      }

      [Observation]
      public void sum_of_particles_number_fractions_must_be_one_for_every_bin()
      {
         CheckSumOfParticlesNumberFractionsPerBin();
      }

      [Observation]
      public void precipitation_should_occur()
      {
         CheckPrecipitation();
      }

      [Observation]
      public void mass_balance_of_drug_should_be_correct()
      {
         CheckMassBalance();
      }

      [Observation]
      public void nearly_all_drug_must_be_precipitated_after_long_simulation()
      {
         TotalPrecipitatedDrug.ShouldBeGreaterThan(0.99f*_appliedDrugMass);
      }
   }

   public class when_running_particles_simulation_with_two_bins_without_precipitation_and_high_particle_radius_dissolved : concern_for_SimulationWithParticlesFormulation
   {
      protected override int NumberOfBins => 2;

      protected override void SetupSimulation()
      {
         //disable precipitation
         PrecipitatedDrugSoluble = true;

         //set particle radius dissolved > particle radius in all bins
         ParticleRadiusDissolved = Math.Max(StartParticleRadius(0).Value, StartParticleRadius(1).Value) + 1;
      }

      [Observation]
      public void sum_of_particles_number_fractions_must_be_one_for_every_bin()
      {
         CheckSumOfParticlesNumberFractionsPerBin();
      }

      [Observation]
      public void precipitation_should_not_occur()
      {
         CheckNoPrecipitation();
      }

      [Observation]
      public void mass_balance_of_drug_should_be_correct()
      {
         CheckMassBalance();
      }

      [Observation]
      public void amount_of_solid_drug_should_be_zero_for_time_greater_than_zero()
      {
         CheckSolidDrugZeroForTimeGreaterThanZero();
      }
   }

   public class when_running_particles_simulation_with_two_bins_with_precipitation_and_high_particle_radius_dissolved : concern_for_SimulationWithParticlesFormulation
   {
      protected override int NumberOfBins => 2;

      protected override void SetupSimulation()
      {
         //disable precipitation
         PrecipitatedDrugSoluble = false;

         //set particle radius dissolved > particle radius in all bins
         ParticleRadiusDissolved = Math.Max(StartParticleRadius(0).Value, StartParticleRadius(1).Value) + 1;
      }

      [Observation]
      public void sum_of_particles_number_fractions_must_be_one_for_every_bin()
      {
         CheckSumOfParticlesNumberFractionsPerBin();
      }

      [Observation]
      public void precipitation_should_not_occur()
      {
         //due to the fact that the whole drug is immediately considered as dissolved, no precipitation should occur
         CheckNoPrecipitation();
      }

      [Observation]
      public void mass_balance_of_drug_should_be_correct()
      {
         CheckMassBalance();
      }

      [Observation]
      public void amount_of_solid_drug_should_be_zero_for_time_greater_than_zero()
      {
         CheckSolidDrugZeroForTimeGreaterThanZero();
      }
   }
}	