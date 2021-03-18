Feature: ThreeNodeCluster

Scenario: Three node cluster
	Given Nodes of the cluster (grpc,memory) are
	| Name | Port  |
	| A        | 19001 |
	| B        | 19002 |  
	| C        | 19003 |  
	When 'A' starts
	And Wait 2000 ms
	And 'B' starts
	And Wait 1000 ms
	And 'C' starts
	And Wait 3000 ms
	Then test ends


Scenario: A W100 BW 100 A down C up 
Given Nodes of the cluster (grpc,memory) are
	| Name | Port  |
	| A        | 19001 |
	| B        | 19002 |  
	| C        | 19003 |  
	When 'A' starts
	And Wait 1000 ms
	And 'B' starts
	And Wait 1000 ms
	And 'A' writes 100 messages
	And 'B' writes 100 messages
	And Wait 1000 ms
	And 'A' ends
	And Wait 1000 ms
	And 'C' starts
	And Wait 1000 ms
	And 'C' offset of 'A' is 99
	Then test ends

