
delete consensus.conceptrelationship where conceptrelationshipid = 'DE7730C0-7C4F-4BEE-888D-CB2EAF1BD8D1'
delete consensus.concept where conceptid = 'A0C3B079-2EC4-4D05-A721-CD3A575691A7'
delete consensus.nameproperty where nameid = '64977BFD-B255-4686-97F6-75D1F4CDDD11'
delete consensus.name where nameid = '64977BFD-B255-4686-97F6-75D1F4CDDD11'


insert into consensus.Name
select '64977BFD-B255-4686-97F6-75D1F4CDDD11', '057D6434-A12A-460D-B705-4510603FAE4F', 'A5233111-61A0-4AE6-9C2B-5E8E71F1473A', 'Test_Name', null, getdate(), null

insert into consensus.NameProperty
select '186AF404-8EAD-4F0D-8614-B094D2BB145E', '64977BFD-B255-4686-97F6-75D1F4CDDD11', '1F64E93C-7EE8-40D7-8681-52B56060D750', null, null, 'Test_Name'

insert into consensus.Concept
select 'A0C3B079-2EC4-4D05-A721-CD3A575691A7', '64977BFD-B255-4686-97F6-75D1F4CDDD11', NULL, null, null, null, GETDATE(), null

--set root as parent
insert into consensus.ConceptRelationship 
select 'DE7730C0-7C4F-4BEE-888D-CB2EAF1BD8D1', 'A0C3B079-2EC4-4D05-A721-CD3A575691A7', '87F73532-0EA2-40BB-960A-AC03C60F26F5', '6A11B466-1907-446F-9229-D604579AA155', 1, null, getdate(), null

