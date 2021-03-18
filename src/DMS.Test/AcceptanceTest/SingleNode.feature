Feature: SingleNode

Scenario: Single node starts
	Given Nodes of the cluster (grpc,memory) are
	| Name | Port  |
	| A        | 19001 |
	When 'A' starts
	And Wait 2000 ms
	Then test ends



Scenario: Single node starts and write 100 messages
Given Nodes of the cluster (grpc,memory) are
	| Name | Port  |
	| A        | 19001 |
	When 'A' starts
	And 'A' writes 100 messages
	And Wait 2000 ms
	Then test ends

Scenario: Single node starts trying to communicate with not-existing node
Given Nodes of the cluster (grpc,memory) are
	| Name | Port  |
	| A        | 19001 |
	When 'A' starts
	And 'A' connect to address http://localhost:19002
	And 'A' writes 100 messages
	And Wait 10000 ms
	Then test ends