-- Fix for SQLite stack overflow on macOS ARM64
-- https://github.com/Open-Systems-Pharmacology/PK-Sim/issues/XXXX
--
-- Problem: ContainerParameters_Species VIEW causes stack overflow when queried
-- through .NET System.Data.SQLite on macOS ARM64 due to limited P/Invoke stack
-- and SQLite's deep recursion in query planner for 3-way JOIN + DISTINCT.
--
-- Solution: Materialize VIEW as TABLE to pre-compute results at build time.

DROP VIEW IF EXISTS [ContainerParameters_Species];

CREATE TABLE [ContainerParameters_Species] (
  [container_id] INTEGER NOT NULL,
  [container_type] TEXT NOT NULL,
  [container_name] TEXT NOT NULL,
  [parameter_name] TEXT NOT NULL,
  [species] TEXT NOT NULL
);

INSERT INTO [ContainerParameters_Species]
SELECT DISTINCT 
                [cpv].[container_id], 
                [cpv].[container_type], 
                [cpv].[container_name], 
                [cpv].[parameter_name], 
                [species]
FROM   [tab_container_parameter_values] AS [cpv],
       [tab_container_parameters] AS [cp]
WHERE  [cpv].[container_id] = [cp].[container_id]
         AND [cpv].[parameter_name] = [cp].[parameter_name]
         AND [building_block_type] = "INDIVIDUAL"
UNION
SELECT DISTINCT 
                [cpr].[container_id], 
                [cpr].[container_type], 
                [cpr].[container_name], 
                [cpr].[parameter_name], 
                [species]
FROM   [tab_container_parameter_rates] AS [cpr],
       [tab_species_calculation_methods] AS [scm],
       [tab_container_parameters] AS [cp]
WHERE  [cpr].[calculation_method] = [scm].[calculation_method]
         AND [cpr].[container_id] = [cp].[container_id]
         AND [cpr].[parameter_name] = [cp].[parameter_name]
         AND [building_block_type] = "INDIVIDUAL";

CREATE INDEX [idx_container_params_species] 
  ON [ContainerParameters_Species]([species], [container_id], [parameter_name]);

-- Fix VIEW_INDIVIDUAL_PARAMETER_SAME_FORMULA_OR_VALUE_FOR_ALL_SPECIES (also has problematic 3-way JOINs with UNION)
DROP VIEW IF EXISTS [VIEW_INDIVIDUAL_PARAMETER_SAME_FORMULA_OR_VALUE_FOR_ALL_SPECIES];

CREATE TABLE [VIEW_INDIVIDUAL_PARAMETER_SAME_FORMULA_OR_VALUE_FOR_ALL_SPECIES] (
  [ContainerId] INTEGER NOT NULL,
  [ContainerType] TEXT NOT NULL,
  [ContainerName] TEXT NOT NULL,
  [ParameterName] TEXT NOT NULL,
  [IsSameFormula] INTEGER NOT NULL
);

INSERT INTO [VIEW_INDIVIDUAL_PARAMETER_SAME_FORMULA_OR_VALUE_FOR_ALL_SPECIES]
SELECT DISTINCT 
                [tab_container_parameter_rates].[container_id] AS [ContainerId], 
                [tab_container_parameter_rates].[container_type] AS [ContainerType], 
                [tab_container_parameter_rates].[container_name] AS [ContainerName], 
                [tab_container_parameter_rates].[parameter_name] AS [ParameterName], 
                1 AS [IsSameFormula]
FROM   [tab_container_parameter_rates],
       [tab_species_calculation_methods],
       [tab_container_parameters]
WHERE  [tab_container_parameter_rates].[calculation_method] = [tab_species_calculation_methods].[calculation_method]
         AND [tab_container_parameters].[container_id] = [tab_container_parameter_rates].[container_id]
         AND [tab_container_parameters].[container_type] = [tab_container_parameter_rates].[container_type]
         AND [tab_container_parameters].[container_name] = [tab_container_parameter_rates].[container_name]
         AND [tab_container_parameters].[parameter_name] = [tab_container_parameter_rates].[parameter_name]
         AND [tab_container_parameters].[building_block_type] = "INDIVIDUAL"
GROUP  BY
          [tab_container_parameter_rates].[container_id], 
          [tab_container_parameter_rates].[container_type], 
          [tab_container_parameter_rates].[container_name], 
          [tab_container_parameter_rates].[parameter_name], 
          [tab_container_parameter_rates].[calculation_method], 
          [tab_container_parameter_rates].[formula_rate]
HAVING COUNT ([tab_species_calculation_methods].[species]) = (SELECT COUNT ([species]) FROM [tab_species])
UNION
SELECT DISTINCT 
                [tab_container_parameter_values].[container_id] AS [ContainerId], 
                [tab_container_parameter_values].[container_type] AS [ContainerType], 
                [tab_container_parameter_values].[container_name] AS [ContainerName], 
                [tab_container_parameter_values].[parameter_name] AS [ParameterName], 
                0 AS [IsSameFormula]
FROM   [tab_container_parameters],
       [tab_container_parameter_values],
       [tab_species_parameter_value_versions]
WHERE  [tab_container_parameters].[container_id] = [tab_container_parameter_values].[container_id]
         AND [tab_container_parameters].[container_type] = [tab_container_parameter_values].[container_type]
         AND [tab_container_parameters].[container_name] = [tab_container_parameter_values].[container_name]
         AND [tab_container_parameters].[parameter_name] = [tab_container_parameter_values].[parameter_name]
         AND [tab_species_parameter_value_versions].[species] = [tab_container_parameter_values].[species]
         AND [tab_species_parameter_value_versions].[parameter_value_version] = [tab_container_parameter_values].[parameter_value_version]
         AND [tab_container_parameters].[building_block_type] = "INDIVIDUAL"
GROUP  BY
          [tab_container_parameter_values].[container_id], 
          [tab_container_parameter_values].[container_type], 
          [tab_container_parameter_values].[container_name], 
          [tab_container_parameter_values].[parameter_name], 
          [tab_container_parameter_values].[default_value]
HAVING COUNT ([tab_container_parameter_values].[species]) = (SELECT COUNT ([species]) FROM [tab_species]);

CREATE INDEX [idx_same_formula_species] 
  ON [VIEW_INDIVIDUAL_PARAMETER_SAME_FORMULA_OR_VALUE_FOR_ALL_SPECIES]([IsSameFormula], [ContainerId], [ParameterName]);

