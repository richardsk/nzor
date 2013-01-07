/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/

:r ".\Reference Data\TaxonRank Reference Data.sql"
:r ".\Reference Data\TaxonRankStyle Reference Data.sql"

:r ".\Reference Data\ReferenceType Reference Data.sql"
:r ".\Reference Data\ReferencePropertyType Reference Data.sql"
:r ".\Reference Data\ReferencePropertyMap Reference Data.sql"

:r ".\Reference Data\NameClass Reference Data.sql"
:r ".\Reference Data\NamePropertyType Reference Data.sql"
:r ".\Reference Data\NamePropertyLookUp Reference Data.sql"

:r ".\Reference Data\ConceptRelationshipType Reference Data.sql"
:r ".\Reference Data\ConceptApplicationType Reference Data.sql"

:r ".\Reference Data\GeographicSchema Reference Data.sql"
:r ".\Reference Data\GeoRegion Reference Data.sql"
:r ".\Reference Data\Country Reference Data.sql"

:r ".\Reference Data\TaxonPropertyClass Reference Data.sql"
:r ".\Reference Data\TaxonPropertyType Reference Data.sql"
:r ".\Reference Data\TaxonPropertyLookUp Reference Data.sql"

:r ".\Reference Data\ConsensusName Reference Data.sql"
:r ".\Reference Data\Consensus Concept Reference Data.sql"
:r ".\Reference Data\ConsensusNameProperty Reference Data.sql"
:r ".\Reference Data\Transformation Reference Data.sql"

:r ".\Reference Data\MessageType Reference Data.sql"
:r ".\Reference Data\User Reference Data.sql"
:r ".\Reference Data\UserMessageType Reference Data.sql"
:r ".\Reference Data\Provider Reference Data.sql"
:r ".\Reference Data\DataSource Reference Data.sql"
:r ".\Reference Data\DataSourceEndpoint Reference Data.sql"
:r ".\Reference Data\DataType Reference Data.sql"
:r ".\Reference Data\ScheduledTask Reference Data.sql"
:r ".\Reference Data\ExternalLookupService Reference Data.sql"
:r ".\Reference Data\AttachmentPoint Reference Data.sql"
:r ".\Reference Data\AttachmentPointDataSource Reference Data.sql"
:r ".\Reference Data\StatisticType Reference Data.sql"

