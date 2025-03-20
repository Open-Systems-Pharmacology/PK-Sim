using System.Collections.Generic;
using System.Xml.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Core.Serialization;
using OSPSuite.Core.Serialization.Xml;
using OSPSuite.Utility.Collections;
using PKSim.Core;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using IContainer = OSPSuite.Utility.Container.IContainer;

namespace PKSim.Infrastructure.ORM.Repositories
{
   public class DimensionRepository : StartableRepository<IDimension>, IDimensionRepository
   {
      private readonly IPKSimDimensionFactory _dimensionFactory;
      private readonly IPKSimConfiguration _pkSimConfiguration;
      private readonly IContainer _container;
      private readonly IUnitSystemXmlSerializerRepository _unitSystemXmlSerializerRepository;

      public DimensionRepository(
         IPKSimDimensionFactory dimensionFactory,
         IUnitSystemXmlSerializerRepository unitSystemXmlSerializerRepository,
         IPKSimConfiguration pkSimConfiguration,
         IContainer container)
      {
         _dimensionFactory = dimensionFactory;
         _dimensionFactory.DimensionRepository = this;
         _unitSystemXmlSerializerRepository = unitSystemXmlSerializerRepository;
         _pkSimConfiguration = pkSimConfiguration;
         _container = container;
      }

      public IDimension OptimalDimensionFor(IDimension dimension) => _dimensionFactory.OptimalDimension(dimension);

      public IDimension DimensionForUnit(string unit) => _dimensionFactory.DimensionForUnit(unit) ?? NoDimension;

      public IDimension DosePerBodyWeight => DimensionByName(CoreConstants.Dimension.DosePerBodyWeight);

      public IDimension InputDose => DimensionByName(CoreConstants.Dimension.InputDose);

      public IDimension Time => DimensionByName(Constants.Dimension.TIME);

      public IDimension MassConcentration => DimensionByName(CoreConstants.Dimension.MASS_CONCENTRATION);

      public IDimension MolarConcentration => DimensionByName(Constants.Dimension.MOLAR_CONCENTRATION);

      public IDimension NoDimension => _dimensionFactory.NoDimension;

      public IDimension Fraction => DimensionByName(CoreConstants.Dimension.Fraction);

      public IDimension Amount => DimensionByName(Constants.Dimension.MOLAR_AMOUNT);

      public IDimension Mass => DimensionByName(Constants.Dimension.MASS_AMOUNT);

      public IDimension AucMolar => DimensionByName(CoreConstants.Dimension.AucMolar);

      public IDimension Auc => DimensionByName(CoreConstants.Dimension.Auc);

      public IDimension AgeInYears => DimensionByName(CoreConstants.Dimension.Age);

      public IDimension AgeInWeeks => DimensionByName(CoreConstants.Dimension.AgeInWeeks);

      public IDimension Length => DimensionByName(CoreConstants.Dimension.Length);

      public IDimension BMI => DimensionByName(CoreConstants.Dimension.BMI);

      public IDimension Volume => DimensionByName(Constants.Dimension.VOLUME);

      public IDimension AmountPerTime => DimensionByName(Constants.Dimension.AMOUNT_PER_TIME);

      public override IEnumerable<IDimension> All()
      {
         Start();
         return _dimensionFactory.Dimensions;
      }

      public IDimension DimensionByName(string dimensionName)
      {
         Start();
         if (string.IsNullOrEmpty(dimensionName))
            return NoDimension;

         if (!_dimensionFactory.Has(dimensionName))
            return NoDimension;

         return _dimensionFactory.Dimension(dimensionName);
      }

      public IDimensionFactory DimensionFactory
      {
         get
         {
            Start();
            return _dimensionFactory;
         }
      }

      public IDimension MergedDimensionFor(IWithDimension objectWithDimension)
      {
         return _dimensionFactory.MergedDimensionFor(objectWithDimension);
      }

      public IDimension MergedDimensionFor(object objectThatMightHaveDimension)
      {
         return objectThatMightHaveDimension is IWithDimension hasDimension ? MergedDimensionFor(hasDimension) : Constants.Dimension.NO_DIMENSION;
      }

      protected override void DoStart()
      {
         loadDimensionsFromFile();

         addInputDoseDimension();

         _dimensionFactory.AddDimension(Constants.Dimension.NO_DIMENSION);
      }

      private void addInputDoseDimension()
      {
         const double CONVERSION_FACTOR_MG_TO_KG = 1e-6;
         const double CONVERSION_FACTOR_MG_PER_KG_TO_KG_PER_KG = 1e-6;
         const double CONVERSION_FACTOR_MG_PER_M2_TO_KG_PER_M2 = 1e-8;

         var inputDoseDimension = _dimensionFactory.AddDimension(new BaseDimensionRepresentation(), CoreConstants.Dimension.InputDose, CoreConstants.Units.KgPerKg);
         inputDoseDimension.BaseUnit.Visible = false;

         addInputDoseUnit(inputDoseDimension, CoreConstants.Units.mg, CONVERSION_FACTOR_MG_TO_KG);
         inputDoseDimension.DefaultUnit = addInputDoseUnit(inputDoseDimension, CoreConstants.Units.MgPerKg, CONVERSION_FACTOR_MG_PER_KG_TO_KG_PER_KG);
         addInputDoseUnit(inputDoseDimension, CoreConstants.Units.MgPerM2, CONVERSION_FACTOR_MG_PER_M2_TO_KG_PER_M2);
      }

      private void loadDimensionsFromFile()
      {
         var serializer = _unitSystemXmlSerializerRepository.SerializerFor(_dimensionFactory);
         var xel = XElementSerializer.PermissiveLoad(_pkSimConfiguration.DimensionFilePath);
         serializer.Deserialize(_dimensionFactory, xel, SerializationTransaction.Create(_container));
      }

      private Unit addInputDoseUnit(IDimension inputDose, string unit, double factor)
      {
         return inputDose.AddUnit(unit, factor, 0);
      }

      protected override void PerformPostStartProcessing()
      {
         //Create merging dimension info
         var molarToMassConcentrationMerging = new SimpleDimensionMergingInformation(MolarConcentration, MassConcentration);
         _dimensionFactory.AddMergingInformation(molarToMassConcentrationMerging);

         var massToMolarConcentrationMerging = new SimpleDimensionMergingInformation(MassConcentration, MolarConcentration);
         _dimensionFactory.AddMergingInformation(massToMolarConcentrationMerging);

         var aucMolarToAucMassConcentrationMerging = new SimpleDimensionMergingInformation(AucMolar, Auc);
         _dimensionFactory.AddMergingInformation(aucMolarToAucMassConcentrationMerging);

         var amountToMassMerging = new SimpleDimensionMergingInformation(Amount, Mass);
         _dimensionFactory.AddMergingInformation(amountToMassMerging);

         var massToAmountMerging = new SimpleDimensionMergingInformation(Mass, Amount);
         _dimensionFactory.AddMergingInformation(massToAmountMerging);
      }
   }
}