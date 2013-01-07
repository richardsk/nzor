CREATE TABLE [admin].[Statistic]
(
	StatisticId uniqueidentifier not null,
	StatisticTypeId uniqueidentifier not null,
	Value nvarchar(max) not null,
	DateGenerated datetime not null
)
