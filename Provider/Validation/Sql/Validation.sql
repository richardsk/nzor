--concept applications with missing vernacular concepts 
select *
from plant_name.conceptapplication ca
left join plant_name.vernacularconcept vc on vc.vernacularconceptid = ca.fromconceptid
where vc.vernacularconceptid is null

--concept applications with missing taxon concepts 
select *
from plant_name.conceptapplication ca
left join plant_name.taxonconcept tc on tc.taxonconceptid = ca.toconceptid
left join plant_name.namebasedconcept nbc on nbc.namebasedconceptid = ca.toconceptid
where tc.taxonconceptid is null and nbc.namebasedconceptid is null

