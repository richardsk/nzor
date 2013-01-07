
declare @canUnintegrate bit
set @canUnintegrate = 1

--children/synonyms still integrated?
if (exists (select pn.NameID
	from provider.Name pn
	inner join provider.vwConcepts c on c.NameToID = pn.NameID
	where pn.NameId = @nameId and c.ConsensusNameID is not null
		and c.NameID <> @nameid))
begin
	set @canUnintegrate = 0
end

--other provider names that need parentage of this name to exist?
if (@canUnintegrate = 1)
begin
	if (exists (select pn.NameID
		from provider.Name pn
		inner join provider.vwConcepts c on c.NameToID = pn.NameID
		inner join consensus.Name cn on cn.NameID = pn.ConsensusNameID 
		inner join provider.Name pn2 on pn2.ConsensusNameID = cn.NameID and pn2.NameID <> pn.NameID 
		left join provider.vwConcepts c2 on c2.NameID = pn2.NameID and c2.ConceptRelationshipTypeID = '6A11B466-1907-446F-9229-D604579AA155' and c2.ConsensusConceptID is not null
		where pn.NameId = @nameId and c2.NameID is null))
	begin
		set @canUnintegrate = 0
	end	
end

select @canUnintegrate
