Feature: TwoNodeCluster

Scenario: Two node cluster
	Given Nodes of the cluster (grpc,memory) are
	| Name | Port  |
	| A        | 19001 |
	| B        | 19002 |  
	When 'A' starts
	And Wait 2000 ms
	And 'B' starts
	And Wait 3000 ms
	Then test ends

Scenario: Two node cluster explicit client connection
Given Nodes of the cluster (grpc,memory) are
	| Name | Port  |
	| A        | 19001 |
	| B        | 19002 |  
	When 'A' starts server
	And 'B' starts server
	And Wait 500 ms
	And 'A' connect to 'B'
	And Wait 10000 ms
	Then test ends

Scenario: A,B AW100
Given Nodes of the cluster (grpc,memory) are
	| Name | Port  |
	| A        | 19001 |
	| B        | 19002 |  
	When 'A' starts server
	And 'B' starts server
	And Wait 500 ms
	And 'A' connect to 'B'
	And 'B' connect to 'A'
	And Wait 1000 ms
	And 'A' writes 100 messages
	And Wait 1000 ms
	Then test ends


Scenario: A,B BDown AW100
Given Nodes of the cluster (grpc,memory) are
	| Name | Port  |
	| A        | 19001 |
	| B        | 19002 |  
	When 'A' starts server
	And 'B' starts server
	And Wait 500 ms
	And 'A' connect to 'B'
	And 'B' connect to 'A'
	And Wait 1000 ms
	And 'B' ends
	And Wait 1000 ms
	And 'A' writes 100 messages
	And Wait 1000 ms
	Then test ends

	
Scenario: A, A->B, B B->A AW100
Given Nodes of the cluster (grpc,memory) are
	| Name | Port  |
	| A        | 19001 |
	| B        | 19002 |  
	When 'A' starts server
	And 'A' connect to 'B'
	And Wait 500 ms
	And 'B' starts server
	And 'B' connect to 'A'
	And Wait 1000 ms
	And 'B' ends
	And Wait 1000 ms
	And 'A' writes 100 messages
	And Wait 1000 ms
	Then test ends




Scenario: Two node cluster. B down A recognize it
Given Nodes of the cluster (grpc,memory) are
	| Name | Port  |
	| A        | 19001 |
	| B        | 19002 |  
	When 'A' starts
	And Wait 2000 ms
	And 'B' starts
	And Wait 3000 ms
	And 'B' ends
	And Wait 1000 ms
	And 'A' see 'B' down
	Then test ends


Scenario: Two node cluster. A W100 and B is Aligned
Given Nodes of the cluster (grpc,memory) are
	| Name | Port  |
	| A        | 19001 |
	| B        | 19002 |  
	When 'A' starts
	And Wait 2000 ms
	And 'B' starts
	And Wait 1000 ms
	And 'A' writes 100 messages
	And Wait 1000 ms
	And 'B' offset of 'A' is 99
	Then test ends


Scenario: A up B up A W100 B down B up
Given Nodes of the cluster (grpc,memory) are
	| Name | Port  |
	| A        | 19001 |
	| B        | 19002 |  
	When 'A' starts
	And Wait 2000 ms
	And 'B' starts
	And Wait 1000 ms
	And 'A' writes 100 messages
	And Wait 1000 ms
	And 'B' ends
	And Wait 1000 ms
	And 'B' starts
	And Wait 1000 ms
	Then test ends


Scenario: Two node cluster with late B
	Given Nodes of the cluster (grpc,memory) are
	| Name | Port  |
	| A        | 19001 |
	| B        | 19002 |  
	When 'A' starts
	And Wait 10000 ms
	And 'B' starts
	And Wait 3000 ms
	Then test ends

Scenario: Two node cluster with late B and A W100
Given Nodes of the cluster (grpc,memory) are
	| Name | Port  |
	| A        | 19001 |
	| B        | 19002 |  
	When 'A' starts
	And Wait 10000 ms
	And 'B' starts
	And Wait 3000 ms
	And 'A' writes 100 messages
	And Wait 11000 ms
	Then test ends


Scenario: A up B up A W100 B down A W100 B up
Given Nodes of the cluster (grpc,memory) are
	| Name | Port  |
	| A        | 19001 |
	| B        | 19002 |  
	When 'A' starts
	And Wait 2000 ms
	And 'B' starts
	And Wait 1000 ms
	And 'A' writes 100 messages
	And Wait 1000 ms
	And 'B' ends
	And 'A' writes 100 messages
	And Wait 1000 ms
	And 'B' starts
	And Wait 2000 ms
	And 'B' offset of 'A' is 199
	Then test ends


Scenario: Two node cluster. A W100 and B W100 both are Aligned
Given Nodes of the cluster (grpc,memory) are
	| Name | Port  |
	| A        | 19001 |
	| B        | 19002 |  
	When 'A' starts
	And Wait 2000 ms
	And 'B' starts
	And Wait 1000 ms
	And 'A' writes 100 messages
	And Wait 1000 ms
	And 'B' writes 100 messages
	And Wait 2000 ms
	And 'B' offset of 'A' is 99
	And 'A' offset of 'B' is 99
	Then test ends

