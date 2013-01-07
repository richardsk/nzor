delete [admin].Setting where Name = @name

insert [admin].Setting
select newid(), @name, @value
