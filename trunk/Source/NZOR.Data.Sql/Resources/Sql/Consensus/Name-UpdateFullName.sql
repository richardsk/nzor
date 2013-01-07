
update consensus.Name
set FullName = consensus.GetFullName(NameID)
where NameID = @nameID


--full name properties
--delete any existing ones
delete consensus.NameProperty
where NameID = @nameID
	and NamePropertyTypeID in ('86E7590B-EF34-4E19-970B-608703B858A5', '86B84828-E1C0-45BD-A5C0-7B272EDC97EF', '00806321-C8BD-4518-9539-1286DA02CA7D', 'F721F463-5F16-4333-9C7D-DDF848F2D1A9', '88020F95-1282-4D9A-819A-0973F7F50284', 'C4954CF2-6A07-469B-B470-2D56E60C6666')


if (@nameClassID = 'A5233111-61A0-4AE6-9C2B-5E8E71F1473A')
begin
	insert consensus.NameProperty 
	select newid(), NameID, '86E7590B-EF34-4E19-970B-608703B858A5', null, null, replace(dbo.ApplyXSLT(FullName, 
		(select convert(nvarchar(max), xslt) from dbo.transformation where name = 'NameText_FullName'), 0),'&amp;', '&')
	from consensus.Name
	where NameID = @nameID

	insert consensus.NameProperty 
	select newid(), NameID, '86B84828-E1C0-45BD-A5C0-7B272EDC97EF', null, null, replace(dbo.ApplyXSLT(FullName, 
		(select convert(nvarchar(max), xslt) from dbo.transformation where name = 'NameText_FullNameFormatted'), 0),'&amp;', '&')
	from consensus.Name
	where NameID = @nameID

	insert consensus.NameProperty 
	select newid(), NameID, '00806321-C8BD-4518-9539-1286DA02CA7D', null, null, replace(dbo.ApplyXSLT(FullName, 
		(select convert(nvarchar(max), xslt) from dbo.transformation where name = 'NameText_PartialName'), 0),'&amp;', '&')
	from consensus.Name
	where NameID = @nameID

	insert consensus.NameProperty 
	select newid(), NameID, 'F721F463-5F16-4333-9C7D-DDF848F2D1A9', null, null, replace(dbo.ApplyXSLT(FullName, 
		(select convert(nvarchar(max), xslt) from dbo.transformation where name = 'NameText_PartialNameFormatted'), 0),'&amp;', '&')
	from consensus.Name
	where NameID = @nameID
end

if (@nameClassID = '05BCC19C-27E8-492C-8ADD-EC5F73325BC5')
begin
	insert consensus.NameProperty 
	select newid(), NameID, '88020F95-1282-4D9A-819A-0973F7F50284', null, null, replace(dbo.ApplyXSLT(FullName, 
		(select convert(nvarchar(max), xslt) from dbo.transformation where name = 'NameText_FullName'), 0),'&amp;', '&')
	from consensus.Name
	where NameID = @nameID
end

if (@nameClassID = '3D3A13B8-C673-459C-B98D-8A5B08E3CA44')
begin
	insert consensus.NameProperty 
	select newid(), NameID, 'C4954CF2-6A07-469B-B470-2D56E60C6666', null, null, replace(dbo.ApplyXSLT(FullName, 
		(select convert(nvarchar(max), xslt) from dbo.transformation where name = 'NameText_FullName'), 0),'&amp;', '&')
	from consensus.Name
	where NameID = @nameID
end

