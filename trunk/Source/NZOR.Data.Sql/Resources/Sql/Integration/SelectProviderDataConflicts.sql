select distinct cn.NameID, 
	cn.FullName, 
	npt.NamePropertyTypeID, 
	npt.Name as NamePropertyType, 
	ds1.DataSourceID as DataSource1, 
	gpnp.Value as Value1, 
	gpnp.Sequence as Sequence1, 
	case when rpn1.ConsensusNameID is not null then rpn1.ConsensusNameID else rpr1.ConsensusReferenceID end as RelatedID1, 
	ds2.DataSourceID as DataSource2, 
	gpnp2.Value as Value2, 
	gpnp2.Sequence as Sequence2, 
	case when rpn2.ConsensusNameID is not null then rpn2.ConsensusNameID else rpr2.ConsensusReferenceID end as RelatedID2, 
	tr.SortOrder
from consensus.Name cn 
inner join TaxonRank tr on tr.TaxonRankID = cn.TaxonRankID
inner join provider.Name pn on pn.ConsensusNameID = cn.NameID
inner join [admin].DataSource ds1 on ds1.DataSourceID = pn.DataSourceID
inner join provider.NameProperty gpnp on gpnp.NameID = pn.NameID 
inner join provider.Name pn2 on pn2.ConsensusNameID = cn.NameID and pn2.NameID <> pn.NameID
inner join [admin].DataSource ds2 on ds2.DataSourceID = pn2.DataSourceID
inner join provider.NameProperty gpnp2 on gpnp2.NameID = pn2.NameID and gpnp2.NamePropertyTypeID = gpnp.NamePropertyTypeID
inner join dbo.NamePropertyType npt on npt.NamePropertyTypeID = gpnp.NamePropertyTypeID
left join provider.Name rpn1 on rpn1.NameID = gpnp.RelatedID
left join provider.Reference rpr1 on rpr1.ReferenceID = gpnp.RelatedID
left join provider.Name rpn2 on rpn2.NameID = gpnp2.RelatedID
left join provider.Reference rpr2 on rpr2.ReferenceID = gpnp2.RelatedID
where case when charindex(' ex ', gpnp.Value) <> 0 
		then replace(replace(replace(replace(SUBSTRING(gpnp.value, CHARINDEX(' ex ',gpnp.Value) + 4, LEN(gpnp.value)), ' and ', ' & '), ' et ', ' & '), '.', ' '), '  ', ' ')
	else
		replace(replace(replace(replace(gpnp.value, ' and ', ' & '), ' et ', ' & '), '.', ' '), '  ', ' ')
	end
	<>
	case when charindex(' ex ', gpnp2.Value) <> 0 
		then replace(replace(replace(replace(SUBSTRING(gpnp2.value, CHARINDEX(' ex ',gpnp2.Value) + 4, LEN(gpnp2.value)), ' and ', ' & '), ' et ', ' & '), '.', ' '), '  ', ' ')
	else
		replace(replace(replace(replace(gpnp2.value, ' and ', ' & '), ' et ', ' & '), '.', ' '), '  ', ' ')
	end
	and	ds1.DataSourceID <> ds2.DataSourceID
order by tr.SortOrder, cn.FullName

