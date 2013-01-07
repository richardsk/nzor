CREATE FUNCTION consensus.[GetFullName]
(
      @NameId             uniqueidentifier
)
      
RETURNS nvarchar(1000)
AS
BEGIN
	
	DECLARE @NameNomCode varchar(5), @nameClassId uniqueidentifier, @fullName nvarchar(500)
	DECLARE @isRecomb bit, @isAnamorph bit

	SELECT @NameNomCode = RTRIM(LTRIM(UPPER(GoverningCode))) 
		  , @isRecomb = IsRecombination
		  , @isAnamorph = 0 --TODO ISNULL(NameIsAnamorph, 0)
		  ,@nameClassId = NameClassID
	FROM consensus.Name WHERE NameId = @NameId
	
	--root
	if (@NameId = '7C087DE1-FD0C-4997-8874-06D61D7CB244')
	begin
		return '<FullName><Name>ROOT</Name></FullName>'
	end

	if (@nameClassId = '05BCC19C-27E8-492C-8ADD-EC5F73325BC5')
	begin
		select top 1 @fullName = FullName from provider.Name where ConsensusNameID = @NameId
		return '<FullName><Name>' + @fullname + '</Name></FullName>'
	end

	IF ISNULL(@NameNomCode, '') = '' SET @NameNomCode = 'ICBN'
	
	DECLARE @status nvarchar(500) = '', @statAnam nvarchar(150) = ''

	--IF @flags = 1
	--BEGIN
		  DECLARE @invalid bit, @illegit bit, @dubious bit, @proparte bit, @aggr bit--, @anam bit--, @misapp bit
		  SELECT @invalid = case when CHARINDEX('nom. inval', nsp.Value) <> 0 then 1 else 0 end, 
				@illegit = case when CHARINDEX('nom. illeg', nsp.Value) <> 0 then 1 else 0 end,
				@dubious = case when CHARINDEX('nom. dub', nsp.Value) <> 0 then 1 else 0 end,
				@proparte = 0, --todo
				@aggr = 0 --todo
				--@anam = 0 
		  FROM consensus.Name n
			left join consensus.NameProperty nsp on nsp.NameID = n.NameID and nsp.NamePropertyTypeID = '65F94532-7687-4958-B5B8-71F54866BEAD'
		  WHERE n.NameId = @NameId

		  IF @NameNomCode in ('ICZN')
			  BEGIN
				  IF @invalid = 1 SET @status = @status + 'unavail., '  -- unavail.
				  IF @illegit = 1 SET @status = @status + 'inval., ' -- inval. 
			  END
		  ELSE
			  BEGIN
				IF @invalid = 1 SET @status = @status + 'nom. inv., ' 
				  IF @illegit = 1 SET @status = @status + 'nom. illegit., ' 
			  END
		  IF @dubious = 1 SET @status = @status + 'nom. dub., '
		  IF @proparte = 1 SET @status = @status + 'pro. parte., '
		  IF @aggr = 1 SET @status = @status + 'agg., '
		  IF @isAnamorph = 1 SET @statAnam =  ' [stat. anam.]'
      
		  IF @status <> ''
		  BEGIN
				SET @status = '(' + RTRIM(@status)
				SET @status = LEFT(@status, LEN(@status) - 1) + ')'
		  END
	--END


	DECLARE @Name table (
		  NameID uniqueidentifier
		  , NameCanonical nvarchar(500)
		  , NameAuthors nvarchar(255)
		  , BasionymAuthors nvarchar(255)
		  , ReferenceId uniqueidentifier
		  , InCitation bit
		  , Citation nvarchar(800)
		  , InAuthors nvarchar(200)
		  , NameYearOf nvarchar(30)
		  , NameYearOn nvarchar(10)
		  , Page nvarchar(100)
		  , RankPk uniqueidentifier
		  , RankSort int
		  , ShowRank bit
		  , IncludeInFullName bit
		  , RankAbbrev nvarchar(50)
		  , RankStyle int
		  , IsName bit
		  , IsAutonym bit
		  , IsMisapplied bit
		  , Orthography nvarchar(100)
		  , AccordingToID uniqueidentifier
		  , position int identity (1,1)
		  );

		  
	INSERT INTO @Name (NameID, RankSort, ShowRank, IncludeInFullName, RankAbbrev, RankPk, IsName, AccordingToID)
	SELECT distinct NameID, sn.SortOrder, ShowRank, IncludeInFullName, tr.Name, tr.TaxonRankID, 1, sn.AccordingToID
	from consensus.StackedName sn
	inner join TaxonRank tr on tr.TaxonRankID = sn.TaxonRankID
	where SeedNameID = @NameId
	order by SortOrder
	

	update na
		set NameAuthors = anp.Value,
			BasionymAuthors = bnp.Value, 
			NameYearOf = ynp.Value,
			NameYearOn = yonp.Value,
			NameCanonical = cnp.Value, 
			IsAutonym = case when pncp.Value = cnp.Value then 1 else 0 end,
			InCitation = 0, -- TODO NameInCitation
			ReferenceId = pinp.RelatedID,
			Page = null, --TODO NamePage
			IsMisapplied = 0, --TODO NameMisapplied
			Orthography = onp.Value
	from @Name na
	left join consensus.vwConcepts pnc on pnc.NameID = na.NameID and pnc.ConceptRelationshipTypeID = '6A11B466-1907-446F-9229-D604579AA155' and pnc.IsActive = 1
	left join consensus.NameProperty cnp on cnp.NameID = na.NameID and cnp.NamePropertyTypeID = '1F64E93C-7EE8-40D7-8681-52B56060D750'
	left join consensus.NameProperty anp on anp.NameID = na.NameID and anp.NamePropertyTypeID = '006D86A8-08A5-4C1A-BC08-C07B0225E01B'
	left join consensus.NameProperty bnp on bnp.NameID = na.NameID and bnp.NamePropertyTypeID = '6272B3D0-C91B-4FD4-A714-662B10FA6E68'
	left join consensus.NameProperty ynp on ynp.NameID = na.NameID and ynp.NamePropertyTypeID = 'C9E19D58-BD92-4F3C-96E8-DE7FA6DCD5F7'
	left join consensus.NameProperty yonp on yonp.NameID = na.NameID and yonp.NamePropertyTypeID = '4EC79307-E41A-4540-8647-03EF48795435'
	left join consensus.NameProperty pncp on pncp.NameID = pnc.NameToID and pncp.NamePropertyTypeID = '1F64E93C-7EE8-40D7-8681-52B56060D750'
	left join consensus.NameProperty pinp on pinp.NameID = na.NameID and pinp.NamePropertyTypeID = 'DEDC63F0-FB2A-420B-9932-786B4347DA45'
	left join consensus.NameProperty onp on onp.NameID = na.NameID and onp.NamePropertyTypeID = '98606514-CC85-4E42-9EF4-9E594ECA03A8'

	UPDATE N
		SET RankStyle = (SELECT Style FROM TaxonRankStyle 
							WHERE TaxonRankID = RankPk AND GoverningCode = @NameNomCode)
	FROM @Name N


	DECLARE @Orthography nvarchar(120)
	SET @Orthography = ''

	SELECT @Orthography = LTRIM(ISNULL(Orthography, '')) FROM @Name 
		WHERE IsName = 1
	IF (@Orthography <> '')
	begin
		if (charindex('as ', @Orthography) = 1 and len(@Orthography) > 3) set @Orthography = SUBSTRING(@orthography, 4, len(@Orthography))
		SET @Orthography = ' [as ' + @Orthography + ']'
		SET @Orthography = REPLACE(@Orthography, '<i>', '')
		SET @Orthography = REPLACE(@Orthography, '</i>', '')
	end

	declare @rankSort int
	select @rankSort = RankSort from @Name where NameID = @NameId

	-- remove unnecessary names
	IF (SELECT MAX(RankSort) FROM @Name) >= 3000
		  BEGIN
				DELETE FROM @Name WHERE (RankSort < 3000 or RankSort > @rankSort) and NameID <> @nameid
            
				-- virus names do not need genus
				IF @NameNomCode = 'ICVN'
				BEGIN
					DELETE FROM @Name WHERE (RankSort = 3000 and RankSort < @rankSort) and NameID <> @nameid
				END

				--if a infra generic name then remove genus too
				if (@rankSort < 4200 and @rankSort > 3000)
				begin
					DELETE FROM @Name WHERE (RankSort >= 3000 and RankSort < 4200) and NameID <> @nameid
				end
		  END
	ELSE
		  BEGIN
				DELETE FROM @Name WHERE IsName = 0
		  END



	-- remove names of ranks that are not added to the full name
	-- this is defined in TaxonRank
	--DELETE FROM @Name WHERE ISNULL(IncludeRank,1) = 0 AND IsName = 0

	DELETE FROM @Name WHERE RankStyle = 0 AND IsName = 0

	--SELECT * FROM @Name

	--remove multiple names with same rank (multiple parent relationships)	
	declare @pos int, @lastRank uniqueidentifier, @thisRank uniqueidentifier
	Select @pos = MAX(position), @lastRank = null from @Name
	while (@pos > 0)
	begin
		if (@lastRank is null)
		begin
			select @lastRank = RankPk from @Name where position = @pos
		end
		else
		begin
			set @thisRank = null
			select @thisRank = RankPk from @Name where position = @pos
			if (@thisRank is null) --row gone
			begin
				set @thisRank = @lastRank
			end
			else if (@thisRank = @lastRank)
			begin
				delete @Name where RankPk = @thisRank and position <> @pos and NameID <> @NameId
			end
			set @lastRank = @thisRank
		end
		set @pos = @pos - 1 
	end


	UPDATE @Name SET NameAuthors = 'sensu ' + NameAuthors 
		WHERE IsMisapplied = 1 and NameAuthors IS NOT NULL
	

	--  format namecanonical except for viruses
	--IF @bitFormatted = 1 AND @NameNomCode NOT IN ('ICVN')
	--BEGIN
	--	  UPDATE @Name
	--			SET NameCanonical = '<i>' + NameCanonical + '</i>'
	--	   WHERE RankPK <> '486C20EF-1296-4F08-B9B8-CEEEEF4FCEAE'
	--END

	-- add rank abbreviations
	--UPDATE @Name 
	--SET NameCanonical = RankAbbrev + ' ' + NameCanonical
	--WHERE ShowRank = 1 AND IsName = 1

	--UPDATE @Name SET NameCanonical = '''' + NameCanonical + '''' 
	--	WHERE RankPk = '486C20EF-1296-4F08-B9B8-CEEEEF4FCEAE'

	--UPDATE @Name 
	--SET NameCanonical = RankAbbrev + ' ' + NameCanonical
	--WHERE IsName = 0
	--	AND RankStyle IN (2, 4)

	--UPDATE @Name 
	--SET NameCanonical = '(' + NameCanonical + ')'
	--WHERE IsName = 0
	--	AND RankStyle IN (3, 4)

	--
	--IF @bitLiterature = 1
	--BEGIN
		UPDATE n
			SET Citation = (select Value from consensus.ReferenceProperty rp where rp.ReferenceID = n.ReferenceId and rp.ReferencePropertyTypeID = '7F835876-B459-4023-90E4-6C22646FBE07')	
		from @Name n
		WHERE n.RankSort = (Select MAX(Ranksort) 
			FROM @Name WHERE IsAutonym = 0);

		  UPDATE @Name
				SET Citation = RTRIM(Citation + ' ' + ISNULL(Page,''))
			WHERE ISNULL(Citation, '') <> ''
		  UPDATE @Name SET Citation = LEFT(Citation, LEN(Citation) - 1)
				WHERE Citation like '%,' OR Citation like '%:'
		  UPDATE N
			  SET InAuthors = (SELECT Value FROM consensus.ReferenceProperty RF
									WHERE RF.ReferenceID = N.ReferenceId
										AND RF.ReferencePropertyTypeID = '037D1485-C6C1-47D2-9619-234F743E9FF6')
			FROM @Name N				
			WHERE InCitation = 1
		  UPDATE @Name SET InAuthors = LEFT(InAuthors, CHARINDEX(',', InAuthors)-1) + ' et al.'
				--CASE @bitFormatted 
					--WHEN 0 THEN ' et al.'
					--ELSE ' <i>et al.</i>'
				--END
			WHERE InAuthors like '%;%'
		  UPDATE @Name SET InAuthors = LEFT(InAuthors, CHARINDEX(',', InAuthors)-1) 
			WHERE InAuthors like '%,%'
		  UPDATE @Name SET Citation = ', ' + Citation WHERE ISNULL(Citation, '') <> ''
		  UPDATE @Name SET Citation = InAuthors +  Citation WHERE ISNULL(InAuthors, '') <> ''
	--END

	-- calculate year values
	UPDATE @Name
		  SET NameYearOf = ISNULL(NameYearOf, '') +  
					  CASE ISNULL(NameYearOn, '')
							WHEN '' THEN ''
							ELSE ' [' + NameYearOn + ']'
					  END

	-- add year 
	IF (@NameNomCode IN ('ICZN') and @isRecomb = 1)
		  BEGIN
				--UPDATE @Name
				--	  SET NameYearOf =  CASE ISNULL(NameAuthors, '')
				--										  WHEN '' THEN ''
				--										  ELSE ', ' 
				--									  END
				--								+ ISNULL(NameYearOf, '')
				--WHERE ISNULL(NameYearOf, '') <> ''
				UPDATE @Name
				SET NameAuthors = '(' + ISNULL(NameAuthors, '') + case when NameYearOf = '' then ')' else RTRIM(ISNULL(', ' + NameYearOf, '')) + ')' end,
					NameYearOf = ''
				where BasionymAuthors is not null
		  END
	
	--ELSE
		  --BEGIN
				--IF @bitLiterature = 1
				--BEGIN
				--	UPDATE @Name
				--		  SET NameYearOf = ' (' + ISNULL(NameYearOf, '') + ')'
				--	WHERE ISNULL(NameYearOf, '') <> ''
				--END
				--ELSE
				--BEGIN
					--UPDATE @Name
					--	  SET NameYearOf = ' ' + ISNULL(NameYearOf, '')
					--WHERE ISNULL(NameYearOf, '') <> ''
				--END
		  --END
	--END

	-- add authors string
	--UPDATE @Name
	--	  SET NameCanonical = RTRIM(NameCanonical + ' ' + ISNULL(NameAuthors, ''))
	--WHERE RankSort = (Select MAX(Ranksort) FROM @Name WHERE IsAutonym = 0);

	--IF @bitLiterature = 1
	--BEGIN
	--	UPDATE @Name
	--		  SET NameCanonical = RTRIM(NameCanonical +  
	--				CASE InCitation
	--					  WHEN 1 THEN ' in '
	--					  ELSE '' 
	--				END +
	--				+ ISNULL(Citation, ''))
	--	WHERE RankSort = (Select MAX(Ranksort) FROM @Name WHERE IsAutonym = 0);
	--END

	--UPDATE @Name
	--	  SET NameCanonical = NameCanonical +  ISNULL(NameYearOf, '')
	--WHERE RankSort = (Select MAX(Ranksort) FROM @Name WHERE IsAutonym = 0);

	declare @fulltext nvarchar(500)
	declare @maxRank int
	Select @maxRank = MAX(Ranksort) FROM @Name WHERE IsAutonym = 0

	--SELECT @fulltext = COALESCE(@fulltext + ' ', '') + 
	--	case when (ShowRank = 1 and IsName = 1) or (IsName = 0 AND RankStyle IN (2, 4)) then RankAbbrev + ' ' else '' end +
	--	case when IsName = 0 AND RankStyle IN (3, 4) then '(' else '' end + 
	--	case when RankPk = '486C20EF-1296-4F08-B9B8-CEEEEF4FCEAE' then '''' else '' end + 
	--	NameCanonical +
	--	case when RankPk = '486C20EF-1296-4F08-B9B8-CEEEEF4FCEAE' then '''' else '' end + 
	--	case when IsName = 0 AND RankStyle IN (3, 4) then ')' else '' end +
	--	case when RankSort = @maxRank then isnull(' ' + rtrim(NameAuthors),'') else '' end +
	--	case when InCitation = 1 then ' in ' + ISNULL(Citation,'') else '' end 
	--FROM @Name where IncludeInFullName = 1 or NameID = @NameId
	--ORDER BY RankSort ASC

	declare @numNames int
	select @numNames = COUNT(*) from @Name

	if (@numNames = 0) --missing concepts!
	begin
		select @fullName = '<FullName><Name>' + FullName + '</Name></FullName>' 
		from provider.Name
		where ConsensusNameID = @NameId
	end
	else
	begin
		SELECT @FullName = COALESCE(@FullName, '') + 
			case when (@NameNomCode <> 'ICZN' and ShowRank = 1 and IsName = 1) or (IsName = 0 AND RankStyle IN (2, 4)) then isnull('<Rank>' + RankAbbrev + '</Rank>','') else '' end +		
			'<Name>' +  
			case when NameID <> @NameId and RankStyle IN (3, 4) then '(' else '' end +
			case when NameID <> @NameId and RankStyle = 5 then '(' else '' end +
			case when RankPk = '486C20EF-1296-4F08-B9B8-CEEEEF4FCEAE' then '''' else '' end + NameCanonical +
			case when RankPk = '486C20EF-1296-4F08-B9B8-CEEEEF4FCEAE' then '''' else '' end +
			case when NameID <> @NameId and RankStyle IN (3, 4) then ')' else '' end + 
			case when NameID <> @NameId and RankStyle = 5 then ')' else '' end +
			'</Name>' +
			case when RankSort = @maxRank and LEN(NameAuthors) > 0 then 
				isnull('<Authors>' + replace(rtrim(NameAuthors),'&','&amp;') + '</Authors>','') 
				else '' end +
			case when RankSort = @maxRank and LEN(NameYearOf) > 0 
				and (BasionymAuthors IS NULL or @NameNomCode <> 'ICZN') then 
				isnull('<Year>' + NameYearOf + '</Year>','')
				else '' end +			
			case when InCitation = 1 and LEN(Citation) > 0 then '<Citation>in ' + ISNULL(Citation,'') + '</Citation>' else '' end 
		FROM @Name where IncludeInFullName = 1 or NameID = @NameId
		ORDER BY RankSort ASC

			--SET @FullName = RTRIM(REPLACE( @FullName, '</i> <i>',  ' '))
	
		SET @FullName = '<FullName>' + @FullName + 
			case when len(@orthography) > 0 then '<Orthography>' + @Orthography + '</Orthography>' else '' end + 
			--case when len(@status) > 0 then '<Status>' + @status + '</Status>' else '' end +
			case when len(@statAnam) > 0 then '<AnamorphStatus>' + @statAnam + '</AnamorphStatus>' else '' end +
			'</FullName>'
	end
               
    RETURN (@FullName)
END
