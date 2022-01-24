.SET PARAM @newSpecies = 'MySpecies'
.SET PARAM @srcSpecies = 'Beagle'
.SET PARAM @defaultValueOrigin = 'Copied from Beagle'

INSERT INTO tab_species (SPECIES, DISPLAY_NAME, DESCRIPTION, SEQUENCE, USER_DEFINED, ICON_NAME) SELECT @newSpecies,@newSpecies,@newSpecies, Max(SEQUENCE)+1,0,@newSpecies FROM tab_species;

INSERT INTO tab_species_calculation_methods (SPECIES, CALCULATION_METHOD) SELECT @newSpecies AS SPECIES, CALCULATION_METHOD FROM tab_species_calculation_methods WHERE SPECIES=@srcSpecies;

INSERT INTO tab_populations (POPULATION, RACE_INDEX, SPECIES, DISPLAY_NAME, DESCRIPTION, IS_AGE_DEPENDENT, IS_HEIGHT_DEPENDENT, ICON_NAME) SELECT @newSpecies,Max(RACE_INDEX)+1,@newSpecies,@newSpecies,@newSpecies,0,0,@newSpecies FROM tab_populations;

INSERT INTO tab_population_genders (POPULATION, SEQUENCE, GENDER)VALUES (@newSpecies,1,'UNKNOWN');

INSERT INTO tab_population_containers (POPULATION, CONTAINER_ID, CONTAINER_TYPE, CONTAINER_NAME) SELECT @newSpecies AS POPULATION, CONTAINER_ID, CONTAINER_TYPE, CONTAINER_NAME FROM tab_population_containers WHERE POPULATION=@srcSpecies;

INSERT INTO tab_species_parameter_value_versions (species, parameter_value_version) SELECT @newSpecies AS species, parameter_value_version FROM tab_species_parameter_value_versions WHERE species=@srcSpecies;

INSERT INTO tab_model_species (model, species, is_default) SELECT model, @newSpecies AS species, is_default FROM tab_model_species WHERE species=@srcSpecies;

INSERT OR IGNORE INTO tab_references(reference) VALUES(@defaultValueOrigin);

INSERT INTO tab_value_origins (id, description, source, method) SELECT Max(id)+1 AS id, @defaultValueOrigin, 'Other', 'Assumption' FROM tab_value_origins;

INSERT INTO tab_container_parameter_values (parameter_value_version, species, container_id, container_type, container_name, parameter_name, default_value, min_value, min_isallowed, max_value, max_isallowed, value_origin) SELECT parameter_value_version, @newSpecies AS species, container_id, container_type, container_name, parameter_name, default_value, min_value, min_isallowed, max_value, max_isallowed, id FROM tab_container_parameter_values, tab_value_origins WHERE species=@srcSpecies AND id=(SELECT MAX(id) FROM tab_value_origins);

INSERT INTO tab_active_transports (gene, species, container_type, container_name, compartment_type, compartment_name, membrane, sequence, transport_type) SELECT gene, @newSpecies AS species, container_type, container_name, compartment_type, compartment_name, membrane, sequence, transport_type FROM tab_active_transports WHERE species=@srcSpecies;

INSERT INTO tab_ontogenies (molecule, species, group_name, postmenstrual_age, ontogeny_factor, deviation) SELECT molecule, @newSpecies AS species, group_name, postmenstrual_age, ontogeny_factor, deviation FROM tab_ontogenies WHERE species=@srcSpecies;