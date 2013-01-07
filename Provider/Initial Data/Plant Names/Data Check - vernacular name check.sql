use NZOR_Data
go


select Na.NameID, na.FullName,
con.conceptid,
	con.ProviderReferenceID,
	n2.FullName
	from prov.Name na
		inner join dbo.NameClass nc on na.NameClassID = nc.NameClassID
		 and nc.Title = 'Vernacular Name'
	 left join prov.Concept con on Na.nameid = con.NameID
	 left join prov.ConceptRelationship cr on con.ConceptID = cr.FromConceptID
	 left join prov.Concept con2 on cr.ToConceptID = con2.ConceptID
	 left join prov.Name n2 on con2.NameID = n2.NameID
	 
				