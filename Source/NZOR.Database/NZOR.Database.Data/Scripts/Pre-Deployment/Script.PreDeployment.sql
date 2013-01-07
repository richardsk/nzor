/*
 Pre-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be executed before the build script.	
 Use SQLCMD syntax to include a file in the pre-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the pre-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/
/*
SET NOCOUNT ON

WHILE 1 = 1
    BEGIN    
    DELETE TOP (10000) FROM provider.TaxonPropertyValue 
    IF @@ROWCOUNT = 0        
        BEGIN        
        BREAK
        END
    END
	
WHILE 1 = 1
    BEGIN    
    DELETE TOP (10000) FROM provider.TaxonProperty 
    IF @@ROWCOUNT = 0        
        BEGIN        
        BREAK
        END
    END

WHILE 1 = 1
    BEGIN    
    DELETE TOP (10000) FROM provider.ConceptRelationship 
    IF @@ROWCOUNT = 0        
        BEGIN        
        BREAK
        END
    END

WHILE 1 = 1
    BEGIN    
    DELETE TOP (10000) FROM provider.ConceptApplication 
    IF @@ROWCOUNT = 0        
        BEGIN        
        BREAK
        END
    END

WHILE 1 = 1
    BEGIN    
    DELETE TOP (10000) FROM provider.Concept 
    IF @@ROWCOUNT = 0        
        BEGIN        
        BREAK
        END
    END

WHILE 1 = 1
    BEGIN    
    DELETE TOP (10000) FROM provider.NameProperty 
    IF @@ROWCOUNT = 0        
        BEGIN        
        BREAK
        END
    END

WHILE 1 = 1
    BEGIN    
    DELETE TOP (10000) FROM provider.Name 
    IF @@ROWCOUNT = 0        
        BEGIN        
        BREAK
        END
    END

WHILE 1 = 1
    BEGIN    
    DELETE TOP (10000) FROM provider.StackedName 
    IF @@ROWCOUNT = 0        
        BEGIN        
        BREAK
        END
    END

WHILE 1 = 1
    BEGIN    
    DELETE TOP (10000) FROM provider.ReferenceProperty 
    IF @@ROWCOUNT = 0        
        BEGIN        
        BREAK
        END
    END

WHILE 1 = 1
    BEGIN    
    DELETE TOP (10000) FROM provider.Reference
    IF @@ROWCOUNT = 0        
        BEGIN        
        BREAK
        END
    END

DELETE FROM DataSource 
DELETE FROM Provider 

WHILE 1 = 1
    BEGIN    
    DELETE TOP (10000) FROM consensus.TaxonPropertyValue 
    IF @@ROWCOUNT = 0        
        BEGIN        
        BREAK
        END
    END
	
WHILE 1 = 1
    BEGIN    
    DELETE TOP (10000) FROM consensus.TaxonProperty 
    IF @@ROWCOUNT = 0        
        BEGIN        
        BREAK
        END
    END

WHILE 1 = 1
    BEGIN    
    DELETE TOP (10000) FROM consensus.ConceptRelationship 
    IF @@ROWCOUNT = 0        
        BEGIN        
        BREAK
        END
    END

WHILE 1 = 1
    BEGIN    
    DELETE TOP (10000) FROM consensus.ConceptApplication 
    IF @@ROWCOUNT = 0        
        BEGIN        
        BREAK
        END
    END

WHILE 1 = 1
    BEGIN    
    DELETE TOP (10000) FROM consensus.Concept 
    IF @@ROWCOUNT = 0        
        BEGIN        
        BREAK
        END
    END

WHILE 1 = 1
    BEGIN    
    DELETE TOP (10000) FROM consensus.NameProperty 
    IF @@ROWCOUNT = 0        
        BEGIN        
        BREAK
        END
    END

WHILE 1 = 1
    BEGIN    
    DELETE TOP (10000) FROM consensus.Name
    IF @@ROWCOUNT = 0        
        BEGIN        
        BREAK
        END
    END

WHILE 1 = 1
    BEGIN    
    DELETE TOP (10000) FROM consensus.StackedName
    IF @@ROWCOUNT = 0        
        BEGIN        
        BREAK
        END
    END

WHILE 1 = 1
    BEGIN    
    DELETE TOP (10000) FROM consensus.ReferenceProperty
    IF @@ROWCOUNT = 0        
        BEGIN        
        BREAK
        END
    END

WHILE 1 = 1
    BEGIN    
    DELETE TOP (10000) FROM consensus.Reference
    IF @@ROWCOUNT = 0        
        BEGIN        
        BREAK
        END
    END


WHILE 1 = 1
    BEGIN    
    DELETE TOP (10000) FROM dbo.georegion
    IF @@ROWCOUNT = 0        
        BEGIN        
        BREAK
        END
    END
	
WHILE 1 = 1
    BEGIN    
    DELETE TOP (10000) FROM dbo.TaxonRank
    IF @@ROWCOUNT = 0        
        BEGIN        
        BREAK
        END
    END

*/