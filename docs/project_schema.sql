
    PRAGMA foreign_keys = OFF

    drop table if exists BUILDING_BLOCKS

    drop table if exists COMPOUNDS

    drop table if exists EVENTS

    drop table if exists EVENT_PROTOCOLS

    drop table if exists EXPRESSION_PROFILES

    drop table if exists FORMULATIONS

    drop table if exists IMPORT_POPULATIONS

    drop table if exists INDIVIDUALS

    drop table if exists OBSERVER_SETS

    drop table if exists PROTOCOLS

    drop table if exists RANDOM_POPULATIONS

    drop table if exists SIMULATIONS

    drop table if exists USED_OBSERVED_DATA

    drop table if exists COMMANDS

    drop table if exists COMMAND_PROPERTIES

    drop table if exists DATA_REPOSITORIES

    drop table if exists HISTORY_ITEMS

    drop table if exists INDIVIDUAL_RESULTS

    drop table if exists INDIVIDUAL_RESULTS_QUANTITY_VALUES

    drop table if exists CONTENTS

    drop table if exists OBSERVED_DATA

    drop table if exists PARAMETER_IDENTIFICATIONS

    drop table if exists PROJECTS

    drop table if exists QUANTITY_VALUES

    drop table if exists SENSITIVITY_ANALYSES

    drop table if exists SIMULATION_ANALYSES

    drop table if exists SIMULATION_CHARTS

    drop table if exists SIMULATION_COMPARISONS

    drop table if exists INDIVIDUAL_SIMULATION_COMPARISONS

    drop table if exists POPULATION_SIMULATION_COMPARISONS

    drop table if exists SIMULATION_RESULTS

    drop table if exists USED_BUILDING_BLOCKS

    drop table if exists WORKSPACE_LAYOUT

    PRAGMA foreign_keys = ON

    create table BUILDING_BLOCKS (
        Id TEXT not null,
       Name TEXT not null,
       Description TEXT,
       Icon TEXT,
       Version INTEGER,
       StructureVersion INTEGER default 0 ,
       ContentId INTEGER,
       PropertiesId INTEGER,
       ProjectId INTEGER,
       primary key (Id),
       constraint fk_BuildingBlock_Content foreign key (ContentId) references CONTENTS,
       constraint fk_BuildingBlock_Properties foreign key (PropertiesId) references CONTENTS,
       constraint fk_Project_BuildingBlocks foreign key (ProjectId) references PROJECTS
    )

    create table COMPOUNDS (
        CompoundId TEXT not null,
       primary key (CompoundId),
       constraint FK_7F395611 foreign key (CompoundId) references BUILDING_BLOCKS
    )

    create table EVENTS (
        EventId TEXT not null,
       primary key (EventId),
       constraint FK_D59FE9E4 foreign key (EventId) references BUILDING_BLOCKS
    )

    create table EVENT_PROTOCOLS (
        EventProtocolId TEXT not null,
       primary key (EventProtocolId),
       constraint FK_51AC794 foreign key (EventProtocolId) references BUILDING_BLOCKS
    )

    create table EXPRESSION_PROFILES (
        ExpressionProfileId TEXT not null,
       primary key (ExpressionProfileId),
       constraint FK_5DEDDCC6 foreign key (ExpressionProfileId) references BUILDING_BLOCKS
    )

    create table FORMULATIONS (
        FormulationId TEXT not null,
       FormulationType TEXT,
       primary key (FormulationId),
       constraint FK_E0FB491E foreign key (FormulationId) references BUILDING_BLOCKS
    )

    create table IMPORT_POPULATIONS (
        PopulationId TEXT not null,
       ExpressionProfileIds TEXT,
       primary key (PopulationId),
       constraint FK_1969CA6C foreign key (PopulationId) references BUILDING_BLOCKS
    )

    create table INDIVIDUALS (
        IndividualId TEXT not null,
       ExpressionProfileIds TEXT,
       primary key (IndividualId),
       constraint FK_44262FBC foreign key (IndividualId) references BUILDING_BLOCKS
    )

    create table OBSERVER_SETS (
        ObserverSetId TEXT not null,
       primary key (ObserverSetId),
       constraint FK_9E1E1AB5 foreign key (ObserverSetId) references BUILDING_BLOCKS
    )

    create table PROTOCOLS (
        ProtocolId TEXT not null,
       ProtocolMode TEXT,
       primary key (ProtocolId),
       constraint FK_D6C82BAC foreign key (ProtocolId) references BUILDING_BLOCKS
    )

    create table RANDOM_POPULATIONS (
        PopulationId TEXT not null,
       ExpressionProfileIds TEXT,
       primary key (PopulationId),
       constraint FK_1CAC4FE8 foreign key (PopulationId) references BUILDING_BLOCKS
    )

    create table SIMULATIONS (
        SimulationId TEXT not null,
       SimulationMode TEXT,
       DataRepositoryId TEXT,
       SimulationResultsId INTEGER,
       SimulationAnalysesId INTEGER,
       primary key (SimulationId),
       constraint FK_E91B0238 foreign key (SimulationId) references BUILDING_BLOCKS,
       constraint fk_Simulation_SimulationResults foreign key (SimulationResultsId) references SIMULATION_RESULTS,
       constraint fk_Simulation_SimulationAnalyses foreign key (SimulationAnalysesId) references SIMULATION_ANALYSES
    )

    create table USED_OBSERVED_DATA (
        SimulationId TEXT not null,
       ObservedDataId TEXT,
       constraint fk_Simulation_ObservedData foreign key (SimulationId) references SIMULATIONS
    )

    create table COMMANDS (
        Id TEXT not null,
       CommandId TEXT not null,
       Discriminator TEXT not null,
       CommandInverseId TEXT,
       CommandType TEXT,
       ObjectType TEXT,
       Description TEXT,
       ExtendedDescription TEXT,
       Visible INTEGER,
       Comment TEXT,
       Sequence INTEGER,
       ParentId TEXT,
       primary key (Id),
       constraint FK_DD4B87FE foreign key (ParentId) references COMMANDS
    )

    create table COMMAND_PROPERTIES (
        Id  integer primary key autoincrement,
       Name TEXT,
       Value TEXT,
       CommandId TEXT,
       constraint FK_71C9276E foreign key (CommandId) references COMMANDS
    )

    create table DATA_REPOSITORIES (
        Id TEXT not null,
       Name TEXT not null,
       Description TEXT,
       ContentId INTEGER,
       primary key (Id),
       constraint fk_DataRepository_Content foreign key (ContentId) references CONTENTS
    )

    create table HISTORY_ITEMS (
        Id TEXT not null,
       UserId TEXT,
       DateTime TEXT,
       State INTEGER,
       Sequence INTEGER,
       CommandId TEXT,
       primary key (Id),
       constraint fk_HistoryItem_Command foreign key (CommandId) references COMMANDS
    )

    create table INDIVIDUAL_RESULTS (
        Id  integer primary key autoincrement,
       IndividualId INTEGER,
       TimeId INTEGER,
       SimulationResultsId INTEGER,
       constraint fk_IndividualResults_QuantityValues foreign key (TimeId) references QUANTITY_VALUES,
       constraint fk_SimulationResults_IndividualResults foreign key (SimulationResultsId) references SIMULATION_RESULTS
    )

    create table INDIVIDUAL_RESULTS_QUANTITY_VALUES (
        IndividualResultsId INTEGER not null,
       QuantityValuesId INTEGER not null,
       primary key (IndividualResultsId, QuantityValuesId),
       constraint FK_1D481FCF foreign key (QuantityValuesId) references QUANTITY_VALUES,
       constraint FK_ABB6905B foreign key (IndividualResultsId) references INDIVIDUAL_RESULTS
    )

    create table CONTENTS (
        Id  integer primary key autoincrement,
       Data image
    )

    create table OBSERVED_DATA (
        Id TEXT not null,
       DataRepositoryId TEXT,
       ProjectId INTEGER,
       primary key (Id),
       constraint fk_ObservedData_DataRepository foreign key (DataRepositoryId) references DATA_REPOSITORIES,
       constraint fk_Project_ObservedData foreign key (ProjectId) references PROJECTS
    )

    create table PARAMETER_IDENTIFICATIONS (
        Id TEXT not null,
       Name TEXT not null,
       Description TEXT,
       ContentId INTEGER,
       PropertiesId INTEGER,
       ProjectId INTEGER,
       primary key (Id),
       constraint fk_ParameterIdentification_Content foreign key (ContentId) references CONTENTS,
       constraint fk_ParameterIdentification_Properties foreign key (PropertiesId) references CONTENTS,
       constraint fk_Project_ParameterIdentifications foreign key (ProjectId) references PROJECTS
    )

    create table PROJECTS (
        Id  integer primary key autoincrement,
       Name TEXT not null,
       Description TEXT,
       Version INTEGER,
       ContentId INTEGER,
       constraint fk_Project_Content foreign key (ContentId) references CONTENTS
    )

    create table QUANTITY_VALUES (
        Id  integer primary key autoincrement,
       QuantityPath TEXT,
       ColumnId TEXT,
       Data image
    )

    create table SENSITIVITY_ANALYSES (
        Id TEXT not null,
       Name TEXT not null,
       Description TEXT,
       ContentId INTEGER,
       PropertiesId INTEGER,
       ProjectId INTEGER,
       primary key (Id),
       constraint fk_SensitivityAnalysis_Content foreign key (ContentId) references CONTENTS,
       constraint fk_SensitivityAnalysis_Properties foreign key (PropertiesId) references CONTENTS,
       constraint fk_Project_SensitivityAnalyses foreign key (ProjectId) references PROJECTS
    )

    create table SIMULATION_ANALYSES (
        Id  integer primary key autoincrement,
       ContentId INTEGER,
       constraint fk_SimulationAnalyses_Content foreign key (ContentId) references CONTENTS
    )

    create table SIMULATION_CHARTS (
        Id TEXT not null,
       Name TEXT not null,
       Description TEXT,
       ContentId INTEGER,
       SimulationId TEXT,
       primary key (Id),
       constraint fk_SimulationChart_Content foreign key (ContentId) references CONTENTS,
       constraint fk_Simulation_Charts foreign key (SimulationId) references SIMULATIONS
    )

    create table SIMULATION_COMPARISONS (
        Id TEXT not null,
       Name TEXT not null,
       Description TEXT,
       ContentId INTEGER,
       ProjectId INTEGER,
       primary key (Id),
       constraint fk_SimulationComparison_Content foreign key (ContentId) references CONTENTS,
       constraint fk_Project_SummaryCharts foreign key (ProjectId) references PROJECTS
    )

    create table INDIVIDUAL_SIMULATION_COMPARISONS (
        Id TEXT not null,
       primary key (Id),
       constraint FK_AC274320 foreign key (Id) references SIMULATION_COMPARISONS
    )

    create table POPULATION_SIMULATION_COMPARISONS (
        Id TEXT not null,
       primary key (Id),
       constraint FK_80C30647 foreign key (Id) references SIMULATION_COMPARISONS
    )

    create table SIMULATION_RESULTS (
        Id  integer primary key autoincrement,
       TimeId INTEGER,
       constraint fk_SimulationResults_QuantityValues foreign key (TimeId) references QUANTITY_VALUES
    )

    create table USED_BUILDING_BLOCKS (
        Id TEXT not null,
       Name TEXT not null,
       TemplateId TEXT not null,
       Version INTEGER not null,
       StructureVersion INTEGER not null,
       BuildingBlockType TEXT not null,
       Altered INTEGER,
       SimulationId TEXT,
       primary key (Id),
       constraint fk_Simulation_BuildingBlocks foreign key (SimulationId) references SIMULATIONS
    )

    create table WORKSPACE_LAYOUT (
        Id  integer primary key autoincrement,
       ContentId INTEGER,
       constraint fk_WorkspaceLayout_Content foreign key (ContentId) references CONTENTS
    )
