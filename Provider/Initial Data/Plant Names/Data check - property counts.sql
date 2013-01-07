use NZOR_Data
go


select nc.Title,ncp.PropertyName,
	(select COUNT(*) from prov.NameProperty np
		where np.NameClassPropertyID = ncp.NameClassPropertyID)
	from dbo.NameClassProperty ncp
		inner join dbo.NameClass nc on ncp.NameClassID = nc.NameClassID

select na.FullName,
	(select COUNT(*) from prov.Concept con
		where con.NameID = na.NameID)
	from prov.Name na
		
		
--select cpt.PropertyName, (select COUNT(*) from prov.ConceptProperty cp
--		where cp.ConceptPropertyID = cpt.ConceptPropertyTypeID)
--	from dbo.ConceptPropertyType cpt
	
select crt.Relationship,
	(select COUNT(*) from prov.ConceptRelationship cr
		where cr.RelationshipTypeID = crt.ConceptRelationshipTypeID)
	from dbo.ConceptRelationshipType crt
	
select parentId, preferredid, count(s.Conceptid) from
(select con.ConceptID, count(cr1.ConceptRelationshipID) as parentId, --crt1.Relationship,
	count(cr2.ConceptRelationshipID) as PreferredId
	from prov.Concept con
		left join prov.ConceptRelationship cr1 on con.ConceptID = cr1.FromConceptID
			and cr1.RelationshipTypeID = '6A11B466-1907-446F-9229-D604579AA155'
		--left join dbo.ConceptRelationshipType crt1 
		--	on cr1.RelationshipTypeID = crt1.ConceptRelationshipTypeID
		left join prov.ConceptRelationship cr2 on con.ConceptID = cr2.FromConceptID
			and cr2.RelationshipTypeID = '8630A70A-942C-4F0A-9DFC-58F7CC64303C'
	group by ConceptID) as s
group by parentId, PreferredId
order by parentId, PreferredId
		
	
	
	
		